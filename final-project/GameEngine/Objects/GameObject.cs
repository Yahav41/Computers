using GameEngine.Services;
using System;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace GameEngine.Objects
{
    public abstract class GameObject
    {
        public double _x;
        public double _y;
        private double _placeX;
        private double _placeY;
        public Image Image { get; set;}
        protected string _fileName;
        public bool Collisional { get; set; } = true;
        public bool IsObjectCreated { get; set; } = false;

        protected GameObject(string fileName, double x, double y, double size)
        {
            _fileName = fileName;
            _x = x;
            _y = y;
            _placeX = x;
            _placeY = y;
            Image = new Image();
            Image.Width = size;
            Image.Height = size*0.738;
            Render();
            SetName(_fileName);
        }

        public void Init()
        {
            _x = _placeX;
            _y = _placeY;
        }

        protected void SetName(string fileName)
        {
            Image.Source = new BitmapImage(new Uri($"ms-appx:///Assets/{fileName}"));
        }

        public virtual void Collide(GameObject gameObject)
        {

        }

        public virtual void Render() 
        {
            Scene.SetLeft(Image,_x);
            Scene.SetTop(Image,_y);
            Rect();
        }

        public virtual Rect Rect()
        {
            return new Rect(_x,_y, Image.ActualWidth, Image.ActualHeight);
        }

    }
}
