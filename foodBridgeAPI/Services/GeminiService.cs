using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;

namespace foodBridgeAPI.Services;

public class GeminiService : IGeminiService
{
    private readonly HttpClient _httpClient;
    private readonly GeminiSettings _settings;
    private readonly ILogger<GeminiService> _logger;

    public GeminiService(HttpClient httpClient, IOptions<GeminiSettings> settings, ILogger<GeminiService> logger)
    {
        _httpClient = httpClient;
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task<GeminiEvaluacionResponse?> EvaluarDonacionAsync(string titulo, string? descripcion, string cantidad, DateTime fechaVencimiento)
    {
        if (string.IsNullOrWhiteSpace(_settings.ApiKey) || _settings.ApiKey.StartsWith("YOUR_"))
        {
            _logger.LogWarning("Gemini ApiKey no está configurada. Se utilizará evaluación por defecto.");
            return GenerarFallback(fechaVencimiento);
        }

        try
        {
            var prompt = $@"
Eres un experto evaluador en logística de alimentos y donaciones de caridad para la plataforma FoodBridge.
Evalúa la siguiente donación de alimentos y determina su urgencia de distribución, si contiene alérgenos y si requiere cadena de frío:

- Título del alimento: {titulo}
- Descripción: {descripcion ?? "Sin descripción"}
- Cantidad: {cantidad}
- Fecha de vencimiento: {fechaVencimiento:yyyy-MM-dd HH:mm:ss} (Fecha/Hora actual UTC: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss})

Reglas para evaluar:
1. 'scoreUrgencia': Entero entre 0 y 100. Asigna un score más alto (80-100) si el alimento vence pronto o es muy perecedero (carne, lácteos, comidas preparadas). Asigna score medio (40-79) si vence en varios días. Asigna score bajo (0-39) si son enlatados o no perecederos.
2. 'contieneAlergenos': booleano. true si el alimento suele contener leche, frutos secos, gluten, huevo, mariscos, soya, etc.
3. 'requiereCadenaFrio': booleano. true si es lácteo, carne, preparado, congelado o necesita refrigeración.
4. 'recomendacionIa': Cadena breve (máximo 250 caracteres) explicando por qué se dio ese score y consejos para el transporte o entrega.
";

            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text = prompt }
                        }
                    }
                },
                generationConfig = new
                {
                    response_mime_type = "application/json",
                    response_schema = new
                    {
                        type = "OBJECT",
                        properties = new
                        {
                            scoreUrgencia = new { type = "INTEGER", description = "Score de urgencia entre 0 y 100" },
                            contieneAlergenos = new { type = "BOOLEAN", description = "Indica si el alimento contiene alérgenos conocidos" },
                            requiereCadenaFrio = new { type = "BOOLEAN", description = "Indica si requiere mantener cadena de frío o refrigeración" },
                            recomendacionIa = new { type = "STRING", description = "Recomendación breve para la distribución de la donación" }
                        },
                        required = new[] { "scoreUrgencia", "contieneAlergenos", "requiereCadenaFrio", "recomendacionIa" }
                    }
                }
            };

            var url = $"https://generativelanguage.googleapis.com/v1beta/models/{_settings.Model}:generateContent?key={_settings.ApiKey}";
            var jsonContent = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(url, jsonContent);
            if (!response.IsSuccessStatusCode)
            {
                var errorText = await response.Content.ReadAsStringAsync();
                _logger.LogError("Gemini API devolvió error {StatusCode}: {ErrorBody}", response.StatusCode, errorText);
                return GenerarFallback(fechaVencimiento);
            }

            var responseStream = await response.Content.ReadAsStreamAsync();
            using var jsonDoc = await JsonDocument.ParseAsync(responseStream);

            var root = jsonDoc.RootElement;
            if (root.TryGetProperty("candidates", out var candidates) && candidates.GetArrayLength() > 0)
            {
                var firstCandidate = candidates[0];
                if (firstCandidate.TryGetProperty("content", out var content) &&
                    content.TryGetProperty("parts", out var parts) && parts.GetArrayLength() > 0)
                {
                    var textResponse = parts[0].GetProperty("text").GetString();
                    if (!string.IsNullOrWhiteSpace(textResponse))
                    {
                        var evaluacion = JsonSerializer.Deserialize<GeminiEvaluacionResponse>(textResponse, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });
                        return evaluacion;
                    }
                }
            }

            _logger.LogWarning("No se obtuvo una respuesta válida de Gemini API. Usando fallback.");
            return GenerarFallback(fechaVencimiento);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al llamar a Gemini API. Se utilizará evaluación por defecto.");
            return GenerarFallback(fechaVencimiento);
        }
    }

    private static GeminiEvaluacionResponse GenerarFallback(DateTime fechaVencimiento)
    {
        var diasRestantes = (fechaVencimiento - DateTime.UtcNow).TotalDays;
        int score = diasRestantes switch
        {
            <= 1 => 90,
            <= 3 => 75,
            <= 7 => 55,
            _ => 30
        };

        return new GeminiEvaluacionResponse
        {
            ScoreUrgencia = score,
            ContieneAlergenos = false,
            RequiereCadenaFrio = false,
            RecomendacionIa = "Evaluación automatizada inicial por contingencia (Gemini no disponible)."
        };
    }
}
