using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using Windows.UI.Core;

namespace final_project.GameServices
{
    public class NetworkServer
    {
        private StreamSocketListener socketListener;
        private StreamSocket clientSocket;
        private DataWriter dataWriter;
        private DataReader dataReader;
        private bool isListening = false;
        private const string PORT = "11111";

        public event Action<PlayerState> OpponentDataReceived;
        public event Action<IReadOnlyList<CoverState>> CoversReceived;
        public event Action<string> StatusChanged;

        public async Task StartServerAsync()
        {
            try
            {
                socketListener = new StreamSocketListener();
                socketListener.ConnectionReceived += OnConnectionReceived;
                await socketListener.BindServiceNameAsync(PORT);
                isListening = true;
                OnStatusChanged($"Server started. Waiting for connections on port {PORT}");
            }
            catch (Exception ex)
            {
                OnStatusChanged("Error starting server: " + ex.Message);
            }
        }

        private async void OnConnectionReceived(StreamSocketListener sender,StreamSocketListenerConnectionReceivedEventArgs args)
        {
            try
            {
                clientSocket = args.Socket;
                dataWriter = new DataWriter(clientSocket.OutputStream);
                dataReader = new DataReader(clientSocket.InputStream);
                dataReader.UnicodeEncoding = UnicodeEncoding.Utf8;
                dataWriter.UnicodeEncoding = UnicodeEncoding.Utf8;

                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(
                    CoreDispatcherPriority.Normal,
                    () => OnStatusChanged($"Client connected from {clientSocket.Information.RemoteAddress}")
                );

                await ListenForDataAsync();
            }
            catch (Exception ex)
            {
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(
                    CoreDispatcherPriority.Normal,
                    () => OnStatusChanged("Connection error: " + ex.Message)
                );
            }
        }

        private async Task ListenForDataAsync()
        {
            try
            {
                while (clientSocket != null && clientSocket.InputStream != null)
                {
                    uint bytesRead = await dataReader.LoadAsync(sizeof(uint));
                    if (bytesRead < sizeof(uint))
                        break;

                    uint messageLength = dataReader.ReadUInt32();
                    bytesRead = await dataReader.LoadAsync(messageLength);
                    if (bytesRead == 0)
                        break;

                    string json = dataReader.ReadString(messageLength);
                    var message = JsonConvert.DeserializeObject<NetworkMessage>(json);

                    if (message == null)
                        continue;

                    switch (message.MessageType)
                    {
                        case NetworkMessageType.PlayerState:
                            var opponentState = JsonConvert.DeserializeObject<PlayerState>(message.Payload);
                            if (opponentState != null)
                            {
                                var task = CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(
                                    CoreDispatcherPriority.Normal,
                                    () => OpponentDataReceived?.Invoke(opponentState)
                                );
                            }
                            break;

                        case NetworkMessageType.CoversSnapshot:
                            var covers = JsonConvert.DeserializeObject<List<CoverState>>(message.Payload);
                            if (covers != null)
                            {
                                var task = CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(
                                    CoreDispatcherPriority.Normal,
                                    () => CoversReceived?.Invoke(covers)
                                );
                            }
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(
                    CoreDispatcherPriority.Normal,
                    () => OnStatusChanged("Error listening for data: " + ex.Message)
                );
            }
            finally
            {
                clientSocket?.Dispose();
                clientSocket = null;
            }
        }

        public async Task SendPlayerStateAsync(PlayerState state)
        {
            try
            {
                if (dataWriter == null)
                    return;

                var payload = JsonConvert.SerializeObject(state);
                var message = new NetworkMessage
                {
                    MessageType = NetworkMessageType.PlayerState,
                    Payload = payload
                };

                string json = JsonConvert.SerializeObject(message);
                uint messageLength = (uint)json.Length;

                dataWriter.WriteUInt32(messageLength);
                dataWriter.WriteString(json);

                await dataWriter.StoreAsync();
                await dataWriter.FlushAsync();
            }
            catch (Exception ex)
            {
                OnStatusChanged("Error sending data: " + ex.Message);
            }
        }

        public async Task SendCoverStatesAsync(IReadOnlyList<CoverState> covers)
        {
            try
            {
                if (dataWriter == null || covers == null)
                    return;

                var payload = JsonConvert.SerializeObject(covers);
                var message = new NetworkMessage
                {
                    MessageType = NetworkMessageType.CoversSnapshot,
                    Payload = payload
                };

                string json = JsonConvert.SerializeObject(message);
                uint messageLength = (uint)json.Length;

                dataWriter.WriteUInt32(messageLength);
                dataWriter.WriteString(json);

                await dataWriter.StoreAsync();
                await dataWriter.FlushAsync();
            }
            catch (Exception ex)
            {
                OnStatusChanged("Error sending covers: " + ex.Message);
            }
        }

        public void StopServer()
        {
            isListening = false;
            try
            {
                socketListener?.Dispose();
                clientSocket?.Dispose();
                dataWriter?.Dispose();
                dataReader?.Dispose();
            }
            catch { }
            finally
            {
                socketListener = null;
                clientSocket = null;
                dataWriter = null;
                dataReader = null;
            }
        }

        private void OnStatusChanged(string status)
        {
            StatusChanged?.Invoke(status);
        }

        public bool IsConnected => clientSocket != null;
    }
}