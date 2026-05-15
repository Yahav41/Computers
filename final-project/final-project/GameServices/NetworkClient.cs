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
    // מימוש של לקוח רשת פשוט שמתחבר לשרת, מקבל ושולח הודעות בפורמט JSON
    public class NetworkClient
    {
        private StreamSocket ServerSocket; // סוקט לחיבור לשרת
        private DataWriter dataWriter; // כותב לזרם היציאה
        private DataReader dataReader; // קורא מזרם הקלט
        private const string PORT = "11111"; // פורט קבוע לשיחה

        // אירועים שהממשק הגבוה יכול להירשם אליהם
        public event Action<PlayerState> OpponentDataReceived;
        public event Action<IReadOnlyList<CoverState>> CoversReceived;
        public event Action<string> StatusChanged;

        // התחברות אסינכרונית לשרת לפי כתובת IP
        public async Task ConnectAsync(string serverIpAddress)
        {
            try
            {
                Disconnect(); // אם היה חיבור קודם - נתק ונקווה התחלה נקייה

                ServerSocket = new StreamSocket();
                HostName hostName = new HostName(serverIpAddress);
                await ServerSocket.ConnectAsync(hostName, PORT); // התחברות לשרת

                // אתחול קורא וכותב לזרמים עם קידוד UTF8
                dataWriter = new DataWriter(ServerSocket.OutputStream);
                dataReader = new DataReader(ServerSocket.InputStream);
                dataReader.UnicodeEncoding = UnicodeEncoding.Utf8;
                dataWriter.UnicodeEncoding = UnicodeEncoding.Utf8;

                OnStatusChanged($"Connected to server {serverIpAddress}:{PORT}");

                _ = ListenForDataAsync(); // מתחיל לולאת הקבלה ברקע (לא מחכה לה)
            }
            catch (Exception ex)
            {
                OnStatusChanged("Connection error: " + ex.Message);
                Disconnect(); // במקרה של שגיאה - נתק ונקה משאבים
            }
        }

        // לולאה אסינכרונית שמקבלת הודעות מהשרת לפי פרוטוקול: [uint length][json string]
        private async Task ListenForDataAsync()
        {
            try
            {
                while (ServerSocket != null && ServerSocket.InputStream != null)
                {
                    // קורא קודם 4 בתים שמייצגים את אורך ההודעה
                    uint bytesRead = await dataReader.LoadAsync(sizeof(uint));
                    if (bytesRead < sizeof(uint))
                        break;

                    uint messageLength = dataReader.ReadUInt32();

                    // מטעין את שאר ההודעה לפי האורך
                    bytesRead = await dataReader.LoadAsync(messageLength);
                    if (bytesRead == 0)
                        break;

                    // קורא את ה־JSON ומפענח ל־NetworkMessage
                    string json = dataReader.ReadString(messageLength);
                    var message = JsonConvert.DeserializeObject<NetworkMessage>(json);
                    if (message == null)
                        continue;

                    // מטפל לפי סוג ההודעה שמגיע
                    switch (message.MessageType)
                    {
                        case NetworkMessageType.PlayerState:
                            var opponentState = JsonConvert.DeserializeObject<PlayerState>(message.Payload);
                            if (opponentState != null)
                            {
                                // קוראים את האירוע על ה־UI thread כי מאזינים עלולים לעדכן UI
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
                // מדווח שגיאה דרך ה־UI thread
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(
                    CoreDispatcherPriority.Normal,
                    () => OnStatusChanged("Error listening for data: " + ex.Message)
                );
            }
            finally
            {
                // ניקוי במקרה של יציאה מהלולאה
                Disconnect();
            }
        }

        // שליחת מצב השחקן לשרת בפורמט NetworkMessage JSON
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

                // כותב ראשון את האורך ואז את המחרוזת
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

        // ניתוק וניקוי משאבים של הלקוח
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

        // הודעת סטטוס כללית למאזינים
        private void OnStatusChanged(string status)
        {
            StatusChanged?.Invoke(status);
        }

        // האם הלקוח מחובר כרגע
        public bool IsConnected => ServerSocket != null;
    }
}