using final_project.GameObjects;
using finalproject.GameServices;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using Windows.UI.Core;
using UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding;

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
        public event Action<string> StatusChanged;
        public event Action<BulletState> BulletFired; 
        public async Task SendBulletAsync(BulletState bullet) 
        { 
            try 
            { 
                if (dataWriter == null) return; 
                string json = JsonConvert.SerializeObject(bullet); 
                uint messageLength = (uint)json.Length; 
                dataWriter.WriteByte(1); 
                dataWriter.WriteUInt32(messageLength); 
                dataWriter.WriteString(json); 
                await dataWriter.StoreAsync(); 
                await dataWriter.FlushAsync(); 
            } 
            catch (Exception ex) 
            { 
                OnStatusChanged($"Error sending bullet: {ex.Message}"); 
            } 
        }

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

        private async void OnConnectionReceived(
            StreamSocketListener sender,
            StreamSocketListenerConnectionReceivedEventArgs args)
        {
            try
            {
                clientSocket = args.Socket;
                dataWriter = new DataWriter(clientSocket.OutputStream);
                dataReader = new DataReader(clientSocket.InputStream);
                dataReader.UnicodeEncoding = UnicodeEncoding.Utf8;
                dataWriter.UnicodeEncoding = UnicodeEncoding.Utf8;

                // FIX: Dispatch to UI thread - this event fires on background thread
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(
                    CoreDispatcherPriority.Normal,
                    () => OnStatusChanged($"Client connected from {clientSocket.Information.RemoteAddress}")
                );

                await ListenForDataAsync();
            }
            catch (Exception ex)
            {
                // FIX: Dispatch to UI thread
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
                    // Read message type prefix
                    uint prefixRead = await dataReader.LoadAsync(sizeof(uint));
                    if (prefixRead < sizeof(uint)) break;

                    byte messageType = dataReader.ReadByte(); // "BULLET" or "PLAYER"

                    // Read message length
                    uint bytesRead = await dataReader.LoadAsync(sizeof(uint));
                    if (bytesRead < sizeof(uint)) break;

                    uint messageLength = dataReader.ReadUInt32();

                    // Read message content
                    bytesRead = await dataReader.LoadAsync(messageLength);
                    if (bytesRead == 0) break;

                    string json = dataReader.ReadString(messageLength);

                    try
                    {
                        if (messageType == 1)
                        {
                            BulletState bullet = JsonConvert.DeserializeObject<BulletState>(json);
                            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(
                                CoreDispatcherPriority.Normal,
                                () => BulletFired?.Invoke(bullet)
                            );
                        }
                        else if (messageType == 0)
                        {
                            PlayerState opponentState = JsonConvert.DeserializeObject<PlayerState>(json);
                            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(
                                CoreDispatcherPriority.Normal,
                                () => OpponentDataReceived?.Invoke(opponentState)
                            );
                        }
                    }
                    catch (Exception parseEx)
                    {
                        Debug.WriteLine($"Error parsing JSON: {parseEx.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(
                    CoreDispatcherPriority.Normal,
                    () => OnStatusChanged($"Error listening for data: {ex.Message}")
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
                if (dataWriter == null) return;

                string json = JsonConvert.SerializeObject(state);
                uint messageLength = (uint)json.Length;

                // Send message type prefix
                dataWriter.WriteByte(0);
                dataWriter.WriteUInt32(messageLength);
                dataWriter.WriteString(json);

                await dataWriter.StoreAsync();
                await dataWriter.FlushAsync();
            }
            catch (Exception ex)
            {
                OnStatusChanged($"Error sending data: {ex.Message}");
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