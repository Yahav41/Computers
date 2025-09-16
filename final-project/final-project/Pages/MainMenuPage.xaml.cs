using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace final_project.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainMenuPage : Page
    {
        public MainMenuPage()
        {
            this.InitializeComponent();
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(ModePage));
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SettingsPage));
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(LoginPage));
        }

        private void SignupButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SignupPage));
        }

        private void HelpButton_Click(object sender, RoutedEventArgs e)
        {
            HelpGrid.Visibility = Visibility.Visible;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            HelpGrid.Visibility = Visibility.Collapsed;
        }
    }
}
