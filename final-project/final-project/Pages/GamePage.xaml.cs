using final_project.GameObjects;
using final_project.GameServices;
using GameEngine.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace final_project.Pages
{
    // תפקיד המשחק — מי מריץ את השרת ומי הלקוח
    public enum GameRole
    {
        Server,
        Client
    }

    // דף המשחק שמאגד לוגיקה של סצנה, רשת ולולאת משחק
    public sealed partial class GamePage : Page
    {
        private GameManager _manager;                 // מנהל המשחק (יוצר עצמים, מחזיק סצנה)
        private IGameNetwork _network;                // ממשק תשתית רשת (שרת/לקוח)
        private DispatcherTimer _gameLoop;            // לולאת שליחה/סינכרון של מצב השחקן
        private GameRole _role;                       // תפקיד הנוכחי (Server/Client)
        private string _serverIp;                     // כתובת שרת במצב Client
        private bool _coversInitialized = false;      // האם התקבלו ויושמו מחסות מהשרת

        public GamePage()
        {
            this.InitializeComponent();
        }

        // קבלת פרמטרי ניווט (תפקיד + כתובת אם לקוח)
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

        // אתחול הדף: יצירת מנהל, רישום אירועים, התחברות לרשת והפעלת טיימר לולאת המשחק
        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // GameManager יוצר שחקנים ומחפשים (ובמידת הצורך) מחסות אם שרת
            _manager = new GameManager(scene, _role == GameRole.Server);
            UpdateBullets(); // עדכון תצוגת כדורים בהתחלה

            // רישום לאירועים גלובליים מהמנוע
            Manager.Events.OnRemoveLifes += RemoveLives; // כאשר שחקן מקבל פגיעה
            Manager.Events.onBulletShot += BulletShot;   // כאשר כדור נורה
            Manager.Events.onReload += Reload;           // כאשר נשק נטען מחדש

            // בחר מימוש רשת — שרת מקומי או לקוח מקומי
            _network = _role == GameRole.Server
                ? (IGameNetwork)new Network()
                : new LocalNetwork();

            // רישום אירועים מהרשת
            _network.OpponentStateReceived += UpdateOpponentPosition;
            _network.CoversReceived += ApplyCoverSnapshot;

            // עדכון סטטוס חיבור: אם השרת מקבל לקוח ישלח מסמך מחסות
            _network.StatusChanged += msg =>
            {
                StatusTextBlock.Text = msg;
                if (_role == GameRole.Server &&
                    msg.StartsWith("Client connected", StringComparison.OrdinalIgnoreCase))
                {
                    var covers = _manager.GetCoverStates();
                    _ = _network.SendCoversAsync(covers);
                }
            };

            // הפעלה/התחברות לרשת (אם לקוח ינסה להתחבר ל-_serverIp)
            await _network.StartOrConnectAsync(_serverIp);

            // טיימר שמייצר אירוע כל ~16ms לשליחת מצב השחקן לרשת
            _gameLoop = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(16)
            };
            _gameLoop.Tick += GameLoop_Tick;
            _gameLoop.Start();
        }

        // קריאה מחזורית: אוסף מצב השחקן המקומי, ממלא DTO ושולח ברשת
        private void GameLoop_Tick(object sender, object e)
        {
            try
            {
                bool isLocalLeft = _role == GameRole.Server; // אם שרת — השחקן המקומי הוא השמאלי
                var localPlayer = _manager.Scene.GetPlayer(isLocalLeft);
                if (localPlayer == null) return;

                // בונה את ה-PlayerState לשליחה
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
                    Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                    ShotFired = localPlayer.ConsumeShotFlag()
                };

                // במצב שרת: מצרף גם סטטוס גלובלי (חיים וכדורים) לשידור ללקוח
                if (_role == GameRole.Server)
                {
                    state.LeftHealth = LeftPlayerHealth.Value;
                    state.RightHealth = RightPlayerHealth.Value;
                    state.LeftBullets = _manager.GetBullets(true);
                    state.RightBullets = _manager.GetBullets(false);
                }

                // שולח את ה-state בצורה אסינכרונית
                _ = _network.SendAsync(state);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"GameLoopTick Error: {ex.Message}");
            }
        }

        // חלה כאשר השרת שולח snapshot של מחסות — יוצרת את המחסות בסצנה פעם אחת
        private void ApplyCoverSnapshot(IReadOnlyList<CoverState> covers)
        {
            if (_coversInitialized || covers == null) return;

            foreach (var c in covers)
            {
                var cover = new final_project.Objects.Covers(
                    (final_project.Objects.Covers.CoverType)c.Type,
                    c.X,
                    c.Y,
                    c.Size,
                    _manager.Scene);

                _manager.Scene.AddObject(cover);
            }

            _coversInitialized = true;
        }

        // מעדכן את מצב היריב שמתקבל מהרשת (מוחל על השחקן המתאים בסצנה)
        private void UpdateOpponentPosition(PlayerState opponentState)
        {
            if (opponentState == null) return;

            try
            {
                // מצד הלקוח היריב הוא השחקן שמאלי אם אנחנו לקוח (היפך מהלוקלי)
                bool opponentIsLeft = _role == GameRole.Client;
                var opponentPlayer = _manager.Scene.GetPlayer(opponentIsLeft);

                // אם הטיפוס הנשק השתנה או השחקן חסר — יש ליצור מחדש את השחקן כדי להתאים לספרייט/נשק
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

                // עדכון מיקום, מהירות ורוטציה לפי ה-state שהתקבל
                opponentPlayer.X = opponentState.X;
                opponentPlayer.Y = opponentState.Y;
                opponentPlayer.SpeedX = opponentState.VelocityX;
                opponentPlayer.SpeedY = opponentState.VelocityY;
                opponentPlayer.Image.Rotation = opponentState.Rotation;

                // אם נורתה ירייה בצד הנגדי — יצירת כדור משוכפל בסצנה המקומית
                if (opponentState.ShotFired)
                {
                    opponentPlayer.SpawnReplicatedBullet();

                    // בשרת: יש לעדכן גם ספירה פנימית של כדורים על השחקן המרוחק
                    if (_role == GameRole.Server)
                    {
                        opponentPlayer.ApplyRemoteShot();
                    }
                }

                // המרת מחרוזת הפעולה למצב אנימציה ועדכון הסטאטוס של השחקן המרוחק
                PlayerAnimationState remoteState;
                if (Enum.TryParse(opponentState.Action, out remoteState))
                {
                    opponentPlayer.SetState(remoteState);

                    // בשרת: אם הלקוח מבצע רענון (Reloading) יש להפעיל לוגיקה מתאימה על מופע המשוחזר
                    if (_role == GameRole.Server && remoteState == PlayerAnimationState.Reloading)
                    {
                        opponentPlayer.Reload();
                    }
                }

                // בלקוח בלבד מוצגים ערכי חיים וכדורים כפי שנשלחו מהשרת
                if (_role == GameRole.Client)
                {
                    LeftPlayerHealth.Value = opponentState.LeftHealth;
                    RightPlayerHealth.Value = opponentState.RightHealth;

                    LeftPlayerBullets.Text = opponentState.LeftBullets.ToString();
                    RightPlayerBullets.Text = opponentState.RightBullets.ToString();

                    // בדיקת ניצחון: אם חיים של אחד הגיעו לאפס — הצגת מסך ניצחון
                    if (LeftPlayerHealth.Value <= 0 || RightPlayerHealth.Value <= 0)
                    {
                        WinGrid.Visibility = Visibility.Visible;
                        WinnerTextBlock.Text = LeftPlayerHealth.Value >= RightPlayerHealth.Value
                            ? "LeftPlayerWins"
                            : "RightPlayerWins";
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"UpdateOpponentPosition Error: {ex.Message}");
            }
        }

        // כאשר צריך לשחזר שחקן יריב (לדוגמה נשק שונה) — בונה מופע חדש ומוסיף לסצנה
        private void RecreateOpponentPlayer(int typeIndex, double x, double y, bool isLeft)
        {
            WeaponProfile weapon;
            switch (typeIndex)
            {
                case 0:
                    weapon = WeaponProfile.Pistol;
                    break;
                case 1:
                    weapon = WeaponProfile.Rifle;
                    break;
                case 2:
                    weapon = WeaponProfile.Shotgun;
                    break;
                default:
                    weapon = WeaponProfile.Pistol;
                    break;
            }

            var old = _manager.Scene.GetPlayer(isLeft);
            if (old != null)
            {
                _manager.Scene.RemoveObject(old);
            }

            var player = new Player(x, y, 80, _manager.Scene, isLeft, weapon, false);
            _manager.Scene.AddObject(player);

            UpdateBullets();
        }

        // עדכון תצוגת כמות כדורים לשני השחקנים
        private void UpdateBullets()
        {
            LeftPlayerBullets.Text = _manager.GetBullets(true).ToString();
            RightPlayerBullets.Text = _manager.GetBullets(false).ToString();
        }

        // מטפל באירוע טעינה מחדש שנשלח מהמנהל — מעדכן את תצוגת הכדורים
        private void Reload(bool isLeft)
        {
            UpdateBullets();
        }

        // מטפל בירייה — מציג עדכון UI של כדורים
        private void BulletShot(bool isLeft)
        {
            UpdateBullets();
        }

        // הורדת חיים במצב שרת — עדכון מציג של חיים והצגת מסך ניצחון במקרה הצורך
        private void RemoveLives(bool isLeft, int damage)
        {
            if (_role == GameRole.Client)
            {
                return; // לקוח לא מחשב חיים — השרת מנהל את מצב החיים
            }

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
                bool winner = LeftPlayerHealth.Value >= RightPlayerHealth.Value;
                WinGrid.Visibility = Visibility.Visible;
                WinnerTextBlock.Text = winner
                    ? "LeftPlayerWins"
                    : "RightPlayerWins";
                WinnerHealthBlock.Text = winner ? "Won with " + LeftPlayerHealth.Value + " health remaining" : "Won with " + RightPlayerHealth.Value + " health remaining";
            }
        }

        // כפתור חזרה — ניתוק רשת והפסקת לולאת המשחק לפני חזרה
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
            _network?.Stop();
            _gameLoop?.Stop();
        }

        // כפתור סגירה — אותו התנהגות כמו חזרה
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
            _network?.Stop();
            _gameLoop?.Stop();
        }
    }
}