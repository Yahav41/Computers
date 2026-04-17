// GameEngine/Services/Scene.cs
using GameEngine.Objects;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace GameEngine.Services
{
    public abstract class Scene : Canvas
    {
        private readonly List<GameObject> _gameObjects = new List<GameObject>();

        protected IEnumerable<GameObject> GameObjectsSnapshot => _gameObjects.ToList();

        protected Scene()
        {
            Manager.Events.OnRun = null;
            Manager.Events.OnRun += OnTick;
            Manager.Events.OnRun += CheckCollisions;
        }

        public void AddObject(GameObject obj)
        {
            _gameObjects.Add(obj);
            Children.Add(obj.Image);
        }

        public void RemoveObject(GameObject obj)
        {
            if (!_gameObjects.Remove(obj)) return;
            Children.Remove(obj.Image);
        }

        public void RemoveAll()
        {
            foreach (var obj in _gameObjects.ToList())
            {
                RemoveObject(obj);
            }
        }

        public void ResetAll()
        {
            foreach (var obj in _gameObjects)
            {
                obj.Reset();
            }
        }

        private void OnTick()
        {
            foreach (var obj in GameObjectsSnapshot)
            {
                if (obj is GameMovingObject moving)
                {
                    moving.Render();
                }
            }
        }

        private void CheckCollisions()
        {
            foreach (var obj in GameObjectsSnapshot)
            {
                if (!obj.Collisional) continue;

                var other = GameObjectsSnapshot.FirstOrDefault(o =>
                    !ReferenceEquals(o, obj)
                    && o.Collisional
                    && !RectHelper.Intersect(o.Bounds(), obj.Bounds()).IsEmpty);

                if (other != null)
                {
                    obj.OnCollide(other);
                }
            }
        }
    }
}