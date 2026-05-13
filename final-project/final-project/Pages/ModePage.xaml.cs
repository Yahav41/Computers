using final_project.GameServices;
using System;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace final_project.Pages
{
    public sealed partial class ModePage : Page
    {
        private Registration _registration;

        private WeaponType _leftWeapon = WeaponType.Pistol;
        private WeaponType _rightWeapon = WeaponType.Pistol;

        public ModePage()
        {
            this.InitializeComponent();

            // Ensure constants match the initial UI (pistols)
            GameConstants.leftPlayer = (int)_leftWeapon;
            GameConstants.rightPlayer = (int)_rightWeapon;
        }

        
        private void ServerButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(GamePage), Tuple.Create(GameRole.Server, (string)null));
        }

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

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }

        private void NextImageButton_Click(object sender, RoutedEventArgs e)
        {
            // Which side?
            bool isLeftButton = ReferenceEquals(sender, NextButton1);

            if (isLeftButton)
            {
                _leftWeapon = NextWeapon(_leftWeapon);
                UpdateWeaponImage(ChosenImage1, _leftWeapon);
            }
            else
            {
                _rightWeapon = NextWeapon(_rightWeapon);
                UpdateWeaponImage(ChosenImage2, _rightWeapon);
            }

            // Persist selection so GameManager can read it
            GameConstants.leftPlayer = (int)_leftWeapon;
            GameConstants.rightPlayer = (int)_rightWeapon;
        }

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

            image.Source = new BitmapImage(new Uri($"ms-appx:///{path}"));
        }
    }

    public enum WeaponType
    {
        Pistol = 0,
        Rifle = 1,
        Shotgun = 2
    }
}