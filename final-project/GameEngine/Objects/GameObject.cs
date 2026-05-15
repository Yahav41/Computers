using GameEngine.Services;
using System;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace GameEngine.Objects
{
    // מחלקת בסיס מופשטת לכל אובייקט במשחק שיש לו מיקום ותמונה על המסך
    public abstract class GameObject
    {
        // מיקום אופקי על המסך של האובייקט
        public double X { get; set; }

        // מיקום אנכי על המסך של האובייקט
        public double Y { get; set; }

        // ערך אופקי ראשוני של האובייקט
        private readonly double _initialX;

        // ערך אנכי ראשוני של האובייקט
        private readonly double _initialY;

        // התמונה הויזואלית של האובייקט שהמשתמש רואה
        public Image Image { get; }

        // האם האובייקט יכול להתנגש עם אובייקטים אחרים
        public bool Collisional { get; set; } = true;

        // מציין האם האובייקט נוצר ומוכן לשימוש ושלא יהיו בעיות או קריסות
        public bool IsCreated { get; protected set; }

        // פעולה בונה, מאתחלת את העצם
        protected GameObject(string assetPath, double x, double y, double width)
        {
            // שמירת המיקומים ההתחלתיים לצורך איפוס עתידי
            _initialX = x;
            _initialY = y;

            // קביעת המיקום הנוכחי להתחלה
            X = x;
            Y = y;

            // יצירת אובייקט תמונה עם רוחב נתון וגובה מחושב לפי יחס קבוע
            Image = new Image
            {
                Width = width,
                Height = width * 0.738
            };

            // אם קיבלנו נתיב לתמונה, הכנס אותה בתור תמונת האובייקט
            if (!string.IsNullOrEmpty(assetPath))
            {
                SetSprite(assetPath);
            }

            // עדכון מיקום התמונה בסצנה בהתאם לערכי X,Y
            Render();
        }

        // מאפס את מיקום האובייקט חזרה לנקודת ההתחלה שלו
        public void Reset()
        {
            X = _initialX;
            Y = _initialY;
            Render();
        }

        // מכניס את הנתיב שקיבלנו לתוך ערך התמונה של האובייקט
        protected void SetSprite(string relativeAssetPath)
        {
            var uri = new Uri($"ms-appx:///Assets/{relativeAssetPath}");
            Image.Source = new BitmapImage(uri);
        }

        // מעדכן את מיקום התצוגה של התמונה על המסך לפי X,Y הנוכחיים
        public virtual void Render()
        {
            Scene.SetLeft(Image, X);
            Scene.SetTop(Image, Y);
        }

        // מחזיר מלבן שמייצג את הגבולות של האובייקט לצורך בדיקות התנגשות
        public virtual Rect Bounds()
        {
            return new Rect(X, Y, Image.ActualWidth, Image.ActualHeight);
        }

        // פונקציה שנקראת כאשר יש התנגשות עם אובייקט אחר
        // מחלקות יורשות אמורות לממש כאן את ההתנהגות בזמן התנגשות
        public virtual void OnCollide(GameObject other)
        {
            // ברירת מחדל: לא עושה כלום
        }
    }
}