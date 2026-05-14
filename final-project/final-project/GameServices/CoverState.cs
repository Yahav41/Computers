using Newtonsoft.Json;

namespace final_project.GameServices
{
    public class CoverState
    {
        [JsonProperty("type")]
        public int Type { get; set; }

        [JsonProperty("x")]
        public double X { get; set; }

        [JsonProperty("y")]
        public double Y { get; set; }

        [JsonProperty("size")]
        public double Size { get; set; }
    }
}