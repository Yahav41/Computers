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
            Frame.Navigate(typeof(GamePage_Client));
        }

        private void LocalButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(GamePage));
        }

        private void OnlineButton_Click(object sender, RoutedEventArgs e)
        {
           // StartReg();
            Frame.Navigate(typeof(GamePage));
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

        private void NextImageButton_Click(object sender, RoutedEventArgs e)
        {
            var target = sender.Equals(NextButton1) ? ref _leftWeapon : ref _rightWeapon;
            target = NextWeapon(target);
            UpdateWeaponImage(sender.Equals(NextButton1) ? ChosenImage1 : ChosenImage2, target);

            GameConstants.LeftPlayerWeapon = target;
            GameConstants.RightPlayerWeapon = _rightWeapon;
        }

        private WeaponType NextWeapon(WeaponType current)
        {
            return current switch
            {
                WeaponType.Pistol => WeaponType.Rifle,
                WeaponType.Rifle => WeaponType.Shotgun,
                _ => WeaponType.Pistol
            };
        }

        private void UpdateWeaponImage(Image image, WeaponType type)
        {
            string path = type switch
            {
                WeaponType.Pistol => "Assets/Models/Players/pistol/pistol-walk.gif",
                WeaponType.Rifle => "Assets/Models/Players/rifle/rifle-walk.gif",
                WeaponType.Shotgun => "Assets/Models/Players/shotgun/shotgun-walk.gif",
                _ => "Assets/Models/Players/pistol/pistol-walk.gif"
            };

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
        public static WeaponProfile ToProfile(this WeaponType type) =>
            type switch
            {
                WeaponType.Pistol => WeaponProfile.Pistol,
                WeaponType.Rifle => WeaponProfile.Rifle,
                WeaponType.Shotgun => WeaponProfile.Shotgun,
                _ => WeaponProfile.Pistol
            };
    }
}
