using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace final_project.GameServices
{
    public interface IGameNetwork
    {
        event Action<PlayerState> OpponentStateReceived;

        event Action<IReadOnlyList<CoverState>> CoversReceived;

        event Action<string> StatusChanged;

        Task StartOrConnectAsync(string ipOrNull);

        Task SendAsync(PlayerState state);

        Task SendCoversAsync(IReadOnlyList<CoverState> covers);

        void Stop();

        bool IsConnected { get; }
    }
}