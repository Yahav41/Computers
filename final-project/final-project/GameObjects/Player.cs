using final_project.GameServices;
using GameEngine.Objects;
using GameEngine.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System;
using Windows.UI.Xaml;

namespace final_project.GameObjects
{
    public enum PlayerState
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

        public PlayerState State { get; private set; } = PlayerState.Idle;
        public WeaponProfile Weapon { get; private set; }

        public int BulletsInMagazine { get; private set; }
        public bool CanShoot { get; private set; } = true;

        public double AngleRad { get; private set; }

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
            var bullet = new Bullet(Image.Rotation, muzzleX, muzzleY, 10, _scene, Weapon.Damage);
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

            var keys = _isLeft ? GameKeys.Left : GameKeys.Right;

            if (key == keys.Left) MoveLeft();
            else if (key == keys.Right) MoveRight();
            else if (key == keys.Up) MoveUp();
            else if (key == keys.Down) MoveDown();
            else if (key == keys.Shoot) StartShoot();
            else if (key == keys.Reload) Reload();
        }

        private void OnKeyUp(VirtualKey key)
        {
            var keys = _isLeft ? GameKeys.Left : GameKeys.Right;

            if (key == keys.Shoot)
            {
                _fireTimer.Stop();
            }

            if (key == keys.Left || key == keys.Right)
            {
                SpeedX = 0;
            }

            if (key == keys.Up || key == keys.Down)
            {
                SpeedY = 0;
            }

            if (SpeedX == 0 && SpeedY == 0)
            {
                SetState(PlayerState.Idle);
            }
        }

        private void MoveLeft()
        {
            SpeedX = -GameConstants.PlayerSpeed;
            SetState(PlayerState.Moving);
        }

        private void MoveRight()
        {
            SpeedX = GameConstants.PlayerSpeed;
            SetState(PlayerState.Moving);
        }

        private void MoveUp()
        {
            SpeedY = -GameConstants.PlayerSpeed;
            SetState(PlayerState.Moving);
        }

        private void MoveDown()
        {
            SpeedY = GameConstants.PlayerSpeed;
            SetState(PlayerState.Moving);
        }

        private void StartShoot()
        {
            if (!CanShoot) return;

            SpeedX = SpeedY = 0;
            SetState(PlayerState.Shooting);
            _fireTimer.Start();
        }

        private void Reload()
        {
            CanShoot = true;
            BulletsInMagazine = Weapon.MagazineSize;
            SpeedX = SpeedY = 0;
            SetState(PlayerState.Reloading);
            Manager.Events.onReload?.Invoke(_isLeft);
        }

        private void SetState(PlayerState newState)
        {
            if (State == newState) return;
            State = newState;

            switch (State)
            {
                case PlayerState.Idle:
                    SetSprite(Weapon.IdleSprite);
                    break;
                case PlayerState.Moving:
                    SetSprite(Weapon.WalkSprite);
                    break;
                case PlayerState.Shooting:
                    SetSprite(Weapon.ShootSprite);
                    break;
                case PlayerState.Reloading:
                    SetSprite(Weapon.ReloadSprite);
                    break;
            }
        }

        public override void Render()
        {
            base.Render();

            AngleRad = Image.Rotation * Math.PI / 180.0;

            // Clamping to scene bounds (you already have similar checks)
            if (X < 0) X = 10;
            if (X > 1150 - Image.Width) X = 1150 - (float)Image.Width - 10;
            if (Y < 0) Y = 10;
            if (Y > 475 - Image.Height) Y = 475 - (float)Image.Height - 10;
        }

        public override void OnCollide(GameObject other)
        {
            if (other is Bullet bullet)
            {
                Manager.Events.OnRemoveLifes?.Invoke(_isLeft, bullet.Damage);
                _scene.RemoveObject(bullet);
            }
        }
    }
}
