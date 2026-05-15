using GameEngine.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace GameEngine.Services
{
    // מנהל המשחק: מטפל בסצנה, בקלט ממקלדת ובלולאת הריצה (טיק)
    public class Manager
    {
        private Scene _scene; // הסצנה הפעילה שהמנהל שולט עליה
        protected Random _random = new Random(); // גנרטור מספרים אקראיים לשימוש במשחק
        private DispatcherTimer _runTimer; // טיימר שמפעיל את האירוע המחזורי


        // אובייקט סטטי שמרכז אירועי משחק (OnRun, OnKeyClick, OnKeyRelease וכו')
        public static GameEvents Events { get; set; } = new GameEvents();

        // בנאי שמקבל סצנה ומתחבר לאירועי מקלדת ומפעיל טיימר
        public Manager(Scene scene)
        {
            _scene = scene;
            // רישום למאזיני מקלדת של חלון היישום
            Window.Current.CoreWindow.KeyUp += CoreWindow_KeyUp;
            Window.Current.CoreWindow.KeyDown += CoreWindow_KeyDown;

            // הגדרת טיימר להפעלת לולאת הריצה
            _runTimer = new DispatcherTimer();
            _runTimer.Interval += TimeSpan.FromMilliseconds(0.01); // שימו לב: שימוש ב-'+=' כאן נראה חריג — בדרך כלל עושים השמה של TimeSpan
            _runTimer.Start();
            _runTimer.Tick += _runTimer_Tick;
        }

        // מטפל באירוע Tick של הטיימר: קורא ל-Events.OnRun כדי להריץ לוגיקה וסצנות
        protected virtual void _runTimer_Tick(object sender, object e)
        {
            Events.OnRun();
        }

        // מטפל בלחיצת מקש (KeyDown): קורא ל-Events.OnKeyClick עם המקשים שנלחצו
        private void CoreWindow_KeyDown(Windows.UI.Core.CoreWindow sender, Windows.UI.Core.KeyEventArgs args)
        {
            try
            {
                Events.OnKeyClick(args.VirtualKey);
            }
            catch
            {
                // פעולה שקטה במקרה של שגיאה — עדיף לרשום לוג אבל כאן נשתקת השגיאה
            }
        }

        // מטפל בשחרור מקש (KeyUp): קורא ל-Events.OnKeyRelease
        private void CoreWindow_KeyUp(Windows.UI.Core.CoreWindow sender, Windows.UI.Core.KeyEventArgs args)
        {
            try
            {
                Events.OnKeyRelease(args.VirtualKey);
            }
            catch
            {
                // כמו למעלה — תיעוד שגיאות מומלץ במקום השתקה
            }
        }
    }
}
