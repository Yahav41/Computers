using final_project.GameServices;
using GameEngine.Objects;
using GameEngine.Services;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System;

namespace final_project.GameObjects
{
    public abstract class Players : GameMovingObject
    {
        public enum PlayerState { idle, moving, shooting, reloading}
        protected PlayerState _playerState { get; set; }

        public double angle { get; set; } =0;
        public bool _isLeft { get; set; }
        public Players(double x, double y, double size, bool isLeft) : base(string.Empty, x, y, size)
        {
            _playerState = PlayerState.idle;
            Manager.Events.OnKeyClick += Move;
            Manager.Events.OnKeyRelease += Stop;
            _isLeft = isLeft;
        }

        protected void Stop(VirtualKey key)
        {
            var state = _playerState;
            if ((_isLeft))
            {
                if (key == GameKeys.LeftPlayerReload) return;
                if (key == GameKeys.LeftPlayerLeft || key == GameKeys.LeftPlayerRight)
                {
                    _speedX = 0;
                }
                if (key == GameKeys.LeftPlayerUp || key == GameKeys.LeftPlayerDown)
                {
                    _speedY = 0;
                }
            }
            else
            {
                if (key == GameKeys.RightPlayerReload) return;
                if (key == GameKeys.RightPlayerLeft || key == GameKeys.RightPlayerRight)
                {
                    _speedX = 0;
                }
                if (key == GameKeys.RightPlayerUp || key == GameKeys.RightPlayerDown)
                {
                    _speedY = 0;
                }
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

        protected virtual void MatchImageToState()
        {
            
        }



        protected void Move(VirtualKey key)
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
        protected void Shoot()
        {
            var state = _playerState;
            _speedX = _speedY = 0;
            _playerState = PlayerState.shooting;
            if (state != _playerState)
                MatchImageToState();
        }

        protected void Reload()
        {
            var state = _playerState;
            _speedY = _speedX = 0;
            _playerState = PlayerState.reloading;
            if (state != _playerState)
                MatchImageToState();
        }

        protected void RunLeft()
        {
            var state = _playerState;
            _speedX = -GameConstants.playerSpeed;
            _playerState = PlayerState.moving;
            if (state != _playerState)
                MatchImageToState();
        }
        protected void RunRight()
        {
            var state = _playerState;
            _speedX = GameConstants.playerSpeed;
            _playerState = PlayerState.moving;
            if (state != _playerState)
                MatchImageToState();
        }
        protected void RunUp()
        {
            var state = _playerState;
            _speedY = -GameConstants.playerSpeed;
            _playerState = PlayerState.moving;
            if (state != _playerState)
                MatchImageToState();
        }
        protected void RunDown()
        {
            var state = _playerState;
            _speedY = GameConstants.playerSpeed;
            _playerState = PlayerState.moving;
            if (state != _playerState)
                MatchImageToState();
        }

    }
}
