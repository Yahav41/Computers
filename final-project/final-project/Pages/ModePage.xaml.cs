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
            Frame.Navigate(typeof(GamePage));
        }

        private void LocalButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(GamePage));
        }

        private void OnlineButton_Click(object sender, RoutedEventArgs e)
        {
            StartReg();
            Frame.Navigate(typeof(GamePage));
        }
        private async Task StartReg()
        {
            _registration = new Registration();
            await _registration.ShowAsync();
        }
        private void NextImageButton_Click(object sender, RoutedEventArgs e)
        {
            NextImageButton(sender.Equals(NextButton1) ? ChosenImage1 : ChosenImage2);
            GameConstants.leftPlayer = (int)ChosenImage1.Margin.Right;
            GameConstants.rightPlayer = (int)ChosenImage2.Margin.Right;
        }
        private void NextImageButton(Image image)
        {
            switch (image.Margin.Right)
            {
                case 0:
                    {
                        image.Source = new BitmapImage(new Uri("ms-appx:///Assets/Models/Players/rifle/rifle-walk.gif"));
                        image.Margin = new Thickness(1, 1, 1, 1);
                        break;
                    }
                case 1:
                    {
                        image.Source = new BitmapImage(new Uri("ms-appx:///Assets/Models/Players/shotgun/shotgun-walk.gif"));
                        image.Margin = new Thickness(2, 2, 2, 2);
                        break;
                    }
                default:
                    {
                        image.Source = new BitmapImage(new Uri("ms-appx:///Assets/Models/Players/pistol/pistol-walk.gif"));
                        image.Margin = new Thickness(0, 0, 0, 0);
                        break;
                    }
            }
        }
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }
    }
}
