using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace final_project.GameServices
{
    public class WeaponProfile
    {
        public string IdleSprite { get; }
        public string WalkSprite { get; }
        public string ShootSprite { get; }
        public string ReloadSprite { get; }

        public int Damage { get; }
        public int MagazineSize { get; }
        public int FireIntervalMs { get; }

        public WeaponProfile(
            string idle, string walk, string shoot, string reload,
            int damage, int magazineSize, int fireIntervalMs)
        {
            IdleSprite = idle;
            WalkSprite = walk;
            ShootSprite = shoot;
            ReloadSprite = reload;
            Damage = damage;
            MagazineSize = magazineSize;
            FireIntervalMs = fireIntervalMs;
        }

        public static WeaponProfile Pistol => new WeaponProfile(
            "Models/Players/pistol/pistol-idle.gif",
            "Models/Players/pistol/pistol-walk.gif",
            "Models/Players/pistol/pistol-shoot.gif",
            "Models/Players/pistol/pistol-reload.gif",
            damage: 10,
            magazineSize: 6,
            fireIntervalMs: 500);

        public static WeaponProfile Rifle => new WeaponProfile(
            "Models/Players/rifle/rifle-idle.gif",
            "Models/Players/rifle/rifle-walk.gif",
            "Models/Players/rifle/rifle-shoot.gif",
            "Models/Players/rifle/rifle-reload.gif",
            damage: 5,
            magazineSize: 30,
            fireIntervalMs: 200);

        public static WeaponProfile Shotgun => new WeaponProfile(
            "Models/Players/shotgun/shotgun-idle.gif",
            "Models/Players/shotgun/shotgun-walk.gif",
            "Models/Players/shotgun/shotgun-shoot.gif",
            "Models/Players/shotgun/shotgun-reload.gif",
            damage: 50,
            magazineSize: 1,
            fireIntervalMs: 100);
    }
}
