using final_project.GameServices;
using GameEngine.Objects;
using GameEngine.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System;

namespace final_project.GameObjects
{
    public class Players : GameMovingObject
    {
        public enum PlayerType { rifle, pistol, archer }
        public enum PlayerState { idle, moving, shooting, reloading}

        private PlayerType _playerType { get; set; }
        private PlayerState _playerState { get; set; }
        private bool _isLeft;
        public Players(PlayerType playerType, bool isLeft, double x, double y, double size) : base(string.Empty, x, y, size)
        {
            _playerType = playerType;
            _playerState = PlayerState.idle;
            _isLeft = isLeft;

            if (playerType == PlayerType.rifle)
            {
                SetName("Models/Players/rifle/rifle-idle.gif");
            }
            else if (playerType == PlayerType.pistol)
            {
                SetName("Models/Players/rifle/rifle-idle.gif");
            }
            else
            {
                SetName("Models/Players/archer/.png");
            }
            Image.Rotation= isLeft ? 0 : 180;
            Manager.Events.OnKeyClick += Move;
            Manager.Events.OnKeyRelease += Stop;
        }

        private void Stop(VirtualKey key)
        {
            if (_isLeft)
            {
                if (key == ControlKeys.LeftPlayerReload) return;
                var state = _playerState;
                if (key == ControlKeys.LeftPlayerLeft || key == ControlKeys.LeftPlayerRight)
                {
                    _speedX = 0;
                }
                if (key == ControlKeys.LeftPlayerUp || key == ControlKeys.LeftPlayerDown)
                {
                    _speedY = 0;
                }
                if (_speedX == 0 && _speedY == 0)
                {
                    _playerState = PlayerState.idle;
                    if (state != _playerState)
                    {
                        MatchImageToState();
                    }
                }
            }
        }

        private void MatchImageToState()
        {
            switch (_playerState)
            {
                case PlayerState.idle:  break;
            }
        }



        private void Move(VirtualKey key)
        {
            throw new NotImplementedException();
        }
    }
}
