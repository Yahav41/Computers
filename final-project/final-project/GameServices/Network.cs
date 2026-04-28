using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace final_project.GameServices
{
    public class Network : IGameNetwork
    {
        private readonly NetworkServer _server = new NetworkServer();

        public event Action<PlayerState> OpponentStateReceived
        {
            add => _server.OpponentDataReceived += value;
            remove => _server.OpponentDataReceived -= value;
        }

        public event Action<string> StatusChanged
        {
            add => _server.StatusChanged += value;
            remove => _server.StatusChanged -= value;
        }

        public bool IsConnected => _server.IsConnected;

        public async Task StartOrConnectAsync(string _)
        {
            await _server.StartServerAsync();
        }

        public Task SendAsync(PlayerState state)
        {
            return _server.SendPlayerStateAsync(state);
        }

        public void Stop()
        {
            _server.StopServer();
        }
    }

    public class ClientNetwork : IGameNetwork
    {
        private readonly NetworkClient _client = new NetworkClient();

        public event Action<PlayerState> OpponentStateReceived
        {
            add => _client.OpponentDataReceived += value;
            remove => _client.OpponentDataReceived -= value;
        }

        public event Action<string> StatusChanged
        {
            add => _client.StatusChanged += value;
            remove => _client.StatusChanged -= value;
        }

        public bool IsConnected => _client.IsConnected;

        public async Task StartOrConnectAsync(string ip)
        {
            await _client.ConnectAsync(ip);
        }

        public Task SendAsync(PlayerState state)
        {
            return _client.SendPlayerStateAsync(state);
        }

        public void Stop()
        {
            _client.Disconnect();
        }
    }
}
