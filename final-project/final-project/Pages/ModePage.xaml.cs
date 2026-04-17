using final_project.GameServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace final_project.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ModePage : Page
    {
        private Registration _registration;
        public ModePage()
        {
            this.InitializeComponent();
        }

        private void OfflineButton_Click(object sender, RoutedEventArgs e)
        {
            // Offline/local → server role, no IP needed
            Frame.Navigate(typeof(GamePage), Tuple.Create(GameRole.Server, (string)null));
        }

        private void LocalButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(GamePage), Tuple.Create(GameRole.Server, (string)null));
        }

        private async void OnlineButton_Click(object sender, RoutedEventArgs e)
        {
            _registration = new Registration();
            var result = await _registration.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                var ip = _registration.IpAddress;   // we add this property below

                if (!string.IsNullOrWhiteSpace(ip))
                {
                    Frame.Navigate(typeof(GamePage), Tuple.Create(GameRole.Client, ip));
                }
            }
        }
        private async Task StartReg()
        {
            _registration = new Registration();
            await _registration.ShowAsync();
        }
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }
        private WeaponType _leftWeapon = WeaponType.Pistol;
        private WeaponType _rightWeapon = WeaponType.Pistol;

        private int _leftWeaponIndex = 0;   // 0 pistol, 1 rifle, 2 shotgun
        private int _rightWeaponIndex = 0;

        private void NextImageButton_Click(object sender, RoutedEventArgs e)
        {
            var target = sender.Equals(NextButton1) ? ref _leftWeapon : ref _rightWeapon;
            target = NextWeapon(target);
            UpdateWeaponImage(sender.Equals(NextButton1) ? ChosenImage1 : ChosenImage2, target);

            // Keep using ints in GameConstants for now
            GameConstants.leftPlayer = (int)_leftWeapon;
            GameConstants.rightPlayer = (int)_rightWeapon;
        }

        private WeaponType NextWeapon(WeaponType current)
        {
            switch (current)
            {
                case WeaponType.Pistol:
                    return WeaponType.Rifle;
                case WeaponType.Rifle:
                    return WeaponType.Shotgun;
                default:
                    return WeaponType.Pistol;
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

    public static class WeaponSelector
    {
        public static WeaponProfile ToProfile(this WeaponType type)
        {
            switch (type)
            {
                case WeaponType.Pistol:
                    return WeaponProfile.Pistol;
                case WeaponType.Rifle:
                    return WeaponProfile.Rifle;
                case WeaponType.Shotgun:
                    return WeaponProfile.Shotgun;
                default:
                    return WeaponProfile.Pistol;
            }
        }
    }
}
