using System;
using System.Collections.Generic;
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

        public bool IsConnected => _server.IsConnected;

        public async Task StartOrConnectAsync(string _)
        {
            await _server.StartServerAsync();
        }

        public Task SendAsync(PlayerState state)
        {
            return _server.SendPlayerStateAsync(state);
        }

        public Task SendCoversAsync(IReadOnlyList<CoverState> covers)
        {
            return _server.SendCoverStatesAsync(covers);
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

        public async Task StartOrConnectAsync(string ip)
        {
            await _client.ConnectAsync(ip);
        }

        public Task SendAsync(PlayerState state)
        {
            return _client.SendPlayerStateAsync(state);
        }

        public Task SendCoversAsync(IReadOnlyList<CoverState> covers)
        {
            return Task.CompletedTask;
        }

        public void Stop()
        {
            _client.Disconnect();
        }
    }
}