using Newtonsoft.Json;

namespace final_project.GameServices
{
    public enum NetworkMessageType
    {
        PlayerState = 0,
        CoversSnapshot = 1
    }

    public class NetworkMessage
    {
        [JsonProperty("type")]
        public NetworkMessageType MessageType { get; set; }

        [JsonProperty("payload")]
        public string Payload { get; set; }
    }
}