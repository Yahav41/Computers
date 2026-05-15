namespace final_project.GameServices
{
    // טיפוס נשק פשוט המשמש כאינדקס לסוגי נשק שונים
    public enum WeaponType
    {
        Pistol = 0,
        Rifle = 1,
        Shotgun = 2
    }

    // פרופיל נשק שמאגד את כל ההגדרות והמשאבים של נשק יחיד
    public class WeaponProfile
    {
        // שמות קבצי ה-sprite עבור מצבי אנימציה שונים
        public string IdleSprite { get; }
        public string WalkSprite { get; }
        public string ShootSprite { get; }
        public string ReloadSprite { get; }

        // תכונות ביצועים של הנשק
        public int Damage { get; }               // נזק לכדור בודד
        public int MagazineSize { get; }         // מספר כדורים במגזין
        public int FireIntervalMs { get; }       // מרווח בין יריות במילישניות
        public int ReloadDurationMs { get; }     // משך טעינה מחדש במילישניות

        // סוג הנשק כמ_ENUM כדי לאפשר זיהוי קל וקומפקטי
        public WeaponType Type { get; }  

        // קונסטרקטור פרמטרי שמקבל את כל ההגדרות ומאחסן אותן
        public WeaponProfile(
            string idle, string walk, string shoot, string reload,
            int damage, int magazineSize, int fireIntervalMs,
            int reloadDurationMs,
            WeaponType type)               
        {
            IdleSprite = idle;
            WalkSprite = walk;
            ShootSprite = shoot;
            ReloadSprite = reload;
            Damage = damage;
            MagazineSize = magazineSize;
            FireIntervalMs = fireIntervalMs;
            ReloadDurationMs = reloadDurationMs;
            Type = type;
        }

        // פרופילים סטטיים מוכנים מראש לשימוש נוח בקוד:
        // כל פרופיל מייצג מופע חדש עם נתוני ברירת מחדל מתאימים
        public static WeaponProfile Pistol => new WeaponProfile(
            "Models/Players/pistol/pistol-idle.gif",
            "Models/Players/pistol/pistol-walk.gif",
            "Models/Players/pistol/pistol-shoot.gif",
            "Models/Players/pistol/pistol-reload.gif",
            damage: 10,
            magazineSize: 6,
            fireIntervalMs: 500,
            reloadDurationMs: 1200,
            type: WeaponType.Pistol
        );

        public static WeaponProfile Rifle => new WeaponProfile(
            "Models/Players/rifle/rifle-idle.gif",
            "Models/Players/rifle/rifle-walk.gif",
            "Models/Players/rifle/rifle-shoot.gif",
            "Models/Players/rifle/rifle-reload.gif",
            damage: 5,
            magazineSize: 30,
            fireIntervalMs: 200,
            reloadDurationMs: 1750,
            type: WeaponType.Rifle
        );

        public static WeaponProfile Shotgun => new WeaponProfile(
            "Models/Players/shotgun/shotgun-idle.gif",
            "Models/Players/shotgun/shotgun-walk.gif",
            "Models/Players/shotgun/shotgun-shoot.gif",
            "Models/Players/shotgun/shotgun-reload.gif",
            damage: 50,
            magazineSize: 1,
            fireIntervalMs: 100,
            reloadDurationMs: 2200,
            type: WeaponType.Shotgun
        );
    }
}