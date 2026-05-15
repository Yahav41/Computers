using System;
using Windows.UI.Xaml.Controls;

namespace GameEngine.Objects
{
    //  מחלקה אבסטרקטית שמייצגת אובייקט במשחק שיכול לנוע באמצעות מאפייני תנועה, מהירות ותאוצה. יורשת מ-GameObject
    public abstract class GameMovingObject : GameObject
    {
        // מהירות בציר האופקי - פיקסלים לפריים או יחידות פר-עדכון
        public double SpeedX { get; set; }
        // מהירות בציר האנכי - פיקסלים לפריים או יחידות פר-עדכון
        public double SpeedY { get; set; }

        // התאוצה בציר אנכי - שמורה כמוגנת כדי שמחלקות יורשות יוכלו לשנות
        protected double AccelerationX { get; set; }
        // התאוצה בציר אופקי - שמורה כמוגנת כדי שמחלקות יורשות יוכלו לשנות
        protected double AccelerationY { get; set; }

        // קונסטרקטור שמקבל נתיב קובץ תמונה, מיקום וגודל.
        // (קורא לקונסטרקטור הבסיסי ומאתחל את האובייקט כעוצר (מהירויות = 0.
        protected GameMovingObject(string fileName, double x, double y, double size)
            : base(fileName, x, y, size)
        {
            Stop(); // מקפיא את התנועה בהתחלה
        }

        // עדכון ומעבר למצב התצוגה: מעדכן מיקום ומהירויות, ואז מצייר באמצעות ההיגיון של המחלקה הבסיסית.
        public override void Render()
        {
            // זז לפי מהירות נוכחית
            X += SpeedX;
            Y += SpeedY;

            // (מעדכן מהירות לפי התאוצה (אינטגרציה פשוטה ללא זמן מדויק
            SpeedX += AccelerationX;
            SpeedY += AccelerationY;

            // קריאה ל-Render של המחלקה הבסיסית לביצוע שאר פעולות הרינדור
            base.Render();
        }

        // מאפס מהירויות והתאצות - משמש לעצירה מיידית של האובייקט.
        public void Stop()
        {
            SpeedX = 0;
            SpeedY = 0;
            AccelerationX = 0;
            AccelerationY = 0;
        }
    }
}