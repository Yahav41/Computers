using final_project.GameServices;
using final_project.Objects;
using GameEngine.Objects;
using System;
using System.Threading.Tasks;

namespace final_project.GameObjects
{
    // מחלקת כדור שיורשת מ-GameMovingObject (תומכת בתנועה ועדכון מיקום)
    public class Bullets : GameMovingObject
    {
        private readonly GameScene _scene; // הסצנה שבה הכדור קיים

        public int Damage { get; } // הנזק שהכדור גורם

        // קונסטרקטור: זווית ברוטציה (degrees), מיקום התחלתי, גודל, הסצנה ונזק
        public Bullets(float angle, double x, double y, double size, GameScene scene, int damage)
            : base(string.Empty, x, y, size)
        {
            Collisional = false; // בתחילת החיים לא יגרום להתנגשויות (מניעת פגיעה מיידית עם היורה)
            _scene = scene;

            SetSprite("Models/Bullet/bullet.png"); // הגדרת תמונת הכדור

            // מרכז הסיבוב של התמונה מוגדר למרכז התמונה
            Image.CenterPoint = new System.Numerics.Vector3(
                (float)(Image.Width / 2.0),
                (float)(Image.Height / 2.0),
                0);

            Image.Rotation = angle + 90; // כיוון הגרפי של הכדור (התאמה ל-sprite)

            SetSpeed(angle); // קביעת מהירות על פי הזווית
            _ = ActivateCollisionAsync(); // מאתחל את ההפעלה של הקוליזיות (לא חוסם)

            Damage = damage; // שמירת הערך של הנזק
        }

        // אחרי השהייה קצרה, מאפשר לכדור להיות קוליזיוני (מניעת התנגשות מידית עם היורה)
        private async Task ActivateCollisionAsync()
        {
            await Task.Delay(100);
            Collisional = true;
        }

        // חישוב מהירות לפי הזווית (במעלות) והצבת מהירות X/Y
        private void SetSpeed(double angle)
        {
            var radians = angle * (Math.PI / 180.0);
            SpeedX = Math.Cos(radians) * GameConstants.bulletSpeed;
            SpeedY = Math.Sin(radians) * GameConstants.bulletSpeed;
            IsCreated = true; // מסמן שהכדור נמצא בשטח ומתחיל להתעדכן
        }

        // בכל רינדור בודק אם הכדור יצא מהמסך — אם כן, מסיר אותו מהסצנה
        public override void Render()
        {
            base.Render();

            if (X < 0 || X > 1150 || Y < 0 || Y > 475)
            {
                _scene.RemoveObject(this);
            }
        }

        // טיפול בהתנגשות: אם הכדור מתנגש ב-Covers (מחסה) — מסיר את הכדור
        public override void OnCollide(GameObject g)
        {
            if (g is Covers)
            {
                _scene.RemoveObject(this);
            }
        }
    }
}