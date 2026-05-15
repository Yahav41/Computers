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
    // מימוש פשוט של שרת TCP שמקבל חיבור יחיד ומעביר הודעות JSON
    public class NetworkServer
    {
        private StreamSocketListener socketListener; // מאזין לחיבורים נכנסים
        private StreamSocket clientSocket; // סוקט לחיבור קליינט יחיד
        private DataWriter dataWriter; // לכתיבה לזרם היציאה של הלקוח
        private DataReader dataReader; // לקריאה מזרם הקלט של הלקוח
        private bool isListening = false;
        private const string PORT = "11111"; // פורט הקשבה קבוע

        // אירועים שממשק גבוה יכול להירשם אליהם
        public event Action<PlayerState> OpponentDataReceived;
        public event Action<IReadOnlyList<CoverState>> CoversReceived;
        public event Action<string> StatusChanged;

        // מתחיל להאזין לחיבורים נכנסים; רץ אסינכרונית
        public async Task StartServerAsync()
        {
            try
            {
                socketListener = new StreamSocketListener();
                socketListener.ConnectionReceived += OnConnectionReceived;
                await socketListener.BindServiceNameAsync(PORT); // קשירה לפורט
                isListening = true;
                OnStatusChanged($"Server started. Waiting for connections on port {PORT}");
            }
            catch (Exception ex)
            {
                // מדווח סטטוס במקרה של שגיאה בהפעלת השרת
                OnStatusChanged("Error starting server: " + ex.Message);
            }
        }

        // מטפל בחיבור נכנס - מאחסן את הסוקט של הלקוח ומאתחל DataReader/Writer
        private async void OnConnectionReceived(StreamSocketListener sender,StreamSocketListenerConnectionReceivedEventArgs args)
        {
            try
            {
                clientSocket = args.Socket;
                dataWriter = new DataWriter(clientSocket.OutputStream);
                dataReader = new DataReader(clientSocket.InputStream);
                dataReader.UnicodeEncoding = UnicodeEncoding.Utf8;
                dataWriter.UnicodeEncoding = UnicodeEncoding.Utf8;

                // מעדכן סטטוס על ממשק ה-UI — לכן משתמש ב-Dispatcher של ה-MainView
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(
                    CoreDispatcherPriority.Normal,
                    () => OnStatusChanged($"Client connected from {clientSocket.Information.RemoteAddress}")
                );

                // מתחיל לולאת קריאה שעוברת על הודעות מהלקוח
                await ListenForDataAsync();
            }
            catch (Exception ex)
            {
                // מדווח שגיאה על ה-UI במקרה שהחיבור נכשל
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(
                    CoreDispatcherPriority.Normal,
                    () => OnStatusChanged("Connection error: " + ex.Message)
                );
            }
        }

        // לולאה שמקבלת הודעות מהמקליינט: קוטעת לפי פורמט (uint length + json string)
        private async Task ListenForDataAsync()
        {
            try
            {
                // ממשיך לקרוא כל עוד הסוקט קיים וזרם הקלט זמין
                while (clientSocket != null && clientSocket.InputStream != null)
                {
                    // קורא גודל הודעה מסוג uint (4 בתים)
                    uint bytesRead = await dataReader.LoadAsync(sizeof(uint));
                    if (bytesRead < sizeof(uint))
                        break;

                    uint messageLength = dataReader.ReadUInt32();

                    // טוען את התוכן לפי האורך
                    bytesRead = await dataReader.LoadAsync(messageLength);
                    if (bytesRead == 0)
                        break;

                    // קורא מחרוזת JSON באורך הנתון
                    string json = dataReader.ReadString(messageLength);
                    var message = JsonConvert.DeserializeObject<NetworkMessage>(json);

                    if (message == null)
                        continue;

                    // מפענח את סוג ההודעה וממפה לאירועים מתאימים
                    switch (message.MessageType)
                    {
                        case NetworkMessageType.PlayerState:
                            var opponentState = JsonConvert.DeserializeObject<PlayerState>(message.Payload);
                            if (opponentState != null)
                            {
                                // האירועים חייבים להתבצע על ה-UI thread כי מאזינים עשויים לעדכן UI
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
                // מדווח על שגיאה בליסטנר דרך ה-UI thread
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(
                    CoreDispatcherPriority.Normal,
                    () => OnStatusChanged("Error listening for data: " + ex.Message)
                );
            }
            finally
            {
                // נקיון סוקט כשהלולאה מסתיימת
                clientSocket?.Dispose();
                clientSocket = null;
            }
        }

        // שליחת מצב שחקן ללקוח המחובר (סוגר על DataWriter)
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

                // פורמט פרוטוקול: כותבים קודם את האורך ואז את המחרוזת
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

        // שליחת snapshot של המחסות ללקוח
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

        // מפסיק את השרת ומנקה משאבים
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

        // עוזר להודיע על שינוי סטטוס למאזינים
        private void OnStatusChanged(string status)
        {
            StatusChanged?.Invoke(status);
        }

        // מציין האם קיים לקוח מחובר כרגע
        public bool IsConnected => clientSocket != null;
    }
}