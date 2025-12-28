using final_project.GameServices;
using final_project.Objects;
using GameEngine.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;

namespace final_project.GameObjects
{
    public class Bullets : GameMovingObject
    {
        GameScene _scene;
        public int _damage;
        public Bullets(float angle,double x, double y, double size, GameScene scene, int damage) : base(String.Empty, x, y, size)
        {
            Collisional = false;
            _scene = scene;
            SetName("Models/Bullet/bullet.png");
            Image.CenterPoint = new System.Numerics.Vector3((float)(Image.Width * 1 / 2), (float)(Image.Height * 1 / 2), 0);
            Image.Rotation = angle + 90;
            SetSpeed(angle);
            ActivateCollision();
            _damage = damage;
        }

        private async Task ActivateCollision()
        {
            await Task.Delay(100);
            Collisional = true;
        }

        private void SetSpeed(double angle)
        {
            _speedX = Math.Cos(angle * (Math.PI / 180)) * GameConstants.bulletSpeed;
            _speedY = Math.Sin(angle * (Math.PI / 180)) * GameConstants.bulletSpeed;
            IsObjectCreated = true;
        }
        
        public override void Render()
        {
            base.Render();
            if(_x < 0 || _x > 1150 || _y < 0 || _y > 475)
            {
                _scene.RemoveObject(this);
            }
            if (IsObjectCreated)
            {
                //RectangleHelper.DrawRectangle(_scene, _x, _y, Image.Width, Image.Height, Colors.White);
            }
        }

        public override void Collide(GameObject g)
        {
            if(g is Covers)
            {
                _scene.RemoveObject(this);
            }
        }
    }
}
