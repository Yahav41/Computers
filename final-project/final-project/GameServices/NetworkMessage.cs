using Newtonsoft.Json;

namespace final_project.GameServices
{
    // סוגי הודעות שמועברות ברשת בין לקוח לשרת
    public enum NetworkMessageType
    {
        PlayerState = 0,   // הודעה שמכילה מצב שחקן (PlayerState)
        CoversSnapshot = 1 // הודעה שמכילה תמונת מצב של המחסות (רשימת CoverState)
    }

    // מבנה כללי של הודעה ברשת: סוג וה־payload כ-string (בפורמט JSON)
    public class NetworkMessage
    {
        [JsonProperty("type")]
        public NetworkMessageType MessageType { get; set; } // סוג ההודעה לשם פרשנות ה-payload

        [JsonProperty("payload")]
        public string Payload { get; set; } // מחרוזת JSON שמכילה את הנטען (לדוגמה: PlayerState או CoverState[])
    }
}