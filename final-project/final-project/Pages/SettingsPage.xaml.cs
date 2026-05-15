using finalproject.GameServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace final_project.Pages
{
    // דף הגדרות המקשר את כפתורי הממשק ל-GameKeys ושומר/טוען אותם מהגדרות מקומיות
    public sealed partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            this.InitializeComponent();
            RefreshKeyTexts(); // מעדכן טקסטים על הכפתורים לפי מיפויי המקש הנוכחיים
        }

        // מעדכן את הטקסט של כל כפתורי המיפוי לפי הערכים הנוכחיים ב-GameKeys
        private void RefreshKeyTexts()
        {
            // Left player
            BtnLeftMoveLeft.Content = GameKeys.LeftPlayerLeft.ToString();
            BtnLeftMoveRight.Content = GameKeys.LeftPlayerRight.ToString();
            BtnLeftMoveUp.Content = GameKeys.LeftPlayerUp.ToString();
            BtnLeftMoveDown.Content = GameKeys.LeftPlayerDown.ToString();
            BtnLeftShoot.Content = GameKeys.LeftPlayerShoot.ToString();
            BtnLeftReload.Content = GameKeys.LeftPlayerReload.ToString();

            // Right player
            BtnRightMoveLeft.Content = GameKeys.RightPlayerLeft.ToString();
            BtnRightMoveRight.Content = GameKeys.RightPlayerRight.ToString();
            BtnRightMoveUp.Content = GameKeys.RightPlayerUp.ToString();
            BtnRightMoveDown.Content = GameKeys.RightPlayerDown.ToString();
            BtnRightShoot.Content = GameKeys.RightPlayerShoot.ToString();
            BtnRightReload.Content = GameKeys.RightPlayerReload.ToString();
        }

        private string _waitingForAction = null; // שם הפעולה שאנו מחכים לקשירת מקש חדש עבורה

        // נרוץ כאשר המשתמש לוחץ על כפתור "שנה מקש" — מתחילים להאזין לאירוע KeyDown הגלובלי
        private void ChangeKeyClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is string actionName)
            {
                _waitingForAction = actionName;        // שומרים איזו פעולה לשייך
                btn.Content = "Press key...";         // משנים טקסט כדי לתת משוב למשתמש
                Window.Current.CoreWindow.KeyDown += CoreWindow_KeyDownForBinding; // מאזינים למקש הבא
            }
        }

        // מטפל בלחיצה על מקש בזמן שאנו במצב קשירת מקש חדש
        private void CoreWindow_KeyDownForBinding(CoreWindow sender, KeyEventArgs args)
        {
            // מטפל רק בפעם הראשונה ואז מסיר את המאזין
            Window.Current.CoreWindow.KeyDown -= CoreWindow_KeyDownForBinding;

            if (_waitingForAction == null)
                return;

            VirtualKey key = args.VirtualKey;

            // ניתן להוסיף כאן בדיקות למקשים אסורים (למשל Escape) לפני שמיישמים
            ApplyKeyBinding(_waitingForAction, key); // מיישם את הקישור החדש ב-GameKeys
            GameKeys.SaveToSettings();               // שומר את המיפוי בהגדרות המקומיות
            _waitingForAction = null;

            // מרענן את טקסטי הכפתורים על ה-UI thread
            _ = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, RefreshKeyTexts);
        }

        // ממפה את שם הפעולה לשדה המתאים ב-GameKeys
        private void ApplyKeyBinding(string action, VirtualKey key)
        {
            switch (action)
            {
                case "LeftPlayerLeft": GameKeys.LeftPlayerLeft = key; break;
                case "LeftPlayerRight": GameKeys.LeftPlayerRight = key; break;
                case "LeftPlayerUp": GameKeys.LeftPlayerUp = key; break;
                case "LeftPlayerDown": GameKeys.LeftPlayerDown = key; break;
                case "LeftPlayerShoot": GameKeys.LeftPlayerShoot = key; break;
                case "LeftPlayerReload": GameKeys.LeftPlayerReload = key; break;

                case "RightPlayerLeft": GameKeys.RightPlayerLeft = key; break;
                case "RightPlayerRight": GameKeys.RightPlayerRight = key; break;
                case "RightPlayerUp": GameKeys.RightPlayerUp = key; break;
                case "RightPlayerDown": GameKeys.RightPlayerDown = key; break;
                case "RightPlayerShoot": GameKeys.RightPlayerShoot = key; break;
                case "RightPlayerReload": GameKeys.RightPlayerReload = key; break;
            }
        }

        // כפתור חזרה - חוזר לדף הקודם
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }

        // מטפל בשינוי עוצמת קול (כרגע ריק - מקום להוספת לוגיקה)
        private void VolumeSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            
        }
    }
}
