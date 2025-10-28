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
        protected double _x;
        protected double _y;
        private double _placeX;
        private double _placeY;
        public Image Image { get; set;}
        protected string _fileName;
        protected Canvas _scene;
        public bool Collisional { get; set; } = true;
        public bool IsObjectCreated { get; set; } = false;

        protected GameObject(Canvas scene,string fileName, double x, double y, double size)
        {
            _scene = scene;
            _fileName = fileName;
            _x = x;
            _y = y;
            _placeX = x;
            _placeY = y;
            Image = new Image();
            Image.Width = size;
            Render();
            SetName(_fileName);
            _scene.Children.Add(Image);
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
            Canvas.SetLeft(Image,_x);
            Canvas.SetTop(Image,_y);
            Rect();
        }

        public virtual Rect Rect()
        {
            return new Rect(_x,_y, Image.ActualWidth, Image.ActualHeight);
        }

    }
}
