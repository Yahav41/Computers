using Newtonsoft.Json;

namespace final_project.GameServices
{
    // DTO פשוט שמייצג מצב של כיסוי/מחסה במפה לצורך סנכרון ברשת או שמירה
    public class CoverState
    {
        [JsonProperty("type")]
        public int Type { get; set; } // סוג המחסה כמספר (מ-map ל־CoverType בפרויקט)

        [JsonProperty("x")]
        public double X { get; set; } // מיקום X של המחסה על המפה

        [JsonProperty("y")]
        public double Y { get; set; } // מיקום Y של המחסה על המפה

        [JsonProperty("size")]
        public double Size { get; set; } // גודל/קוטר של המחסה (בדרך כלל רוחב התמונה)
    }
}