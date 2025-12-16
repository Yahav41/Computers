using final_project.GameServices;
using GameEngine.Services;
using System;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace final_project.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GamePage : Page
    {
        private GameManager _manager;
        public GamePage()
        {
            this.InitializeComponent();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }



        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            _manager = new GameManager(scene);
            LeftPlayerBullets.Text = _manager.getBullets(true).ToString();
            RightPlayerBullets.Text = _manager.getBullets(false).ToString();
            Manager.Events.OnRemoveLifes += RemoveLives;
            Manager.Events.onBulletShot += BulletShot;
            Manager.Events.onReload += Reload;
        }

        private void Reload(bool obj)
        {
            if(obj)
            {

                LeftPlayerBullets.Text = _manager.getBullets(true).ToString();
            }
            else
            {
                RightPlayerBullets.Text = _manager.getBullets(false).ToString();
            }
        }

        private void BulletShot(bool obj)
        {
            if (obj)
            {

                LeftPlayerBullets.Text = _manager.getBullets(true).ToString();
            }
            else
            {
                RightPlayerBullets.Text = _manager.getBullets(false).ToString();
            }
        }

        private void RemoveLives(bool isLeft, int Lives)
        {
            if(isLeft)
            {
                LeftPlayerHealth.Value -= Lives;
            }
            else
            {
                RightPlayerHealth.Value -= Lives;
            }
            if(LeftPlayerHealth.Value <= 0 || RightPlayerHealth.Value <= 0)
            {
                WinGrid.Visibility = Visibility.Visible;
                WinnerTextBlock.Text = LeftPlayerHealth.Value >= RightPlayerHealth.Value ? "LeftPlayerWins" : "RightPlayerWins";
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }
    }
}
