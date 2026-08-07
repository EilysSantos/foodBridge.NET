using System.Text.Json.Serialization;

namespace foodBridgeAPI.Services;

public class GeminiEvaluacionResponse
{
    [JsonPropertyName("scoreUrgencia")]
    public int ScoreUrgencia { get; set; }

    [JsonPropertyName("contieneAlergenos")]
    public bool ContieneAlergenos { get; set; }

    [JsonPropertyName("requiereCadenaFrio")]
    public bool RequiereCadenaFrio { get; set; }

    [JsonPropertyName("recomendacionIa")]
    public string RecomendacionIa { get; set; } = string.Empty;
}
