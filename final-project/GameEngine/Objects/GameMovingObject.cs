using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace GameEngine.Objects
{
    public abstract class GameMovingObject : GameObject
    {
        public double _speedX;
        public double _speedY;
        protected double _accelerationX;
        protected double _accelerationY;
        protected GameMovingObject(string fileName, double x, double y, double size) : base(fileName, x, y, size)
        {
            Stop();
        }
        public override void Render()
        {
            _x += _speedX;
            _y += _speedY;
            _speedX += _accelerationX;
            _speedY += _accelerationY;
            base.Render();
        }

        public void Stop()
        {
            _speedX = _speedY = _accelerationX = _accelerationY = 0;
        }
    }
}
