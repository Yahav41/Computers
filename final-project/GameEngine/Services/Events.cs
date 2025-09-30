using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System;

namespace GameEngine.Services
{
    public class Events
    {
        public Action<VirtualKey> OnKeyClick;
        public Action<VirtualKey> OnKeyRelease;
        public Action<double, double> OnMouseClick;
        public Action OneSecPass;
        public Action<int, int> onUpdateScore;
        public Events()
        {

        }
    }
}
