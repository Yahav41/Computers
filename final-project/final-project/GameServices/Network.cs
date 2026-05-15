using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace final_project.GameServices
{
    // מימוש של IGameNetwork שמקיף את ה-NetworkServer המקומי
    public class Network : IGameNetwork
    {
        private readonly NetworkServer _server = new NetworkServer();

        // מאפייני אירועים שממופים ישירות לאירועים ב- NetworkServer
        public event Action<PlayerState> OpponentStateReceived
        {
            add => _server.OpponentDataReceived += value;   // הוסף מאזין לשרת
            remove => _server.OpponentDataReceived -= value; // הסר מאזין מהשרת
        }

        public event Action<IReadOnlyList<CoverState>> CoversReceived
        {
            add => _server.CoversReceived += value;
            remove => _server.CoversReceived -= value;
        }

        public event Action<string> StatusChanged
        {
            add => _server.StatusChanged += value;
            remove => _server.StatusChanged -= value;
        }

        // מצב חיבור מועבר ישירות מהשרת
        public bool IsConnected => _server.IsConnected;

        // התחלת השרת (כאן הפרמטר לא בשימוש — תמיד מתחיל כשרת)
        public async Task StartOrConnectAsync(string _)
        {
            await _server.StartServerAsync();
        }

        // שליחת מצב השחקן דרך השרת
        public Task SendAsync(PlayerState state)
        {
            return _server.SendPlayerStateAsync(state);
        }

        // שליחת מצב המחסות דרך השרת
        public Task SendCoversAsync(IReadOnlyList<CoverState> covers)
        {
            return _server.SendCoverStatesAsync(covers);
        }

        // עצירת השרת
        public void Stop()
        {
            _server.StopServer();
        }
    }

    // מימוש של IGameNetwork שמקיף לקוח רשת (מחבר לשרת מרוחק)
    public class LocalNetwork : IGameNetwork
    {
        private readonly NetworkClient _client = new NetworkClient();

        // מיפוי אירועים ל- NetworkClient
        public event Action<PlayerState> OpponentStateReceived
        {
            add => _client.OpponentDataReceived += value;
            remove => _client.OpponentDataReceived -= value;
        }

        public event Action<IReadOnlyList<CoverState>> CoversReceived
        {
            add => _client.CoversReceived += value;
            remove => _client.CoversReceived -= value;
        }

        public event Action<string> StatusChanged
        {
            add => _client.StatusChanged += value;
            remove => _client.StatusChanged -= value;
        }

        public bool IsConnected => _client.IsConnected;

        // התחברות ל-IP של השרת
        public async Task StartOrConnectAsync(string ip)
        {
            await _client.ConnectAsync(ip);
        }

        // שליחת מצב השחקן דרך הלקוח
        public Task SendAsync(PlayerState state)
        {
            return _client.SendPlayerStateAsync(state);
        }

        // ב- LocalNetwork אין שליחת CoverStates (מוחזיר CompletedTask)
        public Task SendCoversAsync(IReadOnlyList<CoverState> covers)
        {
            return Task.CompletedTask;
        }

        // ניתוק הלקוח
        public void Stop()
        {
            _client.Disconnect();
        }
    }
}