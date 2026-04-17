using System;
using Windows.UI.Xaml.Controls;

namespace GameEngine.Objects
{
    public abstract class GameMovingObject : GameObject
    {
        public double SpeedX { get; set; }
        public double SpeedY { get; set; }

        protected double AccelerationX { get; set; }
        protected double AccelerationY { get; set; }

        protected GameMovingObject(string fileName, double x, double y, double size)
            : base(fileName, x, y, size)
        {
            Stop();
        }

        public override void Render()
        {
            X += SpeedX;
            Y += SpeedY;

            SpeedX += AccelerationX;
            SpeedY += AccelerationY;

            base.Render();
        }

        public void Stop()
        {
            SpeedX = 0;
            SpeedY = 0;
            AccelerationX = 0;
            AccelerationY = 0;
        }
    }
}