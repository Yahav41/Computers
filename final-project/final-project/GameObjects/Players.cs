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
using Windows.UI;
using Windows.UI.Xaml;

namespace final_project.GameObjects
{
    public abstract class Players : GameMovingObject
    {
        public enum PlayerState { idle, moving, shooting, reloading}
        public PlayerState _playerState { get; set; }

        public double angle { get; set; } =0;
        public bool _isLeft;
        protected GameScene _scene;
        protected DispatcherTimer shootTimerFirst;
        protected float localOffsetX;
        protected float localOffsetY;
        public int _bullets;
        protected int _maxBullets;
        protected bool _canShoot;
        public Players(double x, double y, double size,GameScene scene, bool isLeft, int bullets) : base(string.Empty, x, y, size)
        {
            _playerState = PlayerState.idle;
            Manager.Events.OnKeyClick += Move;
            Manager.Events.OnKeyRelease += Stop;
            _isLeft = isLeft;
            _bullets = _maxBullets = bullets;
            _scene = scene;
            _canShoot = true;

            shootTimerFirst = new DispatcherTimer();
            shootTimerFirst.Tick += ShootTimerFirst_Tick;
            Image.CenterPoint = new System.Numerics.Vector3((float)(Image.Width* 1/ 2), (float)(Image.Height* 1 / 2), 0);
        }
        
        protected virtual void ShootTimerFirst_Tick(object sender, object e)
        {
            if (!_canShoot) return;
            var rect = Rect();
            float centerX = (float)(rect.Left + rect.Width / 2f);
            float centerY = (float)(rect.Top + rect.Height / 2f);

            float cosA = (float)Math.Cos(angle);
            float sinA = (float)Math.Sin(angle);

            float rotatedOffsetX = localOffsetX * cosA - localOffsetY * sinA;
            float rotatedOffsetY = localOffsetX * sinA + localOffsetY * cosA;

            float muzzleX = centerX + rotatedOffsetX;
            float muzzleY = centerY + rotatedOffsetY;
            ShootBullet(muzzleX,muzzleY);
            BulletShot();
        }

        protected virtual void ShootBullet(float muzzleX, float muzzleY)
        {
            
        }

        private void BulletShot()
        {
            _bullets--;
            if (_bullets <= 0)
            {
                _canShoot = false;
            }
            Manager.Events.onBulletShot(_isLeft);
        }

        protected void Stop(VirtualKey key)
        {
            var state = _playerState;
            if ((_isLeft))
            {
                if (key == GameKeys.LeftPlayerReload) return;
                if (key == GameKeys.LeftPlayerShoot)
                {
                    
                    shootTimerFirst.Stop();
                }
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
                if (key == GameKeys.RightPlayerShoot)
                {
                    
                    shootTimerFirst.Stop();
                }
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
            IsObjectCreated = true;
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
            shootTimerFirst.Start();
        }

        protected void Reload()
        {
            _canShoot = true;
            _bullets = _maxBullets;
            var state = _playerState;
            _speedY = _speedX = 0;
            _playerState = PlayerState.reloading;
            if (state != _playerState)
                MatchImageToState();
            Manager.Events.onReload(_isLeft);
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
        public override void Render()
        {
            base.Render();
            angle = Image.Rotation * Math.PI /180;
            if (IsObjectCreated)
            {
                //RectangleHelper.DrawRectangle(_scene, _x, _y, Image.Width, Image.Height, Colors.BlueViolet);
            }
            if(_x < 0)
            {
                _x = 10;
            }
            if(_x > 1150 - Image.Width)
            {
                _x = 1150 - (float)Image.Width - 10;
            }
            if(_y < 0)
            {
                _y = 10;
            }
            if(_y > 475 - Image.Height)
            {
                _y = 475 - (float)Image.Height - 10;
            }
        }
        public override void Collide(GameObject gameObject)
        {
            if (gameObject is Bullets)
            {
                var b = gameObject as Bullets;
                Manager.Events.OnRemoveLifes(_isLeft,b._damage);
               _scene.RemoveObject(b);
            }
        }
    }
}
