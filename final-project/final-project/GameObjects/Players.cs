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
                if (key == GameKeys.LeftPlayerReload) return;
                var state = _playerState;
                if (key == GameKeys.LeftPlayerLeft || key == GameKeys.LeftPlayerRight)
                {
                    _speedX = 0;
                }
                if (key == GameKeys.LeftPlayerUp || key == GameKeys.LeftPlayerDown)
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

        protected virtual void MatchImageToState()
        {
            
        }



        protected virtual void Move(VirtualKey key)
        {
            
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
