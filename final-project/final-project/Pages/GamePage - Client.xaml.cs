using final_project.GameServices;
using GameEngine.Services;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace final_project.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GamePage_Client : Page
    {
        private GameManager _manager;
        private NetworkClient networkClient;
        private DispatcherTimer gameLoop;
        public GamePage_Client()
        {
            this.InitializeComponent();
            networkClient = new NetworkClient();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }
        private async void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            string serverIP = ServerIPTextBox.Text; // User enters "192.168.1.100"

            // Connect to server
            await networkClient.ConnectAsync(serverIP);

            // Listen for opponent updates
            networkClient.OpponentDataReceived += UpdateOpponentPosition;
            networkClient.StatusChanged += async (msg) => //await SaveStringToUserLocationAsync(msg);
             StatusTextBlock.Text = msg;

            // Start game loop
            gameLoop = new DispatcherTimer();
            gameLoop.Interval = TimeSpan.FromMilliseconds(16); // ~60 FPS
            gameLoop.Tick += GameLoop_Tick;
            gameLoop.Start();
        }
        public async Task SaveStringToUserLocationAsync(string text)
        {
            var savePicker = new FileSavePicker
            {
                SuggestedStartLocation = PickerLocationId.Desktop, // user can pick Desktop
                SuggestedFileName = "myfile"
            };
            savePicker.FileTypeChoices.Add("Text file", new[] { ".txt" });

            StorageFile file = await savePicker.PickSaveFileAsync();
            if (file != null)
            {
                await FileIO.WriteTextAsync(file, text);
            }
        }
        private void GameLoop_Tick(object sender, object e)
        {
            // Update local player position based on input
            double playerX = 300, playerY = 400; // Your game logic here

            // Create state object
            PlayerState state = new PlayerState
            {
                PlayerId = 2,
                X = playerX,
                Y = playerY,
                VelocityX = 0,
                VelocityY = 0,
                Action = "idle",
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
            };

            // Send to opponent
            _ = networkClient.SendPlayerStateAsync(state);
        }
        private void UpdateOpponentPosition(PlayerState opponentState)
        {
            // Update opponent sprite position on UI
            Debug.WriteLine($"Opponent at: {opponentState.X}, {opponentState.Y}");
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
