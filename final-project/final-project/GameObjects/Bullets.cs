using final_project.GameServices;
using final_project.Objects;
using GameEngine.Objects;
using System;
using System.Threading.Tasks;

namespace final_project.GameObjects
{
    public class Bullets : GameMovingObject
    {
        private readonly GameScene _scene;

        public int Damage { get; }

        public Bullets(float angle, double x, double y, double size, GameScene scene, int damage)
            : base(string.Empty, x, y, size)
        {
            Collisional = false;
            _scene = scene;

            SetSprite("Models/Bullet/bullet.png");

            Image.CenterPoint = new System.Numerics.Vector3(
                (float)(Image.Width / 2.0),
                (float)(Image.Height / 2.0),
                0);

            Image.Rotation = angle + 90;

            SetSpeed(angle);
            _ = ActivateCollisionAsync();

            Damage = damage;
        }

        private async Task ActivateCollisionAsync()
        {
            await Task.Delay(100);
            Collisional = true;
        }

        private void SetSpeed(double angle)
        {
            var radians = angle * (Math.PI / 180.0);
            SpeedX = Math.Cos(radians) * GameConstants.bulletSpeed;
            SpeedY = Math.Sin(radians) * GameConstants.bulletSpeed;
            IsCreated = true;
        }

        public override void Render()
        {
            base.Render();

            if (X < 0 || X > 1150 || Y < 0 || Y > 475)
            {
                _scene.RemoveObject(this);
            }
        }

        public override void OnCollide(GameObject g)
        {
            if (g is Covers)
            {
                _scene.RemoveObject(this);
            }
        }
    }
}