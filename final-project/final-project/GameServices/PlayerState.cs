using Newtonsoft.Json;

namespace final_project.GameServices
{
    // DTO שמייצג את מצבו של שחקן שנשלח/מקבל ברשת (ממופה ל-JSON בעזרת Newtonsoft.Json)
    public class PlayerState
    {
        // מזהה השחקן (מספר זיהוי)
        [JsonProperty("playerId")]
        public int PlayerId { get; set; }

        // מיקום X של השחקן
        [JsonProperty("x")]
        public double X { get; set; }

        // מיקום Y של השחקן
        [JsonProperty("y")]
        public double Y { get; set; }

        // מהירות/וקטור מהירות לפי ציר X
        [JsonProperty("vx")]
        public double VelocityX { get; set; }

        // מהירות/וקטור מהירות לפי ציר Y
        [JsonProperty("vy")]
        public double VelocityY { get; set; }

        // פעולה נוכחית כ-string (למשל "move", "shoot" וכו') — שימוש לפי פרוטוקול המשחק
        [JsonProperty("action")]
        public string Action { get; set; }

        // סיבוב/רוטציה של השחקן (בדרגות) כפי שנשלח ברשת
        [JsonProperty("rotation")]
        public float Rotation { get; set; }

        // חותמת זמן (לרוב UTC ticks או millis) לצורך סינכרון/לגיטימציה של עדכונים
        [JsonProperty("timestamp")]
        public long Timestamp { get; set; }

        // סוג/טיפוס הודעה או שדה עזר כללי (מייצג כ-int לפי פרוטוקול)
        [JsonProperty("type")]
        public int Type { get; set; }

        // דגל המצביע אם ירייה בוצעה במסגרת העדכון
        [JsonProperty("shot")]
        public bool ShotFired { get; set; }

        // בריאות השחקן השמאלי (משתמשים בשדות אלה כדי לשדר מצב משחק גלובלי)
        [JsonProperty("leftHealth")]
        public double LeftHealth { get; set; }

        // בריאות השחקן הימני
        [JsonProperty("rightHealth")]
        public double RightHealth { get; set; }

        // כמות כדורים שברשות השחקן השמאלי
        [JsonProperty("leftBullets")]
        public int LeftBullets { get; set; }

        // כמות כדורים שברשות השחקן הימני
        [JsonProperty("rightBullets")]
        public int RightBullets { get; set; }

        // מימוש ToString שימושי ל־logging או דיבוג קצר של מצב השחקן
        public override string ToString()
        {
            return $"Player {PlayerId}: X={X:F2}, Y={Y:F2}, action={Action}";
        }
    }
}