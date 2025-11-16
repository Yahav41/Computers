using GameEngine.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace final_project.GameObjects
{
    public class Bullets : GameMovingObject
    {
        public Bullets(float angle,double x, double y, double size) : base(String.Empty, x, y, size)
        {
            SetName("Models/Bullet/bullet.png");
            Image.Rotation = angle;
            SetSpeed(angle);
        }

        private void SetSpeed(float angle)
        {
            _speedX = Math.Cos(angle);
            _speedY = Math.Sin(angle);
        }
    }
}
