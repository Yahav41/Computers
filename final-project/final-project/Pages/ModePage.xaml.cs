using final_project.GameServices;
using System;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace final_project.Pages
{
    // דף בחירת מצב/תצורה לפני כניסה למשחק
    public sealed partial class ModePage : Page
    {
        private Registration _registration; // דיאלוג רישום/הזנת כתובת למצב לקוח

        // מצביעים לבחירת נשק לשני הצדדים (ברירת מחדל: אקדח)
        private WeaponType _leftWeapon = WeaponType.Pistol;
        private WeaponType _rightWeapon = WeaponType.Pistol;

        public ModePage()
        {
            this.InitializeComponent(); // אתחול רכיבי XAML

            // מוודא שקבועי המשחק ישקפו את הבחירה הראשונית בממשק
            GameConstants.leftPlayer = (int)_leftWeapon;
            GameConstants.rightPlayer = (int)_rightWeapon;
        }

        // לחיצה על כפתור 'שרת' - ניווט לעמוד המשחק בתפקיד שרת
        private void ServerButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(GamePage), Tuple.Create(GameRole.Server, (string)null));
        }

        // לחיצה על כפתור 'לקוח' - פותח דיאלוג להזנת IP ואז נווט כלקוח אם התקבל IP
        private async void ClientButton_Click(object sender, RoutedEventArgs e)
        {
            _registration = new Registration();
            var result = await _registration.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                var ip = _registration.IpAddress;

                if (!string.IsNullOrWhiteSpace(ip))
                {
                    Frame.Navigate(typeof(GamePage), Tuple.Create(GameRole.Client, ip));
                }
            }
        }

        // לחיצה על כפתור חזרה - חוזר לדף הקודם
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }

        // לחיצה על חץ לשינוי תמונה/נשק: מחליפה את הנשק המתאים (שמאל/ימין)
        private void NextImageButton_Click(object sender, RoutedEventArgs e)
        {
            // בודק האם הכפתור ששוחרר שייך לשחקן השמאלי או הימני
            bool isLeftButton = ReferenceEquals(sender, NextButton1);

            if (isLeftButton)
            {
                _leftWeapon = NextWeapon(_leftWeapon); // מקבל את הנשק הבא בסדרה
                UpdateWeaponImage(ChosenImage1, _leftWeapon); // מעדכן את התמונה ב-UI
            }
            else
            {
                _rightWeapon = NextWeapon(_rightWeapon);
                UpdateWeaponImage(ChosenImage2, _rightWeapon);
            }

            // שומר את הבחירות בקבועי המשחק כדי ש-GameManager יקרא אותן בהמשך
            GameConstants.leftPlayer = (int)_leftWeapon;
            GameConstants.rightPlayer = (int)_rightWeapon;
        }

        // מתחלף בין הנשקים בזוּת קבועה: Pistol -> Rifle -> Shotgun -> Pistol
        private WeaponType NextWeapon(WeaponType current)
        {
            switch (current)
            {
                case WeaponType.Pistol: return WeaponType.Rifle;
                case WeaponType.Rifle: return WeaponType.Shotgun;
                case WeaponType.Shotgun:
                default: return WeaponType.Pistol;
            }
        }

        // מעדכן את מקור התמונה של ה-`Image` בהתאם לטיפוס הנשק (שימוש ב־BitmapImage)
        private void UpdateWeaponImage(Image image, WeaponType type)
        {
            string path;

            switch (type)
            {
                case WeaponType.Pistol:
                    path = "Assets/Models/Players/pistol/pistol-walk.gif";
                    break;
                case WeaponType.Rifle:
                    path = "Assets/Models/Players/rifle/rifle-walk.gif";
                    break;
                case WeaponType.Shotgun:
                    path = "Assets/Models/Players/shotgun/shotgun-walk.gif";
                    break;
                default:
                    path = "Assets/Models/Players/pistol/pistol-walk.gif";
                    break;
            }

            // הנחיה לטעינת תמונה מתוך חבילת היישום (ms-appx:///)
            image.Source = new BitmapImage(new Uri($"ms-appx:///{path}"));
        }
    }

    // enum מקומי למימוש בחירה במסך זה (מתאים ל-WeaponProfile.Type בהמשך)
    public enum WeaponType
    {
        Pistol = 0,
        Rifle = 1,
        Shotgun = 2
    }
}