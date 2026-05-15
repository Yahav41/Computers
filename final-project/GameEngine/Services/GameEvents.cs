using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System;

namespace GameEngine.Services
{
    // מחלקה שמכילה מצביעים עבור אירועי המשחק
    // כל שדה מסוג Action מיועד להישאב על ידי מאזינים חיצוניים
    public class GameEvents
    {
        // קורא כאשר נלחץ מקש; מקבל את ה-VirtualKey שנלחץ
        public Action<VirtualKey> OnKeyClick;

        // קורא בכל טיק/עדכון של הלולאה הראשית (Run)
        public Action OnRun;

        // קורא כאשר מקש משוחרר; מקבל את ה-VirtualKey ששוחרר
        public Action<VirtualKey> OnKeyRelease;

        // קורא לעדכון ניקוד; מקבל שני מספרים (למשל: סכום ונקודה/מזהה)
        public Action<int, int> onUpdateScore;

        // קורא להסרת חיים; מקבל בוליאן ואינט (משמש לפי לוגיקת המשחק)
        public Action<bool, int> OnRemoveLifes;

        // קורא כאשר נורה כדור; הפרמטר בוליאני כנראה מציין מצב/מקור הירי
        public Action<bool> onBulletShot;

        // קורא בעת טעינה מחודשת של נשק; הפרמטר בוליאני כנראה מצב ותלות במספר
        public Action<bool> onReload;

        // בנאי ריק — השדות יהיו ברירת-מחדל null עד שישימו להם מאזינים
        public GameEvents()
        {

        }
    }
}
