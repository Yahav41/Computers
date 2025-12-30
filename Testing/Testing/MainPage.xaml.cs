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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Testing
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private DatagramSocket _socket;
        private IOutputStream _outputStream;
        public MainPage()
        {
            this.InitializeComponent();
        }
        private async void StartHost_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string port = PortTextBox.Text;

                _socket = new DatagramSocket();
                _socket.MessageReceived += Socket_MessageReceived;

                // Bind to local port to listen for any sender on this UDP port
                await _socket.BindServiceNameAsync(port); // Host scenario. [web:1]

                AppendLog($"Host listening on UDP port {port}");
            }
            catch (Exception ex)
            {
                AppendLog("Host error: " + ex.Message);
            }
        }

        private async void Socket_MessageReceived(DatagramSocket sender,DatagramSocketMessageReceivedEventArgs args)
        {
            using (var reader = args.GetDataReader())
            {
                uint len = reader.UnconsumedBufferLength;
                string msg = reader.ReadString(len);

                // Extract remote info
                string remoteHost = args.RemoteAddress.CanonicalName;
                string remotePort = args.RemotePort;

                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    AppendLog($"Received from {remoteHost}:{remotePort} -> {msg}");
                });
            }
        }
        private async void StartClient_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string hostIp = HostIpTextBox.Text;
                string port = PortTextBox.Text;

                _socket = new DatagramSocket();
                _socket.MessageReceived += Socket_MessageReceived;

                // Client can also listen for responses, bind to any local port (empty string)
                await _socket.BindServiceNameAsync(""); // ephemeral port. [web:1]

                HostName remoteHost = new HostName(hostIp);
                _outputStream = await _socket.GetOutputStreamAsync(remoteHost, port); // [web:17]

                AppendLog($"Client ready. Target {hostIp}:{port}");
            }
            catch (Exception ex)
            {
                AppendLog("Client error: " + ex.Message);
            }
        }
        private async void Send_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_socket == null || _outputStream == null)
                {
                    AppendLog("Socket not initialized or remote not set.");
                    return;
                }

                using (var writer = new DataWriter(_outputStream))
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
        private void AppendLog(string text)
        {
            LogTextBlock.Text += text + Environment.NewLine;
        }
    }
}
