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
        protected Image _image;
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
            _image = new Image();
            _image.Width = size;
            Render();
            SetName(_fileName);
            _scene.Children.Add(_image);
        }

        protected void SetName(string fileName)
        {
            _image.Source = new BitmapImage(new Uri($"ms-appx:///Assets/{fileName}"));
        }

        public virtual void Collide(GameObject gameObject)
        {

        }

        public virtual void Render() 
        {
            Canvas.SetLeft(_image,_x);
            Canvas.SetTop(_image,_y);
            Rect();
        }

        public virtual Rect Rect()
        {
            return new Rect(_x,_y, _image.ActualWidth, _image.ActualHeight);
        }

    }
}
