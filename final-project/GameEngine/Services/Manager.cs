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
        private Canvas _scene;
        private List<GameObject> _objects;
        private Random _random = new Random();
        private DispatcherTimer _runTimer;

        public static Events Events { get; set; } = new Events();

        public Manager(Canvas scene)
        {
            _scene = scene;
            _runTimer = new DispatcherTimer();
            _runTimer.Interval = TimeSpan.FromMilliseconds(0.01);
            _runTimer.Tick += _runTimer_Tick;
            _runTimer.Start();
            _objects = new List<GameObject>();
            AddCovers();
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

        private void _runTimer_Tick(object sender, object e)
        {
            Run();
            CheckCollisional();
        }

        private void AddCovers()
        {
            for (int i = 1; i <= 15; i++)
            {
                _objects.Add(new Covers(_scene, (Covers.CoverType)_random.Next(0, 4), _random.Next(100, 801), _random.Next(401), 100));
            }
        }

        private void Run()
        {

            foreach (GameObject gameObject in _objects)
            {
                if (gameObject is GameMovingObject obj)
                {
                    obj.Render();
                }
            }
        }
        private void CheckCollisional()
        {
            for (int i = 0; i < _objects.Count; i++)
            {
                for (int j = 0; j < _objects.Count; j++)
                {
                    if (i != j && _objects[i].Collisional && _objects[j].Collisional && !RectHelper.Intersect(_objects[i].Rect(), _objects[j].Rect()).IsEmpty)
                    {
                        _objects[i].Collide(_objects[j]);
                    }
                }
            }
        }
    }
}
