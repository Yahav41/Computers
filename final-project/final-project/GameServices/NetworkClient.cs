using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using Windows.UI.Core;

namespace final_project.GameServices
{
    public class NetworkClient
    {
        private StreamSocket ServerSocket;
        private DataWriter dataWriter;
        private DataReader dataReader;
        private const string PORT = "11111";

        public event Action<PlayerState> OpponentDataReceived;
        public event Action<IReadOnlyList<CoverState>> CoversReceived;
        public event Action<string> StatusChanged;

        public async Task ConnectAsync(string serverIpAddress)
        {
            try
            {
                Disconnect();

                ServerSocket = new StreamSocket();
                HostName hostName = new HostName(serverIpAddress);
                await ServerSocket.ConnectAsync(hostName, PORT);

                dataWriter = new DataWriter(ServerSocket.OutputStream);
                dataReader = new DataReader(ServerSocket.InputStream);
                dataReader.UnicodeEncoding = UnicodeEncoding.Utf8;
                dataWriter.UnicodeEncoding = UnicodeEncoding.Utf8;

                OnStatusChanged($"Connected to server {serverIpAddress}:{PORT}");

                _ = ListenForDataAsync();
            }
            catch (Exception ex)
            {
                OnStatusChanged("Connection error: " + ex.Message);
                Disconnect();
            }
        }

        private async Task ListenForDataAsync()
        {
            try
            {
                while (ServerSocket != null && ServerSocket.InputStream != null)
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
                Disconnect();
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

        public void Disconnect()
        {
            try
            {
                ServerSocket?.Dispose();
                dataWriter?.Dispose();
                dataReader?.Dispose();
            }
            catch { }
            finally
            {
                ServerSocket = null;
                dataWriter = null;
                dataReader = null;
            }
        }

        private void OnStatusChanged(string status)
        {
            StatusChanged?.Invoke(status);
        }

        public bool IsConnected => ServerSocket != null;
    }
}