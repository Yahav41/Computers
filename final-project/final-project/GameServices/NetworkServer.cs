using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using Windows.UI.Core;
using static final_project.GameObjects.Players;

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

        // Event to notify UI when opponent's data is received
        public event Action<PlayerState> OpponentDataReceived;
        public event Action<string> StatusChanged;

        public async Task StartServerAsync()
        {
            try
            {
                socketListener = new StreamSocketListener();

                // Register for the ConnectionReceived event
                socketListener.ConnectionReceived += OnConnectionReceived;

                // Bind the socket to port 1920
                await socketListener.BindServiceNameAsync(PORT);
                isListening = true;

                OnStatusChanged("Server started. Waiting for connections on port " + PORT);
            }
            catch (Exception ex)
            {
                OnStatusChanged("Error starting server: " + ex.Message);
            }
        }

        private async void OnConnectionReceived(
            StreamSocketListener sender,
            StreamSocketListenerConnectionReceivedEventArgs args)
        {
            try
            {
                clientSocket = args.Socket;
                dataWriter = new DataWriter(clientSocket.OutputStream);
                dataReader = new DataReader(clientSocket.InputStream);

                dataReader.UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding.Utf8;
                dataWriter.UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding.Utf8;

                OnStatusChanged("Client connected from: " +
                    clientSocket.Information.RemoteAddress);

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
                        break; // Connection closed
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
                if (clientSocket != null)
                {
                    clientSocket.Dispose();
                    clientSocket = null;
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

        public void StopServer()
        {
            isListening = false;
            socketListener?.Dispose();
            clientSocket?.Dispose();
            dataWriter?.Dispose();
            dataReader?.Dispose();
        }

        private void OnStatusChanged(string status)
        {
            StatusChanged?.Invoke(status);
        }

        public bool IsConnected => clientSocket != null;
    }
}
