using final_project.GameObjects;
using GameEngine.Objects;
using GameEngine.Services;
using System;
using System.Numerics;
using Windows.Foundation;

namespace final_project.GameServices
{
    // סצנה מותאמת למשחק — יורשת מ־`Scene` ומוסיפה לוגיקה ספציפית לשחקנים
    public class GameScene : Scene
    {
        public GameScene() : base()
        {
            // בריצה של המשחק נוסיף מאזין שמסובב את השחקנים בכל טיק
            Manager.Events.OnRun += RotatePlayers;
        }

        // סובב את השחקנים כך שיעמדו/יתבוננו זה לעבר זה עם זווית מטעמי רקע/אסתטיקה
        private void RotatePlayers()
        {
            var leftPlayer = GetPlayer(true);   // מקבל את השחקן השמאלי דרך `GetPlayer`
            var rightPlayer = GetPlayer(false); // מקבל את השחקן הימני

            // אם אחד מהשחקנים חסר — אין מה לעשות
            if (leftPlayer == null || rightPlayer == null)
                return;

            // מחשבים את מרכז הגבולות של כל שחקן (כדי לסובב סביב מרכזם)
            Point center1 = new Point(
                rightPlayer.Bounds().Left + rightPlayer.Bounds().Width / 2,
                rightPlayer.Bounds().Top + rightPlayer.Bounds().Height / 2);

            Point center2 = new Point(
                leftPlayer.Bounds().Left + leftPlayer.Bounds().Width / 2,
                leftPlayer.Bounds().Top + leftPlayer.Bounds().Height / 2);

            // הבדל במיקום בין המרכזים ושיוך זווית (במעלות) באמצעות Atan2
            double dx = center1.X - center2.X;
            double dy = center1.Y - center2.Y;
            double angle = Math.Atan2(dy, dx) * 180 / Math.PI;

            // מרחק בין השחקנים
            double distance = Math.Sqrt(dx * dx + dy * dy);
            // חישוב זווית-תיקון (offset) המבוססת על מרחק — במטרה לשנות מעט את הזווית להצגה
            double offsetAngle = Math.Atan2(45.9, distance) * 180 / Math.PI;

            // הגדרת רוטציה לכל שחקן עם התחשבות ב־offset ובכיוון הכללי
            leftPlayer.Image.Rotation = (float)(angle - offsetAngle + 10);
            rightPlayer.Image.Rotation = (float)(angle + 180 - offsetAngle + 10);
        }

        // מחזיר את ה־`Player` שמתאים לצד המבוקש (`isLeft`), או `null` אם לא נמצא
        public Player GetPlayer(bool isLeft)
        {
            // משתמש ב־`GameObjectsSnapshot` (עותק בטוח של הרשימה) כדי לא לבצע שינויים תוך איטרציה
            foreach (GameObject obj in GameObjectsSnapshot)
            {
                if (obj is Player player && player.IsLeft == isLeft)
                    return player;
            }
            return null;
        }
    }
}