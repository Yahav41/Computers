using System;
using Windows.System;
using Windows.Storage;

namespace finalproject.GameServices
{
    // מחלקה סטטית שמרכזת את המיפוי של מקשי המשחק לשני השחקנים
    public static class GameKeys
    {
        // הגדרת מקשי ברירת מחדל לשחקן השמאלי
        public static VirtualKey LeftPlayerLeft { get; set; } = VirtualKey.A;    // תזוזה שמאלה
        public static VirtualKey LeftPlayerRight { get; set; } = VirtualKey.D;   // תזוזה ימינה
        public static VirtualKey LeftPlayerDown { get; set; } = VirtualKey.S;    // תזוזה מטה
        public static VirtualKey LeftPlayerUp { get; set; } = VirtualKey.W;      // תזוזה מעלה
        public static VirtualKey LeftPlayerShoot { get; set; } = VirtualKey.F;   // ירי
        public static VirtualKey LeftPlayerReload { get; set; } = VirtualKey.R;  // טעינה מחדש

        // הגדרת מקשי ברירת מחדל לשחקן הימני
        public static VirtualKey RightPlayerLeft { get; set; } = VirtualKey.Left;   // חץ שמאלה
        public static VirtualKey RightPlayerRight { get; set; } = VirtualKey.Right; // חץ ימינה
        public static VirtualKey RightPlayerDown { get; set; } = VirtualKey.Down;   // חץ מטה
        public static VirtualKey RightPlayerUp { get; set; } = VirtualKey.Up;       // חץ מעלה
        public static VirtualKey RightPlayerShoot { get; set; } = VirtualKey.Shift; // ירי (שיפט)
        public static VirtualKey RightPlayerReload { get; set; } = VirtualKey.L;    // טעינה מחדש (L)

        // אחסון מקומי (ApplicationData.LocalSettings) לשמירת קיצורי מקשים בין ריצות
        private static readonly ApplicationDataContainer Settings = ApplicationData.Current.LocalSettings;

        // קורא מההגדרות המקומיות ומעדכן את המפתחות (אם קיימים)
        public static void LoadFromSettings()
        {
            LeftPlayerLeft = ReadKey("LeftPlayerLeft", LeftPlayerLeft);
            LeftPlayerRight = ReadKey("LeftPlayerRight", LeftPlayerRight);
            LeftPlayerDown = ReadKey("LeftPlayerDown", LeftPlayerDown);
            LeftPlayerUp = ReadKey("LeftPlayerUp", LeftPlayerUp);
            LeftPlayerShoot = ReadKey("LeftPlayerShoot", LeftPlayerShoot);
            LeftPlayerReload = ReadKey("LeftPlayerReload", LeftPlayerReload);

            RightPlayerLeft = ReadKey("RightPlayerLeft", RightPlayerLeft);
            RightPlayerRight = ReadKey("RightPlayerRight", RightPlayerRight);
            RightPlayerDown = ReadKey("RightPlayerDown", RightPlayerDown);
            RightPlayerUp = ReadKey("RightPlayerUp", RightPlayerUp);
            RightPlayerShoot = ReadKey("RightPlayerShoot", RightPlayerShoot);
            RightPlayerReload = ReadKey("RightPlayerReload", RightPlayerReload);
        }

        // שומר את המפתחות הנוכחיים בהגדרות המקומיות
        public static void SaveToSettings()
        {
            WriteKey("LeftPlayerLeft", LeftPlayerLeft);
            WriteKey("LeftPlayerRight", LeftPlayerRight);
            WriteKey("LeftPlayerDown", LeftPlayerDown);
            WriteKey("LeftPlayerUp", LeftPlayerUp);
            WriteKey("LeftPlayerShoot", LeftPlayerShoot);
            WriteKey("LeftPlayerReload", LeftPlayerReload);

            WriteKey("RightPlayerLeft", RightPlayerLeft);
            WriteKey("RightPlayerRight", RightPlayerRight);
            WriteKey("RightPlayerDown", RightPlayerDown);
            WriteKey("RightPlayerUp", RightPlayerUp);
            WriteKey("RightPlayerShoot", RightPlayerShoot);
            WriteKey("RightPlayerReload", RightPlayerReload);
        }

        // קורא ערך מקש מההגדרות; אם לא קיים מחזיר את הערך הדיפולטי שנמסר
        private static VirtualKey ReadKey(string name, VirtualKey defaultValue)
        {
            object value = Settings.Values[name];
            if (value is int intVal)
                return (VirtualKey)intVal; // מאחסן כ-int אז ממיר בחזרה ל-VirtualKey
            return defaultValue;
        }

        // כותב ערך מקש להגדרות (שומר כ-int)
        private static void WriteKey(string name, VirtualKey key)
        {
            Settings.Values[name] = (int)key;
        }
    }
}