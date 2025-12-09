using final_project.GameServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace final_project.GameObjects
{
    public class RiflePlayer : Players
    {
        public RiflePlayer(double x, double y, GameScene scene, bool isLeft) : base(x, y, 120, scene, isLeft)
        {
            SetName("Models/Players/rifle/rifle-idle.gif");
            shootTimerFirst.Interval = System.TimeSpan.FromMilliseconds(300);
            localOffsetX = 40f;
            localOffsetY = 15f;
        }
        protected override void MatchImageToState()
        {
            switch (_playerState)
            {
                case PlayerState.idle:
                    SetName("Models/Players/rifle/rifle-idle.gif");
                    break;
                case PlayerState.moving:
                    SetName("Models/Players/rifle/rifle-walk.gif");
                    break;
                case PlayerState.shooting:
                    SetName("Models/Players/rifle/rifle-shoot.gif");
                    break;
                case PlayerState.reloading:
                    SetName("Models/Players/rifle/rifle-reload.gif");
                    break;
                default:
                    break;
            }
        }
    }
}
