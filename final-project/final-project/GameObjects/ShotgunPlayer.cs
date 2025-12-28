using final_project.GameServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace final_project.GameObjects
{
    public class ShotgunPlayer : Players
    {
        public ShotgunPlayer(double x, double y, GameScene scene, bool isLeft) : base(x, y, 100, scene, isLeft, 1)
        {
            SetName("Models/Players/shotgun/shotgun-idle.gif");
            shootTimerFirst.Interval = System.TimeSpan.FromMilliseconds(100);
            localOffsetX = 40f;
            localOffsetY = 15f;
        }
        protected override void ShootBullet(float muzzleX, float muzzleY)
        {
            _scene.AddObject(new Bullets(Image.Rotation, muzzleX, muzzleY, 10, _scene, 50));
        }
        protected override void MatchImageToState()
        {
            switch (_playerState)
            {
                case PlayerState.idle:
                    SetName("Models/Players/shotgun/shotgun-idle.gif");
                    break;
                case PlayerState.moving:
                    SetName("Models/Players/shotgun/shotgun-walk.gif");
                    break;
                case PlayerState.shooting:
                    SetName("Models/Players/shotgun/shotgun-shoot.gif");
                    break;
                case PlayerState.reloading:
                    SetName("Models/Players/shotgun/shotgun-reload.gif");
                    break;
                default:
                    break;
            }
        }
    }
}
