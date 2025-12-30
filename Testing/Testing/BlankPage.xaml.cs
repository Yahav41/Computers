using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Testing
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BlankPage : Page
    {
        private DatagramSocket _socket;
        private IOutputStream _peerStream;
        public BlankPage()
        {
            this.InitializeComponent();
        }
        private async void InitSocket_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string myPort = MyPortTextBox.Text;
                string peerIp = PeerIpTextBox.Text;
                string peerPort = PeerPortTextBox.Text;

                _socket = new DatagramSocket();
                _socket.MessageReceived += Socket_MessageReceived;

                // Bind to *my* UDP port so I can receive. [web:1]
                await _socket.BindServiceNameAsync(myPort);

                // Prepare stream to peer's IP+port. [web:17]
                HostName peerHost = new HostName(peerIp);
                _peerStream = await _socket.GetOutputStreamAsync(peerHost, peerPort);

                AppendLog($"Bound on {myPort}, ready to send to {peerIp}:{peerPort}");
            }
            catch (Exception ex)
            {
                AppendLog("Init error: " + ex.Message);
            }
        }
        private async void Send_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_socket == null || _peerStream == null)
                {
                    AppendLog("Socket or peer stream not ready.");
                    return;
                }

                using (var writer = new DataWriter(_peerStream))
                {
                    string msg = MessageToSendTextBox.Text;
                    writer.WriteString(msg);
                    await writer.StoreAsync();
                    await writer.FlushAsync();
                }

                AppendLog("Sent: " + MessageToSendTextBox.Text);
            }
            catch (Exception ex)
            {
                AppendLog("Send error: " + ex.Message);
            }
        }
        private async void Socket_MessageReceived(DatagramSocket sender,DatagramSocketMessageReceivedEventArgs args)
        {
            using (var reader = args.GetDataReader())
            {
                uint len = reader.UnconsumedBufferLength;
                string msg = reader.ReadString(len);
                string fromHost = args.RemoteAddress.CanonicalName;
                string fromPort = args.RemotePort;

                await Dispatcher.RunAsync(
                    Windows.UI.Core.CoreDispatcherPriority.Normal,
                    () => AppendLog($"Received from {fromHost}:{fromPort} -> {msg}")
                );
            }
        }
        private void AppendLog(string text)
        {
            LogTextBlock.Text += text + Environment.NewLine;
        }
    }
}
