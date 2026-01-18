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

        private bool NeedToRecreatePlayer(Players currentPlayer, int newCharacterType)
        {
            if (currentPlayer == null) return true;

            bool isSameType = (currentPlayer is PistolPlayer && newCharacterType == 0) ||
                              (currentPlayer is RiflePlayer && newCharacterType == 1) ||
                              (currentPlayer is ShotgunPlayer && newCharacterType == 2);

            return !isSameType;
        }


        private void RecreateOpponentPlayer(int characterType, double x, double y, bool isLeft)
        {
            try
            {
                Players oldPlayer = _manager._scene.getPlayer(isLeft);
                if (oldPlayer != null) _manager._scene.RemoveObject(oldPlayer);

                switch (characterType)
                {
                    case 0: _manager._scene.AddObject(new PistolPlayer(x, y, _manager._scene, isLeft)); break;
                    case 1: _manager._scene.AddObject(new RiflePlayer(x, y, _manager._scene, isLeft)); break;
                    case 2: _manager._scene.AddObject(new ShotgunPlayer(x, y, _manager._scene, isLeft)); break;
                }
            }
            catch (Exception ex) { Debug.WriteLine($"Error: {ex.Message}"); }
        }



        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            _manager = new GameManager(scene);
            LeftPlayerBullets.Text = _manager.getBullets(true).ToString();
            RightPlayerBullets.Text = _manager.getBullets(false).ToString();
            Manager.Events.OnRemoveLifes += RemoveLives;
            Manager.Events.onBulletShot += BulletShot;
            Manager.Events.onReload += Reload;

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
                        Type = serverPlayer.Type(),
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

                // FIRST: Check if character type changed and recreate if needed
                bool needsRecreate = opponentPlayer == null || NeedToRecreatePlayer(opponentPlayer, opponentState.Type);

                if (needsRecreate)
                {
                    Debug.WriteLine($"[Server] Recreating opponent - Type: {opponentState.Type}");
                    RecreateOpponentPlayer(opponentState.Type, opponentState.X, opponentState.Y, false);

                    // CRITICAL: Get the newly created player!
                    opponentPlayer = _manager._scene.getPlayer(false);

                    if (opponentPlayer == null)
                    {
                        Debug.WriteLine("[Server] ERROR: Failed to create opponent player!");
                        return;
                    }
                }

                // NOW update the (newly created or existing) player
                if (opponentPlayer != null)
                {
                    opponentPlayer._x = opponentState.X;
                    opponentPlayer._y = opponentState.Y;
                    opponentPlayer._speedX = opponentState.VelocityX;
                    opponentPlayer._speedY = opponentState.VelocityY;
                    opponentPlayer.Image.Rotation = opponentState.Rotation;

                    Debug.WriteLine($"[Server] Updated opponent - Type: {opponentState.Type}, Pos: ({opponentState.X:F2}, {opponentState.Y:F2})");
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
            networkServer = null;
            gameLoop = null;
            _manager = null;
        }
    }
}
