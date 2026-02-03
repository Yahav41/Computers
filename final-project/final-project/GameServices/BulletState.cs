using Newtonsoft.Json;
using System;
namespace finalproject.GameServices
{
    public class BulletState
    { 
    [JsonProperty("bulletId")] public int BulletId { get; set; } 
    [JsonProperty("x")] public double X { get; set; } 
    [JsonProperty("y")] public double Y { get; set; } 
    [JsonProperty("angle")] public float Angle { get; set; } 
    [JsonProperty("damage")] public int Damage { get; set; } 
    [JsonProperty("playerId")] public int PlayerId { get; set; } 
    [JsonProperty("timestamp")] public long Timestamp { get; set; }
    }
}
