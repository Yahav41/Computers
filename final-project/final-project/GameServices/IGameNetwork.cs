using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace final_project.GameServices
{
    public interface IGameNetwork
    {
        event Action<PlayerState> OpponentStateReceived;
        event Action<string> StatusChanged;

        Task StartOrConnectAsync(string ipOrNull);
        Task SendAsync(PlayerState state);
        void Stop();

        bool IsConnected { get; }
    }
}
