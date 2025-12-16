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

        protected override void Move(Windows.System.VirtualKey key)
        {
            if (_isLeft)
            {
                if (key == GameKeys.LeftPlayerLeft)
                {
                    RunLeft();
                }
                else if (key == GameKeys.LeftPlayerRight)
                {
                    RunRight();
                }
                else if (key == GameKeys.LeftPlayerUp)
                {
                    RunUp();
                }
                else if (key == GameKeys.LeftPlayerDown)
                {
                    RunDown();
                }
                else if (key == GameKeys.LeftPlayerShoot)
                {
                    Shoot();
                }
                else if (key == GameKeys.LeftPlayerReload)
                {
                    Reload();
                }
            }
            else
            {
                if (key == GameKeys.RightPlayerLeft)
                {
                    RunLeft();
                }
                else if (key == GameKeys.RightPlayerRight)
                {
                    RunRight();
                }
                else if (key == GameKeys.RightPlayerUp)
                {
                    RunUp();
                }
                else if (key == GameKeys.RightPlayerDown)
                {
                    RunDown();
                }
                else if (key == GameKeys.RightPlayerShoot)
                {
                    Shoot();
                }
                else if (key == GameKeys.RightPlayerReload)
                {
                    Reload();
                }
            }
        }
    }
}
