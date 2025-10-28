using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System;

namespace GameEngine.Services
{
    public class GameEvents
    {
        public Action<VirtualKey> OnKeyClick;
        public Action OnRun;
        public Action<VirtualKey> OnKeyRelease;
        public Action<int, int> onUpdateScore;
        public Action<int> OnRemoveLifes;
        public GameEvents()
        {

        }
    }
}
