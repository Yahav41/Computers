using final_project.GameServices;
using Windows.Foundation;

namespace final_project.GameObjects
{
    public class PistolPlayer : Players
    {
        public PistolPlayer(double x, double y,GameScene scene ,bool isLeft) : base(x, y, 80,scene,isLeft,6)
        {
            SetName("Models/Players/pistol/pistol-idle.gif");
            shootTimerFirst.Interval = System.TimeSpan.FromMilliseconds(500);
            localOffsetX = 40f;
            localOffsetY = 15f;
        }
        protected override void ShootBullet(float muzzleX, float muzzleY)
        {
            _scene.AddObject(new Bullets(Image.Rotation, muzzleX, muzzleY, 10, _scene,10));
        }
        protected override void MatchImageToState()
        {
            switch (_playerState)
            {
                case PlayerState.idle:
                    SetName("Models/Players/pistol/pistol-idle.gif");
                    break;
                case PlayerState.moving:
                    SetName("Models/Players/pistol/pistol-walk.gif");
                    break;
                case PlayerState.shooting:
                    SetName("Models/Players/pistol/pistol-shoot.gif");
                    break;
                case PlayerState.reloading:
                    SetName("Models/Players/pistol/pistol-reload.gif");
                    break;
                default:
                    break;
            }
        }
    }
}
