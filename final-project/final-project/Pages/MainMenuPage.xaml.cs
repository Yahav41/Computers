using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// דף תפריט ראשי — קלאס שמייצג את ה-Page של ה-Main Menu
namespace final_project.Pages
{
    public sealed partial class MainMenuPage : Page
    {
        // קונסטרקטור: אתחול רכיבי ה-XAML (InitializeComponent יוצר את העץ הוויזואלי)
        public MainMenuPage()
        {
            this.InitializeComponent();
        }

        // מטפל בלחיצה על כפתור START: ניווט לעמוד בחירת מצב/מפה (ModePage)
        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(ModePage));
        }

        // מטפל בלחיצה על כפתור SETTINGS: ניווט לעמוד ההגדרות (SettingsPage)
        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SettingsPage));
        }

        // מטפל בלחיצה על כפתור HELP: מציג/מסתיר רשת עזרה (HelpGrid) על ידי החלפת ה-Visibility
        private void HelpButton_Click(object sender, RoutedEventArgs e)
        {
            HelpGrid.Visibility = HelpGrid.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        }

        // מטפל בלחיצה על כפתור סגירה בתוך חלון העזרה: מסתיר את ה-HelpGrid
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            HelpGrid.Visibility = Visibility.Collapsed;
        }

        // אירוע טעינת הדף (יכול לשמש לאתחולים שנדרשים אחרי יצירת ה-UI)
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // כרגע אין לוגיקה כאן — מקום טוב להוסיף אתחול שמצריך את ה-Visual tree
        }
    }
}
