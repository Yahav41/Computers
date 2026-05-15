using final_project.GameObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace final_project.GameServices
{
    // מחלקה סטטית שבה מגדירים קבועי משחק גלובליים הניתנים לגישה מכל מקום בקוד
    public static class GameConstants
    {
        // מהירות תזוזת השחקן (פיקסלים לכל עדכון/טיק)
        public static double playerSpeed = 3;

        // מהירות התקדמות הכדור (פיקסלים לכל עדכון/טיק)
        public static double bulletSpeed = 7;

        // בחירות נשק/קונפיגורציה לשחקן השמאלי (למשל: 0=Pistol, 1=Rifle, 2=Shotgun)
        public static int leftPlayer;

        // בחירות נשק/קונפיגורציה לשחקן הימני
        public static int rightPlayer;
    }
}
