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
        protected Random _random = new Random();
        private DispatcherTimer _runTimer;
        protected GameEvents _gameEvents { get; set;}


        public static GameEvents Events { get; set; } = new GameEvents();

        public Manager(Scene scene)
        {
            _scene = scene;
            Window.Current.CoreWindow.KeyUp += CoreWindow_KeyUp;
            Window.Current.CoreWindow.KeyDown += CoreWindow_KeyDown;

            _runTimer = new DispatcherTimer();
            _runTimer.Interval += TimeSpan.FromMilliseconds(0.01);
            _runTimer.Start();
            _runTimer.Tick += _runTimer_Tick;
        }

        protected virtual void _runTimer_Tick(object sender, object e)
        {
            Events.OnRun();
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
