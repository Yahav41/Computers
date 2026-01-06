using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using Windows.UI.Core;
using static final_project.GameObjects.Players;

namespace final_project.GameServices
{
    public class NetworkClient
    {
        private StreamSocket socket;
        private DataWriter dataWriter;
        private DataReader dataReader;
        private const string PORT = "22222";

        // Event to notify UI when opponent's data is received
        public event Action<PlayerState> OpponentDataReceived;
        public event Action<string> StatusChanged;

        public async Task ConnectAsync(string serverIpAddress)
        {
            try
            {
                socket = new StreamSocket();

                // Create a HostName object for the server address
                HostName hostName = new HostName(serverIpAddress);

                // Connect to the server
                await socket.ConnectAsync(hostName, PORT);

                dataWriter = new DataWriter(socket.OutputStream);
                dataReader = new DataReader(socket.InputStream);

                dataReader.UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding.Utf8;
                dataWriter.UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding.Utf8;

                OnStatusChanged("Connected to server: " + serverIpAddress);

                // Start listening for incoming data
                await ListenForDataAsync();
            }
            catch (Exception ex)
            {
                OnStatusChanged("Connection error: " + ex.Message);
            }
        }

        private async Task ListenForDataAsync()
        {
            try
            {
                while (true)
                {
                    // Read the message length (first 4 bytes = uint)
                    uint bytesRead = await dataReader.LoadAsync(sizeof(uint));

                    if (bytesRead < sizeof(uint))
                    {
                        break; // Server disconnected
                    }

                    uint messageLength = dataReader.ReadUInt32();

                    // Read the actual message
                    bytesRead = await dataReader.LoadAsync(messageLength);
                    if (bytesRead == 0) break;

                    string json = dataReader.ReadString(messageLength);

                    // Parse the received player state
                    PlayerState opponentState =
                        JsonConvert.DeserializeObject<PlayerState>(json);

                    // Notify the UI/Game that opponent data was received
                    var task = CoreApplication.MainView.CoreWindow.Dispatcher
                        .RunAsync(CoreDispatcherPriority.Normal, () =>
                        {
                            OpponentDataReceived?.Invoke(opponentState);
                        });
                }
            }
            catch (Exception ex)
            {
                OnStatusChanged("Error listening for data: " + ex.Message);
            }
            finally
            {
                if (socket != null)
                {
                    socket.Dispose();
                    socket = null;
                }
            }
        }

        public async Task SendPlayerStateAsync(PlayerState state)
        {
            try
            {
                if (dataWriter == null) return;

                string json = JsonConvert.SerializeObject(state);

                // Write message length first
                uint messageLength = (uint)json.Length;
                dataWriter.WriteUInt32(messageLength);

                // Write the message
                dataWriter.WriteString(json);

                // Send it all
                await dataWriter.StoreAsync();
            }
            catch (Exception ex)
            {
                OnStatusChanged("Error sending data: " + ex.Message);
            }
        }

        public void Disconnect()
        {
            socket?.Dispose();
            dataWriter?.Dispose();
            dataReader?.Dispose();
        }

        private void OnStatusChanged(string status)
        {
            StatusChanged?.Invoke(status);
        }

        public bool IsConnected => socket != null;
    }
}
