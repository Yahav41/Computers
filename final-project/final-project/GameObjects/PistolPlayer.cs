using final_project.GameServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace final_project.GameObjects
{
    public class PistolPlayer : Players
    {
        public PistolPlayer(double x, double y, double size, bool isLeft) : base(x, y, size,isLeft)
        {
            SetName("Models/Players/pistol/pistol-idle.gif");
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
