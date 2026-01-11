using final_project.GameObjects;
using final_project.GameServices;
using GameEngine.Services;
using System;
using System.Diagnostics;
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
        private NetworkServer networkServer;
        private DispatcherTimer gameLoop;
        public GamePage()
        {
            this.InitializeComponent();
            networkServer = new NetworkServer();

        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }


        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            _manager = new GameManager(scene);
            LeftPlayerBullets.Text = _manager.getBullets(true).ToString();
            RightPlayerBullets.Text = _manager.getBullets(false).ToString();
            Manager.Events.OnRemoveLifes += RemoveLives;
            Manager.Events.onBulletShot += BulletShot;
            Manager.Events.onReload += Reload;

            // Start server
            await networkServer.StartServerAsync();

            // Listen for opponent updates
            await networkServer.StartServerAsync();
            networkServer.OpponentDataReceived += UpdateOpponentPosition;
            networkServer.StatusChanged += msg => StatusTextBlock.Text = msg;

            gameLoop = new DispatcherTimer();
            gameLoop.Interval = TimeSpan.FromMilliseconds(16);
            gameLoop.Tick += GameLoop_Tick;
            gameLoop.Start();
        }

        private void GameLoop_Tick(object sender, object e)
        {
            try
            {
                // Get the actual server player (leftPlayer, isLeft = true)
                Players serverPlayer = _manager._scene.getPlayer(true);

                if (serverPlayer != null)
                {
                    // Create state with REAL position and data
                    PlayerState state = new PlayerState
                    {
                        PlayerId = 1,
                        X = serverPlayer._x,
                        Y = serverPlayer._y,
                        VelocityX = serverPlayer._speedX,
                        VelocityY = serverPlayer._speedY,
                        Rotation = serverPlayer.Image.Rotation,
                        Action = serverPlayer._playerState.ToString(),
                        Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
                    };

                    // Send to client
                    _ = networkServer.SendPlayerStateAsync(state);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"GameLoopTick Error: {ex.Message}");
            }
        }
        private void UpdateOpponentPosition(PlayerState opponentState)
        {
            if (opponentState == null) return;

            try
            {
                // On server, opponent is rightPlayer (isLeft = false)
                Players opponentPlayer = _manager._scene.getPlayer(false);

                if (opponentPlayer != null)
                {
                    // Update position
                    opponentPlayer._x = opponentState.X;
                    opponentPlayer._y = opponentState.Y;

                    // Update velocity for smooth movement
                    opponentPlayer._speedX = opponentState.VelocityX;
                    opponentPlayer._speedY = opponentState.VelocityY;

                    // Update rotation (facing direction)
                    opponentPlayer.Image.Rotation = opponentState.Rotation;

                    Debug.WriteLine($"[Server] Updated opponent at ({opponentState.X:F2}, {opponentState.Y:F2})");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"UpdateOpponentPosition Error: {ex.Message}");
            }
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
