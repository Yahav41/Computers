
using GameEngine.Objects;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace GameEngine.Services
{
    // מחלקה מופשטת שמייצגת סצנה במשחק; יורשת מ-Canvas כדי לאחסן ולהציג תמונות של עצמים
    public abstract class Scene : Canvas
    {
        // רשימה פנימית של כל ה-GameObject בסצנה
        private readonly List<GameObject> _gameObjects = new List<GameObject>();

        // Snapshot קריאה בלבד שמחזירה עותק של הרשימה כדי למנוע שינויים בזמן איטרציה
        public IEnumerable<GameObject> GameObjectsSnapshot => _gameObjects.ToList();

        protected Scene()
        {
            // אתחול (OnRun) של ה-Manager.
            // קודם מאפס כדי להבטיח שלא יהיו מאזינים כפולים, ואז מוסיף שני מאזינים:
            // OnTick שמרנדר תזוזות ו-CheckCollisions שבודק התנגשויות.
            Manager.Events.OnRun = null;
            Manager.Events.OnRun += OnTick;
            Manager.Events.OnRun += CheckCollisions;
        }

        // מוסיף אובייקט למשחק: לרשימה ול-Canvas (להציג את ה-Image שלו)
        public void AddObject(GameObject obj)
        {
            _gameObjects.Add(obj);
            Children.Add(obj.Image);
        }

        // מסיר אובייקט גם מהרשימה וגם מה-Canvas; אם לא נמצא - לא עושה כלום
        public void RemoveObject(GameObject obj)
        {
            if (!_gameObjects.Remove(obj)) return;
            Children.Remove(obj.Image);
        }

        // מסיר את כל האובייקטים בעזרת עותק של הרשימה כדי לא לבצע שינויים תוך איטרציה על המקור
        public void RemoveAll()
        {
            foreach (var obj in _gameObjects.ToList())
            {
                RemoveObject(obj);
            }
        }

        // מאפס כל אובייקט: קורא לפונקציית Reset של כל GameObject
        public void ResetAll()
        {
            foreach (var obj in _gameObjects)
            {
                obj.Reset();
            }
        }

        // מטפל באירוע טיק/ריצה: מרנדר עצמים נעים בלבד
        private void OnTick()
        {
            foreach (var obj in GameObjectsSnapshot)
            {
                // בדיקה אם האובייקט הוא GameMovingObject ואז קורא ל-Render שלו
                if (obj is GameMovingObject moving)
                {
                    moving.Render();
                }
            }
        }

        // בודק התנגשויות בין עצמים קוליזיוניים
        private void CheckCollisions()
        {
            foreach (var obj in GameObjectsSnapshot)
            {
                if (!obj.Collisional) continue; // מדלג על עצמים שלא משתתפים בקוליזיות

                // מחפש אובייקט אחר שמצטלב עם האובייקט הנוכחי.
                // השימוש ב-GameObjectsSnapshot וב-ReferenceEquals מונע בדיקה מול אותו מופע.
                // RectHelper.Intersect בודק חיתוך בין הגבולות (Bounds) של שני האובייקטים.
                var other = GameObjectsSnapshot.FirstOrDefault(o =>
                    !ReferenceEquals(o, obj)
                    && o.Collisional
                    && !RectHelper.Intersect(o.Bounds(), obj.Bounds()).IsEmpty);

                if (other != null)
                {
                    // קורא להתנהגות ההתנגשות של האובייקט כשנמצאה התנגשות
                    obj.OnCollide(other);
                }
            }
        }
    }
}