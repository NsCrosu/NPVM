using System.Text.Json.Serialization;

namespace NPVM.Entity;

[Serializable]
public class SongList
{
    [JsonPropertyName("songs")] public Songs[]? Songs { get; set; }
}
