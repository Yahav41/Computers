using final_project.GameObjects;
using final_project.Objects;
using GameEngine.Services;
using System;

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
                        (Covers.CoverType)random.Next(0, 4),
                        random.Next(100, 801),
                        random.Next(401),
                        100,
                        _scene));
                }
            }

            // Left player
            var leftWeapon = GameConstants.leftPlayer switch
            {
                0 => WeaponProfile.Pistol,
                1 => WeaponProfile.Rifle,
                2 => WeaponProfile.Shotgun,
                _ => WeaponProfile.Pistol
            };

            _scene.AddObject(new Player(100, 200, 80, _scene, true, leftWeapon));

            // Right player
            var rightWeapon = GameConstants.rightPlayer switch
            {
                0 => WeaponProfile.Pistol,
                1 => WeaponProfile.Rifle,
                2 => WeaponProfile.Shotgun,
                _ => WeaponProfile.Pistol
            };

            _scene.AddObject(new Player(800, 200, 80, _scene, false, rightWeapon));
        }

        public int GetBullets(bool isLeft)
        {
            var player = _scene.GetPlayer(isLeft);
            return player?.BulletsInMagazine ?? 0;
        }
    }
}