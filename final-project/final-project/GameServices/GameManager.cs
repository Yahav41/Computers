using final_project.GameObjects;
using final_project.Objects;
using GameEngine.Services;

namespace final_project.GameServices
{
    public class GameManager : Manager
    {
        private readonly GameScene _scene;
        private readonly bool _isServer;

        public GameScene Scene => _scene;

        public GameManager(GameScene scene, bool isServer)
            : base(scene)
        {
            _scene = scene;
            _isServer = isServer;
            CreateObjects();
        }

        private void CreateObjects()
        {
            if (_isServer)
            {
                for (int i = 0; i < 15; i++)
                {
                    _scene.AddObject(new Covers(
                        (Covers.CoverType)_random.Next(0, 4),
                        _random.Next(100, 801),
                        _random.Next(401),
                        100,
                        _scene));
                }
            }

            // Left player weapon
            WeaponProfile leftWeapon;
            switch (GameConstants.leftPlayer)
            {
                case 0:
                    leftWeapon = WeaponProfile.Pistol;
                    break;
                case 1:
                    leftWeapon = WeaponProfile.Rifle;
                    break;
                case 2:
                    leftWeapon = WeaponProfile.Shotgun;
                    break;
                default:
                    leftWeapon = WeaponProfile.Pistol;
                    break;
            }

            _scene.AddObject(new Player(100, 200, 80, _scene, true, leftWeapon));

            // Right player weapon
            WeaponProfile rightWeapon;
            switch (GameConstants.rightPlayer)
            {
                case 0:
                    rightWeapon = WeaponProfile.Pistol;
                    break;
                case 1:
                    rightWeapon = WeaponProfile.Rifle;
                    break;
                case 2:
                    rightWeapon = WeaponProfile.Shotgun;
                    break;
                default:
                    rightWeapon = WeaponProfile.Pistol;
                    break;
            }

            _scene.AddObject(new Player(800, 200, 80, _scene, false, rightWeapon));
        }

        public int GetBullets(bool isLeft)
        {
            var player = _scene.GetPlayer(isLeft);
            return player?.BulletsInMagazine ?? 0;
        }
    }
}