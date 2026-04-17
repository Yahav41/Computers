using final_project.GameServices;
using GameEngine.Objects;
using GameEngine.Services;
using System;
using Windows.System;
using Windows.UI.Xaml;

namespace final_project.GameObjects
{
    public enum PlayerAnimationState
    {
        Idle,
        Moving,
        Shooting,
        Reloading
    }

    public class Player : GameMovingObject
    {
        private readonly bool _isLeft;
        private readonly GameScene _scene;
        private readonly DispatcherTimer _fireTimer = new DispatcherTimer();

        private float _localOffsetX = 40f;
        private float _localOffsetY = 15f;

        public PlayerAnimationState State { get; private set; } = PlayerAnimationState.Idle;
        public WeaponProfile Weapon { get; private set; }

        public int BulletsInMagazine { get; private set; }
        public bool CanShoot { get; private set; } = true;

        public double AngleRad { get; private set; }

        public bool IsLeft => _isLeft;

        public Player(
            double x,
            double y,
            double size,
            GameScene scene,
            bool isLeft,
            WeaponProfile weapon)
            : base(string.Empty, x, y, size)
        {
            _scene = scene;
            _isLeft = isLeft;
            Weapon = weapon;

            BulletsInMagazine = weapon.MagazineSize;
            SetSprite(weapon.IdleSprite);

            _fireTimer.Interval = TimeSpan.FromMilliseconds(weapon.FireIntervalMs);
            _fireTimer.Tick += FireTimer_Tick;

            Image.CenterPoint = new System.Numerics.Vector3(
                (float)(Image.Width / 2.0),
                (float)(Image.Height / 2.0),
                0);

            Manager.Events.OnKeyClick += OnKeyDown;
            Manager.Events.OnKeyRelease += OnKeyUp;
        }

        public int WeaponTypeIndex =>
            Weapon == WeaponProfile.Pistol ? 0 :
            Weapon == WeaponProfile.Rifle ? 1 : 2;

        private void FireTimer_Tick(object sender, object e)
        {
            if (!CanShoot) return;

            var rect = Bounds();
            float centerX = (float)(rect.Left + rect.Width / 2.0);
            float centerY = (float)(rect.Top + rect.Height / 2.0);

            float cosA = (float)Math.Cos(AngleRad);
            float sinA = (float)Math.Sin(AngleRad);

            float rotatedOffsetX = _localOffsetX * cosA - _localOffsetY * sinA;
            float rotatedOffsetY = _localOffsetX * sinA + _localOffsetY * cosA;

            float muzzleX = centerX + rotatedOffsetX;
            float muzzleY = centerY + rotatedOffsetY;

            SpawnBullet(muzzleX, muzzleY);
            OnBulletShot();
        }

        private void SpawnBullet(float muzzleX, float muzzleY)
        {
            var bullet = new Bullets(Image.Rotation, muzzleX, muzzleY, 10, _scene, Weapon.Damage);
            _scene.AddObject(bullet);
        }

        private void OnBulletShot()
        {
            BulletsInMagazine--;
            if (BulletsInMagazine <= 0)
            {
                CanShoot = false;
            }

            Manager.Events.onBulletShot?.Invoke(_isLeft);
        }

        private void OnKeyDown(VirtualKey key)
        {
            IsCreated = true;

            // For now use existing GameKeys static mapping
            bool isLeft = _isLeft;
            if (isLeft)
            {
                if (key == GameKeys.LeftPlayerLeft) MoveLeft();
                else if (key == GameKeys.LeftPlayerRight) MoveRight();
                else if (key == GameKeys.LeftPlayerUp) MoveUp();
                else if (key == GameKeys.LeftPlayerDown) MoveDown();
                else if (key == GameKeys.LeftPlayerShoot) StartShoot();
                else if (key == GameKeys.LeftPlayerReload) Reload();
            }
            else
            {
                if (key == GameKeys.RightPlayerLeft) MoveLeft();
                else if (key == GameKeys.RightPlayerRight) MoveRight();
                else if (key == GameKeys.RightPlayerUp) MoveUp();
                else if (key == GameKeys.RightPlayerDown) MoveDown();
                else if (key == GameKeys.RightPlayerShoot) StartShoot();
                else if (key == GameKeys.RightPlayerReload) Reload();
            }
        }

        private void OnKeyUp(VirtualKey key)
        {
            bool isLeft = _isLeft;

            if (isLeft)
            {
                if (key == GameKeys.LeftPlayerShoot)
                {
                    _fireTimer.Stop();
                }

                if (key == GameKeys.LeftPlayerLeft || key == GameKeys.LeftPlayerRight)
                {
                    SpeedX = 0;
                }

                if (key == GameKeys.LeftPlayerUp || key == GameKeys.LeftPlayerDown)
                {
                    SpeedY = 0;
                }
            }
            else
            {
                if (key == GameKeys.RightPlayerShoot)
                {
                    _fireTimer.Stop();
                }

                if (key == GameKeys.RightPlayerLeft || key == GameKeys.RightPlayerRight)
                {
                    SpeedX = 0;
                }

                if (key == GameKeys.RightPlayerUp || key == GameKeys.RightPlayerDown)
                {
                    SpeedY = 0;
                }
            }

            if (SpeedX == 0 && SpeedY == 0)
            {
                SetState(PlayerAnimationState.Idle);
            }
        }

        private void MoveLeft()
        {
            SpeedX = -GameConstants.playerSpeed;
            SetState(PlayerAnimationState.Moving);
        }

        private void MoveRight()
        {
            SpeedX = GameConstants.playerSpeed;
            SetState(PlayerAnimationState.Moving);
        }

        private void MoveUp()
        {
            SpeedY = -GameConstants.playerSpeed;
            SetState(PlayerAnimationState.Moving);
        }

        private void MoveDown()
        {
            SpeedY = GameConstants.playerSpeed;
            SetState(PlayerAnimationState.Moving);
        }

        private void StartShoot()
        {
            if (!CanShoot) return;

            SpeedX = SpeedY = 0;
            SetState(PlayerAnimationState.Shooting);
            _fireTimer.Start();
        }

        private void Reload()
        {
            CanShoot = true;
            BulletsInMagazine = Weapon.MagazineSize;
            SpeedX = SpeedY = 0;
            SetState(PlayerAnimationState.Reloading);
            Manager.Events.onReload?.Invoke(_isLeft);
        }

        private void SetState(PlayerAnimationState newState)
        {
            if (State == newState) return;
            State = newState;

            switch (State)
            {
                case PlayerAnimationState.Idle:
                    SetSprite(Weapon.IdleSprite);
                    break;
                case PlayerAnimationState.Moving:
                    SetSprite(Weapon.WalkSprite);
                    break;
                case PlayerAnimationState.Shooting:
                    SetSprite(Weapon.ShootSprite);
                    break;
                case PlayerAnimationState.Reloading:
                    SetSprite(Weapon.ReloadSprite);
                    break;
            }
        }

        public override void Render()
        {
            base.Render();

            AngleRad = Image.Rotation * Math.PI / 180.0;

            // Clamp to arena bounds
            if (X < 0) X = 10;
            if (X > 1150 - Image.Width) X = 1150 - Image.Width - 10;
            if (Y < 0) Y = 10;
            if (Y > 475 - Image.Height) Y = 475 - Image.Height - 10;
        }

        public override void OnCollide(GameObject other)
        {
            if (other is Bullets bullet)
            {
                Manager.Events.OnRemoveLifes?.Invoke(_isLeft, bullet.Damage);
                _scene.RemoveObject(bullet);
            }
        }
    }
}