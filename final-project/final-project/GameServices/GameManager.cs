using final_project.GameObjects;
using final_project.Objects;
using GameEngine.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace final_project.GameServices
{
    // מנהל המשחק המותאם לפרויקט — יורש מ-'Manager' ומוסיף יצירת עצמים ולוגיקה ספציפית
    public class GameManager : Manager
    {
        private readonly GameScene _scene; // הסצנה הנוכחית שהמנהל שולט עליה
        private readonly bool _isServer; // דגל שמציין האם המנהל פועל בצד השרת

        public GameScene Scene => _scene; // גטר לגישה לסצנה מבחוץ

        // בנאי שמקבל סצנה ודגל שרת, מעביר את הסצנה לבסיס ויוצר עצמים התחלתיים
        public GameManager(GameScene scene, bool isServer)
            : base(scene)
        {
            _scene = scene;
            _isServer = isServer;
            CreateObjects(); // יצירת עצמים (שחקנים, מחסות וכו') בהתאם לתפקיד (שרת/לקוח)
        }

        // יוצר עצמים התחלתיים במשחק
        private void CreateObjects()
        {
            if (_isServer)
            {
                // אם אנו השרת - ניצור מחסות אקראיים במפה
                for (int i = 0; i < 15; i++)
                {
                    _scene.AddObject(new Covers(
                        (Covers.CoverType)_random.Next(0, 4), // סוג מחסה אקראי
                        _random.Next(100, 801), // X אקראי בטווח
                        _random.Next(401), // Y אקראי בטווח
                        100, // גודל
                        _scene));
                }
            }

            // קובעים מי נשלט מקומית: אם שרת — השחקן השמאלי מקומי, הלקוחו שחקן הימני מקומי
            bool leftLocal = _isServer;
            bool rightLocal = !_isServer;

            // בוחרים נשק לשחקן השמאלי לפי הגדרה ב-GameConstants
            WeaponProfile leftWeapon;
            switch (GameConstants.leftPlayer)
            {
                case 0: leftWeapon = WeaponProfile.Pistol; break;
                case 1: leftWeapon = WeaponProfile.Rifle; break;
                case 2: leftWeapon = WeaponProfile.Shotgun; break;
                default: leftWeapon = WeaponProfile.Pistol; break;
            }

            // מוסיפים את השחקן השמאלי לסצנה
            _scene.AddObject(new Player(100, 200, 80, _scene, true, leftWeapon, leftLocal));

            // בוחרים נשק לשחקן הימני לפי הגדרה ב-GameConstants
            WeaponProfile rightWeapon;
            switch (GameConstants.rightPlayer)
            {
                case 0: rightWeapon = WeaponProfile.Pistol; break;
                case 1: rightWeapon = WeaponProfile.Rifle; break;
                case 2: rightWeapon = WeaponProfile.Shotgun; break;
                default: rightWeapon = WeaponProfile.Pistol; break;
            }

            // מוסיפים את השחקן הימני לסצנה
            _scene.AddObject(new Player(800, 200, 80, _scene, false, rightWeapon, rightLocal));
        }

        // מחזיר את כמות הכדורים שנותרו במגזין של השחקן המתאים (שמאל/ימין)
        public int GetBullets(bool isLeft)
        {
            var player = _scene.GetPlayer(isLeft);
            return player?.BulletsInMagazine ?? 0; // מחזיר 0 אם השחקן לא נמצא
        }

        // מחזיר את מצב כל המחסות בסצנה כ-ReadOnlyList של CoverState (למשל לשידור ברשת)
        public IReadOnlyList<CoverState> GetCoverStates()
        {
            return _scene.GameObjectsSnapshot
                .OfType<Covers>() // מסנן רק עצמים מסוג Covers
                .Select(c => new CoverState
                {
                    Type = (int)c.CoverKind, // סוג המחסה כמספר
                    X = c.X, // מיקום X
                    Y = c.Y, // מיקום Y
                    Size = c.Image.Width // גודל/רוחב המחסה
                })
                .ToList(); // מחזיר רשימה של מצבים
        }
    }
}