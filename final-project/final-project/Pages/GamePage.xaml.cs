using final_project.GameObjects;
using final_project.GameServices;
using GameEngine.Services;
using System;
using System.Diagnostics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace final_project.Pages
{
    public enum GameRole
    {
        Server,
        Client
    }

    public sealed partial class GamePage : Page
    {
        private GameManager _manager;
        private IGameNetwork _network;
        private DispatcherTimer _gameLoop;
        private GameRole _role;
        private string _serverIp;

        public GamePage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is Tuple<GameRole, string> p)
            {
                _role = p.Item1;
                _serverIp = p.Item2;
            }
            else
            {
                _role = GameRole.Server;
                _serverIp = null;
            }

            base.OnNavigatedTo(e);
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            _manager = new GameManager(scene, _role == GameRole.Server);
            UpdateBullets();

            Manager.Events.OnRemoveLifes += RemoveLives;
            Manager.Events.onBulletShot += BulletShot;
            Manager.Events.onReload += Reload;

            _network = _role == GameRole.Server
                ? (IGameNetwork)new ServerNetwork()
                : new ClientNetwork();

            _network.OpponentStateReceived += UpdateOpponentPosition;
            _network.StatusChanged += msg => StatusTextBlock.Text = msg;

            await _network.StartOrConnectAsync(_serverIp);

            _gameLoop = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(16)
            };
            _gameLoop.Tick += GameLoop_Tick;
            _gameLoop.Start();
        }

        private void GameLoop_Tick(object sender, object e)
        {
            try
            {
                bool isLocalLeft = _role == GameRole.Server;
                var localPlayer = _manager.Scene.GetPlayer(isLocalLeft);
                if (localPlayer == null) return;

                PlayerState state = new PlayerState
                {
                    PlayerId = isLocalLeft ? 1 : 2,
                    X = localPlayer.X,
                    Y = localPlayer.Y,
                    VelocityX = localPlayer.SpeedX,
                    VelocityY = localPlayer.SpeedY,
                    Rotation = localPlayer.Image.Rotation,
                    Type = localPlayer.WeaponTypeIndex,
                    Action = localPlayer.State.ToString(),
                    Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
                };

                _ = _network.SendAsync(state);
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
                bool opponentIsLeft = _role == GameRole.Client;
                var opponentPlayer = _manager.Scene.GetPlayer(opponentIsLeft);

                bool needsRecreate = opponentPlayer == null ||
                                     opponentPlayer.WeaponTypeIndex != opponentState.Type;

                if (needsRecreate)
                {
                    RecreateOpponentPlayer(opponentState.Type,
                        opponentState.X,
                        opponentState.Y,
                        opponentIsLeft);

                    opponentPlayer = _manager.Scene.GetPlayer(opponentIsLeft);
                    if (opponentPlayer == null) return;
                }

                opponentPlayer.X = opponentState.X;
                opponentPlayer.Y = opponentState.Y;
                opponentPlayer.SpeedX = opponentState.VelocityX;
                opponentPlayer.SpeedY = opponentState.VelocityY;
                opponentPlayer.Image.Rotation = opponentState.Rotation;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"UpdateOpponentPosition Error: {ex.Message}");
            }
        }

        private void RecreateOpponentPlayer(int typeIndex, double x, double y, bool isLeft)
        {
            var weapon = typeIndex switch
            {
                0 => WeaponProfile.Pistol,
                1 => WeaponProfile.Rifle,
                2 => WeaponProfile.Shotgun,
                _ => WeaponProfile.Pistol
            };

            var old = _manager.Scene.GetPlayer(isLeft);
            if (old != null)
            {
                _manager.Scene.RemoveObject(old);
            }

            var player = new Player(x, y, 80, _manager.Scene, isLeft, weapon);
            _manager.Scene.AddObject(player);

            UpdateBullets();
        }

        private void UpdateBullets()
        {
            LeftPlayerBullets.Text = _manager.GetBullets(true).ToString();
            RightPlayerBullets.Text = _manager.GetBullets(false).ToString();
        }

        private void Reload(bool isLeft)
        {
            UpdateBullets();
        }

        private void BulletShot(bool isLeft)
        {
            UpdateBullets();
        }

        private void RemoveLives(bool isLeft, int damage)
        {
            if (isLeft)
            {
                LeftPlayerHealth.Value -= damage;
            }
            else
            {
                RightPlayerHealth.Value -= damage;
            }

            if (LeftPlayerHealth.Value <= 0 || RightPlayerHealth.Value <= 0)
            {
                WinGrid.Visibility = Visibility.Visible;
                WinnerTextBlock.Text = LeftPlayerHealth.Value >= RightPlayerHealth.Value
                    ? "LeftPlayerWins"
                    : "RightPlayerWins";
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
            _network?.Stop();
            _gameLoop?.Stop();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
            _network?.Stop();
            _gameLoop?.Stop();
        }
    }
}