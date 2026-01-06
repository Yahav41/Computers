using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace final_project.GameServices
{
    public class PlayerState
    {
        [JsonProperty("playerId")]
        public int PlayerId { get; set; }

        [JsonProperty("x")]
        public double X { get; set; }

        [JsonProperty("y")]
        public double Y { get; set; }

        [JsonProperty("vx")]
        public double VelocityX { get; set; }

        [JsonProperty("vy")]
        public double VelocityY { get; set; }

        [JsonProperty("shoot")]
        public string Shoot { get; set; }

        [JsonProperty("reload")]
        public string Reload { get; set; }

        [JsonProperty("timestamp")]
        public long Timestamp { get; set; }

        public override string ToString()
        {
            return $"Player {PlayerId}: X={X:F2}, Y={Y:F2}, shoot={Shoot}, reload={Reload}";
        }
    }
}
