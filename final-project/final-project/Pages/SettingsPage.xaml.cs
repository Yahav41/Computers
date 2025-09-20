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
    public sealed partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            this.InitializeComponent();
        }

        private void KeysButton_Click(object sender, RoutedEventArgs e)
        {
            KeysGrid.Visibility = KeysGrid.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
            AudioGrid.Visibility = Visibility.Collapsed;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            KeysGrid.Visibility = Visibility.Collapsed;
            AudioGrid.Visibility = Visibility.Collapsed;
        }

        private void VolumeSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            
        }

        private void AudioButton_Click(object sender, RoutedEventArgs e)
        {
            AudioGrid.Visibility = AudioGrid.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
            KeysGrid.Visibility = Visibility.Collapsed;
        }
    }
}
