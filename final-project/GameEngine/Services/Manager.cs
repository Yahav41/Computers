using GameEngine.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace GameEngine.Services
{
    public class Manager
    {
        private Scene _scene;
        private Random _random = new Random();
        private DispatcherTimer _runTimer;


        public static GameEvents Events { get; set; } = new GameEvents();

        public Manager(Scene scene)
        {
            _scene = scene;
            Window.Current.CoreWindow.KeyUp += CoreWindow_KeyUp;
            Window.Current.CoreWindow.KeyDown += CoreWindow_KeyDown;
        }

        private void CoreWindow_KeyDown(Windows.UI.Core.CoreWindow sender, Windows.UI.Core.KeyEventArgs args)
        {
            try
            {
                Events.OnKeyClick(args.VirtualKey);
            }
            catch
            {

            }
        }

        private void CoreWindow_KeyUp(Windows.UI.Core.CoreWindow sender, Windows.UI.Core.KeyEventArgs args)
        {
            try
            {
                Events.OnKeyRelease(args.VirtualKey);
            }
            catch
            {

            }
        }
    }
}
