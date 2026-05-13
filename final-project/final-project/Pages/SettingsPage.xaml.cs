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
            RefreshKeyTexts();
        }

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

        private string _waitingForAction = null;

        private void ChangeKeyClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is string actionName)
            {
                _waitingForAction = actionName;
                btn.Content = "Press key...";
                Window.Current.CoreWindow.KeyDown += CoreWindow_KeyDownForBinding;
            }
        }

        private void CoreWindow_KeyDownForBinding(CoreWindow sender, KeyEventArgs args)
        {
            // Only handle one key press
            Window.Current.CoreWindow.KeyDown -= CoreWindow_KeyDownForBinding;

            if (_waitingForAction == null)
                return;

            VirtualKey key = args.VirtualKey;

            // Optionally: block Escape or other forbidden keys
            ApplyKeyBinding(_waitingForAction, key);
            GameKeys.SaveToSettings();
            _waitingForAction = null;

            // Update button texts with new key names
            _ = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, RefreshKeyTexts);
        }

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

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }

        private void VolumeSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            
        }
    }
}
