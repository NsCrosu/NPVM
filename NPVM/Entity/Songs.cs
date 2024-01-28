using System.Text.Json.Serialization;

namespace NPVM.Entity;

[Serializable]
public class Songs
{
    [JsonPropertyName("id")] public uint Id { get; set; }
    [JsonPropertyName("title")] public string Title { get; set; } = null!;
    [JsonPropertyName("artist")] public string Artist { get; set; } = null!;
    [JsonPropertyName("length")] public string Length { get; set; } = null!;
}
