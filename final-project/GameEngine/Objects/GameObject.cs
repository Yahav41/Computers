
using GameEngine.Services;
using System;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace GameEngine.Objects
{
    public abstract class GameObject
    {
        public double X { get; set; }
        public double Y { get; set; }

        private readonly double _initialX;
        private readonly double _initialY;

        public Image Image { get; }
        public bool Collisional { get; set; } = true;
        public bool IsCreated { get; protected set; }

        protected GameObject(string assetPath, double x, double y, double width)
        {
            _initialX = x;
            _initialY = y;

            X = x;
            Y = y;

            Image = new Image
            {
                Width = width,
                Height = width * 0.738
            };

            if (!string.IsNullOrEmpty(assetPath))
            {
                SetSprite(assetPath);
            }

            Render();
        }

        public void Reset()
        {
            X = _initialX;
            Y = _initialY;
        }

        protected void SetSprite(string relativeAssetPath)
        {
            var uri = new Uri($"ms-appx:///Assets/{relativeAssetPath}");
            Image.Source = new BitmapImage(uri);
        }

        public virtual void Render()
        {
            Scene.SetLeft(Image, X);
            Scene.SetTop(Image, Y);
        }

        public virtual Rect Bounds()
        {
            return new Rect(X, Y, Image.ActualWidth, Image.ActualHeight);
        }

        public virtual void OnCollide(GameObject other)
        {
            
        }
    }
}