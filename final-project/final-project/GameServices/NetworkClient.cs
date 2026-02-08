using final_project.GameObjects;
using finalproject.GameServices;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using Windows.UI.Core;
using UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding;

namespace final_project.GameServices
{
    public class NetworkClient
    {
        private StreamSocket socket;
        private DataWriter dataWriter;
        private DataReader dataReader;
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
                dataWriter.WriteString("BULLET|"); 
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

        public async Task ConnectAsync(string serverIpAddress)
        {
            try
            {
                Disconnect();

                socket = new StreamSocket();
                HostName hostName = new HostName(serverIpAddress);
                await socket.ConnectAsync(hostName, PORT);

                dataWriter = new DataWriter(socket.OutputStream);
                dataReader = new DataReader(socket.InputStream);
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
                while (socket != null && socket.InputStream != null)
                {
                    // Read the message length (4 bytes for uint)
                    uint bytesRead = await dataReader.LoadAsync(sizeof(uint));
                    if (bytesRead < sizeof(uint))
                        break;

                    uint messageLength = dataReader.ReadUInt32();

                    // Load the exact number of bytes for the string
                    bytesRead = await dataReader.LoadAsync(messageLength);
                    if (bytesRead == 0)
                        break;

                    // Read the string with the byte length
                    string json = dataReader.ReadString(messageLength);

                    try
                    {
                        PlayerState opponentState = JsonConvert.DeserializeObject<PlayerState>(json);

                        var task = CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(
                            CoreDispatcherPriority.Normal,
                            () => OpponentDataReceived?.Invoke(opponentState));
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
            () => OnStatusChanged($"Error listening for data: {ex.Message}"));
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

                string json = JsonConvert.SerializeObject(state);

                // Get the byte length of the UTF-8 encoded string
                byte[] buffer = Encoding.UTF8.GetBytes(json);
                uint messageLength = (uint)buffer.Length;

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


        public void Disconnect()
        {
            try
            {
                socket?.Dispose();
                dataWriter?.Dispose();
                dataReader?.Dispose();
            }
            catch { }
            finally
            {
                socket = null;
                dataWriter = null;
                dataReader = null;
            }
        }

        private void OnStatusChanged(string status)
        {
            StatusChanged?.Invoke(status);
        }

        public bool IsConnected => socket != null;
    }
}