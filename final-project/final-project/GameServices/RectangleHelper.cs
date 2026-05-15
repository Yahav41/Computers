using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace final_project.GameServices
{
    public static class RectangleHelper
    {
        /// <summary>
        /// הפעולה מציירת מלבן במקום ובגודל ובצבע שקובעים
        /// </summary>
        /// <param name="scene">במה (`GameScene`) עליה לצייר את המלבן</param>
        /// <param name="x">מיקום אופקי של הפינה העליונה-שמאלית של המלבן</param>
        /// <param name="y">מיקום אנכי של הפינה העליונה-שמאלית של המלבן</param>
        /// <param name="width">רוחב המלבן</param>
        /// <param name="height">גובה המלבן</param>
        /// <param name="color">צבע המילוי של המלבן (`Windows.UI.Color`)</param>
        public static void DrawRectangle(GameScene scene, double x, double y, double width, double height, Color color)
        {
            // יוצרים אובייקט `Rectangle` ומגדירים לו רוחב, גובה וצבע מילוי
            Rectangle rectangle = new Rectangle
            {
                Width = (int)width,
                Height = (int)height,
                Fill = new SolidColorBrush(color),
            };

            // מגדירים את המיקום על ה-`Canvas` (הסצנה היא יורשת של `Canvas`)
            Canvas.SetLeft(rectangle, x);
            Canvas.SetTop(rectangle, y);

            // מחפשים אם כבר קיים מלבן קרוב מאוד למיקום כדי להחליף אותו במקום לצרף עוד אחד.
            // השוואה מבוצעת לפי מרחק אופקי ואנכי קטן מ-50 פיקסלים.
            var rect = scene.Children.FirstOrDefault(r => r is Rectangle &&
                                        Math.Abs(Canvas.GetLeft(r) - x) < 50 && Math.Abs(Canvas.GetTop(r) - y) < 50);
            if (rect != null)
            {
                // אם נמצא מלבן קיים קרוב — מסירים אותו כדי לא לצבור מלבנים חופפים
                scene.Children.Remove(rect);
            }

            // מוסיפים את המלבן החדש ל־Children של ה־`scene` כדי שיוצג ב־UI
            scene.Children.Add(rectangle);
        }
    }
}
