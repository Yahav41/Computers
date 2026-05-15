using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace final_project.GameServices
{
    // ממשק לתקשורת רשת במשחק — מגדיר אירועים ופונקציות שחייבים להתממש על ידי מימוש רשת (שרת/לקוח)
    public interface IGameNetwork
    {
        // אירוע שמתרחש כשהתקבלה עדכון מצב של היריב (DTO מסוג `PlayerState`)
        event Action<PlayerState> OpponentStateReceived;

        // אירוע שמתרחש כשהתקבלה תמונת מצב של המחסות (רשימת `CoverState`)
        event Action<IReadOnlyList<CoverState>> CoversReceived;

        // אירוע שינויים במצב החיבור/סטטוס (למשל: "Connected", "Disconnected", "Error")
        event Action<string> StatusChanged;

        // מתחיל או מתחבר ל-IP מסוים; אם הפרמטר הוא null — כנראה מתחיל כמארח/שרת
        Task StartOrConnectAsync(string ipOrNull);

        // שולח את מצב השחקן הנוכחי לרשת (לרוב לשאר המשתתפים)
        Task SendAsync(PlayerState state);

        // שולח snapshot של המחסות לרשת (משתמש ל-sync התחלי או עדכון)
        Task SendCoversAsync(IReadOnlyList<CoverState> covers);

        // מפסיק את החיבור ואת פעולות הרשת
        void Stop();

        // מצביע אם יש חיבור פעיל כרגע
        bool IsConnected { get; }
    }
}