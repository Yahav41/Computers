using final_project.GameServices;
using GameEngine.Objects;
using GameEngine.Services;
using System;
using Windows.System;
using Windows.UI.Xaml;
using System.Threading.Tasks;
using finalproject.GameServices;

namespace final_project.GameObjects
{
    // מציין מצבים אנימציה אפשריים לשחקן
    public enum PlayerAnimationState
    {
        Idle,       // עומד
        Moving,     // נע
        Shooting,   // יורה
        Reloading   // טוען מחדש
    }

    // מחלקת השחקן שיורשת מ-GameMovingObject (תומכת בתזוזה וגבולות)
    public class Player : GameMovingObject
    {
        private readonly bool _isLeft; // האם השחקן בצד השמאלי (להבדיל מהרשת)
        private readonly GameScene _scene; // הסצנה שבה נמצא השחקן
        private readonly bool _isLocalControlled; // האם המשחק שולט בשחקן מקומית
        private readonly DispatcherTimer _fireTimer = new DispatcherTimer(); // טיימר לקצב ירי חוזר

        private float _localOffsetX = 40f; // מיקום פתח הירי יחסית למרכז בשורת X
        private float _localOffsetY = 15f; // מיקום פתח הירי יחסית למרכז בשורת Y

        public PlayerAnimationState State { get; private set; } = PlayerAnimationState.Idle; // מצב אנימציה נוכחי
        public WeaponProfile Weapon { get; private set; } // פרופיל הנשק של השחקן

        public int BulletsInMagazine { get; private set; } // כמות כדורים במגזין
        public bool CanShoot { get; private set; } = true; // האם מותר לירות כרגע

        public double AngleRad { get; private set; } // זווית בשיערות רדיאנים לשימוש בחישובים

        public bool IsLeft => _isLeft; // גישה לקריאה בלבד האם השחקן בשמאל
        private bool _isReloading; // דגל טעינה מחדש

        // קונסטרקטור שמגדיר מיקום, גודל, סצנה, צד, נשק ובקרה מקומית
        public Player(
            double x,
            double y,
            double size,
            GameScene scene,
            bool isLeft,
            WeaponProfile weapon,
            bool isLocalControlled)
            : base(string.Empty, x, y, size)
        {
            _scene = scene;
            _isLeft = isLeft;
            _isLocalControlled = isLocalControlled;
            Weapon = weapon;

            BulletsInMagazine = weapon.MagazineSize; // מילוי ראשוני של המגזין
            SetSprite(weapon.IdleSprite); // הגדרת ספרייט התחלי

            // הגדרת טיימר ירי על פי קצב הנשק
            _fireTimer.Interval = TimeSpan.FromMilliseconds(weapon.FireIntervalMs);
            _fireTimer.Tick += FireTimer_Tick;

            // מגדירים נקודת מרכז עבור סיבוב התמונה (Rotation)
            Image.CenterPoint = new System.Numerics.Vector3(
                (float)(Image.Width / 2.0),
                (float)(Image.Height / 2.0),
                0);

            // רישום למאזיני מקשים גלובליים דרך ה-Manager
            Manager.Events.OnKeyClick += OnKeyDown;
            Manager.Events.OnKeyRelease += OnKeyUp;
        }

        public int WeaponTypeIndex => (int)Weapon.Type; // אינדקס טיפוס הנשק (לממשק/רשת)

        // קריאה בכל פעם שהטיימר ירי מתיז: יוצרת כדור אם מותר
        private void FireTimer_Tick(object sender, object e)
        {
            if (!CanShoot) return;

            var rect = Bounds();
            float centerX = (float)(rect.Left + rect.Width / 2.0);
            float centerY = (float)(rect.Top + rect.Height / 2.0);

            // חישוב וקטור סיבוב לפי הזווית הנוכחית
            float cosA = (float)Math.Cos(AngleRad);
            float sinA = (float)Math.Sin(AngleRad);

            // סיבוב ההיסט ה-local כדי לקבל את נקודת המוצא של הכדור (Muzzle)
            float rotatedOffsetX = _localOffsetX * cosA - _localOffsetY * sinA;
            float rotatedOffsetY = _localOffsetX * sinA + _localOffsetY * cosA;

            float muzzleX = centerX + rotatedOffsetX;
            float muzzleY = centerY + rotatedOffsetY;

            SpawnBullet(muzzleX, muzzleY); // יצירת כדור וספירה
            OnBulletShot(); // עדכון סטייט פנימי ואירוע לרשת/ממשק
        }

        // יוצר עצם כדור ומוסיף לסצנה
        private void SpawnBullet(float muzzleX, float muzzleY)
        {
            var bullet = new Bullets(Image.Rotation, muzzleX, muzzleY, 10, _scene, Weapon.Damage);
            _scene.AddObject(bullet);
        }

        private bool _shotPendingNetwork; // דגל שמציין שיש ירייה שעדיין לא שודרה לרשת

        // קריאה כשכדור נורה: מעדכנת מספור, מצב ירי ושולחת אירוע
        private void OnBulletShot()
        {
            BulletsInMagazine--;
            if (BulletsInMagazine <= 0)
            {
                CanShoot = false;
            }

            // שולח עדכון אירוע גלובלי על ירייה (מי ירה - שמאל/ימין)
            Manager.Events.onBulletShot?.Invoke(_isLeft);

            // מסמן שיש צורך לדווח על הירייה ברשת
            _shotPendingNetwork = true;
        }

        // צריכה להיקרא על ידי הלוגיקה הרשתית לצריכת הדגל ולהחזיר אמת אם היה ירי ממתין
        public bool ConsumeShotFlag()
        {
            if (!_shotPendingNetwork) return false;
            _shotPendingNetwork = false;
            return true;
        }

        // יצירת כדור המשחזר ירייה שהגיעה מהרשת (לא מסמן אירוע רשת נוסף)
        public void SpawnReplicatedBullet()
        {
            var rect = Bounds();
            float centerX = (float)(rect.Left + rect.Width / 2.0);
            float centerY = (float)(rect.Top + rect.Height / 2.0);

            double angleRad = Image.Rotation * Math.PI / 180.0;
            float cosA = (float)Math.Cos(angleRad);
            float sinA = (float)Math.Sin(angleRad);

            float rotatedOffsetX = _localOffsetX * cosA - _localOffsetY * sinA;
            float rotatedOffsetY = _localOffsetX * sinA + _localOffsetY * cosA;

            float muzzleX = centerX + rotatedOffsetX;
            float muzzleY = centerY + rotatedOffsetY;

            var bullet = new Bullets(Image.Rotation, muzzleX, muzzleY, 10, _scene, Weapon.Damage);
            _scene.AddObject(bullet);
        }

        // חיזוק סטייט ירי שמגיע מהרשת (מעדכן כמות יריות ומפעיל אירוע)
        public void ApplyRemoteShot()
        {
            BulletsInMagazine--;
            if (BulletsInMagazine <= 0)
            {
                CanShoot = false;
            }

            Manager.Events.onBulletShot?.Invoke(_isLeft);
        }

        // טיפול באירוע KeyDown דרך ה-Manager.Events
        private void OnKeyDown(VirtualKey key)
        {
            IsCreated = true; // מסמן שהאובייקט נוצר/מפוקח
            if (!_isLocalControlled) return; // אם לא נשלט מקומית, מתעלם

            bool isLeft = _isLeft;
            if (isLeft)
            {
                // בדיקת מקשים מותאמים לשחקן השמאלי
                if (key == GameKeys.LeftPlayerLeft) MoveLeft();
                else if (key == GameKeys.LeftPlayerRight) MoveRight();
                else if (key == GameKeys.LeftPlayerUp) MoveUp();
                else if (key == GameKeys.LeftPlayerDown) MoveDown();
                else if (key == GameKeys.LeftPlayerShoot) StartShoot();
                else if (key == GameKeys.LeftPlayerReload) Reload();
            }
            else
            {
                // בדיקת מקשים לשחקן הימני
                if (key == GameKeys.RightPlayerLeft) MoveLeft();
                else if (key == GameKeys.RightPlayerRight) MoveRight();
                else if (key == GameKeys.RightPlayerUp) MoveUp();
                else if (key == GameKeys.RightPlayerDown) MoveDown();
                else if (key == GameKeys.RightPlayerShoot) StartShoot();
                else if (key == GameKeys.RightPlayerReload) Reload();
            }
        }

        // טיפול בשחרור מקש (KeyUp)
        private void OnKeyUp(VirtualKey key)
        {
            bool isLeft = _isLeft;
            if (!_isLocalControlled) return;

            if (isLeft)
            {
                if (key == GameKeys.LeftPlayerShoot)
                {
                    _fireTimer.Stop(); // הפסקת הירי החוזר כשמשתחרר מקש הירי
                }

                if (key == GameKeys.LeftPlayerLeft || key == GameKeys.LeftPlayerRight)
                {
                    SpeedX = 0; // עצירת תנועה אופקית כשמשתחרר כפתור תנועה אופקי
                }

                if (key == GameKeys.LeftPlayerUp || key == GameKeys.LeftPlayerDown)
                {
                    SpeedY = 0; // עצירת תנועה אנכית
                }
            }
            else
            {
                if (key == GameKeys.RightPlayerShoot)
                {
                    _fireTimer.Stop();
                }

                if (key == GameKeys.RightPlayerLeft || key == GameKeys.RightPlayerRight)
                {
                    SpeedX = 0;
                }

                if (key == GameKeys.RightPlayerUp || key == GameKeys.RightPlayerDown)
                {
                    SpeedY = 0;
                }
            }

            // אם הפסיקה התנועה והלא בטעינה מחדש - נחזיר סטייט ל-Idle
            if (SpeedX == 0 && SpeedY == 0 && !_isReloading)
            {
                SetState(PlayerAnimationState.Idle);
            }
        }

        // פונקציות תנועה שמעדכנות מהירות ומצב אנימציה
        private void MoveLeft()
        {
            SpeedX = -GameConstants.playerSpeed;
            SetState(PlayerAnimationState.Moving);
        }

        private void MoveRight()
        {
            SpeedX = GameConstants.playerSpeed;
            SetState(PlayerAnimationState.Moving);
        }

        private void MoveUp()
        {
            SpeedY = -GameConstants.playerSpeed;
            SetState(PlayerAnimationState.Moving);
        }

        private void MoveDown()
        {
            SpeedY = GameConstants.playerSpeed;
            SetState(PlayerAnimationState.Moving);
        }

        // התחלת רצף ירי: מאפס מהירות ומפעיל טיימר ירי אם מותר
        private void StartShoot()
        {
            if (!CanShoot) return;

            SpeedX = SpeedY = 0;
            SetState(PlayerAnimationState.Shooting);
            _fireTimer.Start();
        }

        // פעולת טעינה מחדש אסינכרונית: חוסמת ירי לזמן הטעינה, מאפס מהירות ומעדכן אירוע בסיום
        public async void Reload()
        {
            if (_isReloading || BulletsInMagazine == Weapon.MagazineSize)
                return;

            _isReloading = true;
            CanShoot = false;
            SpeedX = SpeedY = 0;

            _fireTimer.Stop();

            SetState(PlayerAnimationState.Reloading);

            await Task.Delay(Weapon.ReloadDurationMs); // המתנה מדומה לפי משך טעינה

            BulletsInMagazine = Weapon.MagazineSize;
            CanShoot = true;
            _isReloading = false;

            SetState(PlayerAnimationState.Idle);

            Manager.Events.onReload?.Invoke(_isLeft); // הודעה למאזינים שהטען הושלם
        }

        // מעביר בין מצבי האנימציה ומשנה ספרייט בהתאם
        public void SetState(PlayerAnimationState newState)
        {
            if (State == newState) return;
            State = newState;

            switch (State)
            {
                case PlayerAnimationState.Idle:
                    SetSprite(Weapon.IdleSprite);
                    break;
                case PlayerAnimationState.Moving:
                    SetSprite(Weapon.WalkSprite);
                    break;
                case PlayerAnimationState.Shooting:
                    SetSprite(Weapon.ShootSprite);
                    break;
                case PlayerAnimationState.Reloading:
                    SetSprite(Weapon.ReloadSprite);
                    break;
            }
        }

        // קריאה לכל טיק רינדור: מעדכנת רוטציה והגבולות כדי שלא יצא מהמגרש
        public override void Render()
        {
            base.Render();

            AngleRad = Image.Rotation * Math.PI / 180.0;

            // תיקוני גבולות כדי לשמור את השחקן dentro המסך
            if (X < 0) X = 10;
            if (X > 1150 - Image.Width) X = 1150 - Image.Width - 10;
            if (Y < 0) Y = 10;
            if (Y > 475 - Image.Height) Y = 475 - Image.Height - 10;
        }

        // טיפול בהתנגשות: אם קיבלנו כדור - עדכון חיים והסרת הכדור מהסצנה
        public override void OnCollide(GameObject other)
        {
            if (other is Bullets bullet)
            {
                Manager.Events.OnRemoveLifes?.Invoke(_isLeft, bullet.Damage);
                _scene.RemoveObject(bullet);
            }
        }
    }
}