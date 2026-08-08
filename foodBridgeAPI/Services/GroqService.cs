using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;

namespace foodBridgeAPI.Services;

public class GroqService : IGroqService
{
    private readonly HttpClient _httpClient;
    private readonly GroqSettings _settings;
    private readonly ILogger<GroqService> _logger;

    public GroqService(HttpClient httpClient, IOptions<GroqSettings> settings, ILogger<GroqService> logger)
    {
        _httpClient = httpClient;
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task<GroqEvaluacionResponse?> EvaluarDonacionAsync(string titulo, string? descripcion, string cantidad, DateTime fechaVencimiento)
    {
        if (string.IsNullOrWhiteSpace(_settings.ApiKey))
        {
            _logger.LogWarning("Groq ApiKey no está configurada. Usando evaluación por defecto.");
            return GenerarFallback(fechaVencimiento);
        }

        try
        {
            var systemMessage = @"
Eres un experto evaluador de donaciones de alimentos para FoodBridge.
DEBES responder SIEMPRE únicamente en formato JSON con la siguiente estructura exacta y nombres de propiedad:
{
  ""scoreUrgencia"": (entero de 0 a 100 indicando prioridad de distribución según cercanía a vencimiento y tipo de alimento),
  ""contieneAlergenos"": (booleano true si contiene leche, gluten, maní, huevo, soya, etc.),
  ""requiereCadenaFrio"": (booleano true si es lácteo, carne, preparado o necesita refrigeración),
  ""recomendacionIa"": (cadena de texto en español breve con explicación y recomendaciones logísticas)
}";

            var userMessage = $@"
Evalúa esta donación:
- Título: {titulo}
- Descripción: {descripcion ?? "Sin descripción"}
- Cantidad: {cantidad}
- Fecha de vencimiento: {fechaVencimiento:yyyy-MM-dd HH:mm:ss} (Fecha actual UTC: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss})
";

            var requestBody = new
            {
                model = _settings.Model,
                messages = new[]
                {
                    new { role = "system", content = systemMessage },
                    new { role = "user", content = userMessage }
                },
                response_format = new { type = "json_object" },
                temperature = 0.2
            };

            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.groq.com/openai/v1/chat/completions");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _settings.ApiKey);
            request.Content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                var errorText = await response.Content.ReadAsStringAsync();
                _logger.LogError("Groq API devolvió error {StatusCode}: {ErrorBody}", response.StatusCode, errorText);
                return GenerarFallback(fechaVencimiento);
            }

            var responseStream = await response.Content.ReadAsStreamAsync();
            using var jsonDoc = await JsonDocument.ParseAsync(responseStream);

            var root = jsonDoc.RootElement;
            if (root.TryGetProperty("choices", out var choices) && choices.GetArrayLength() > 0)
            {
                var firstChoice = choices[0];
                if (firstChoice.TryGetProperty("message", out var message) &&
                    message.TryGetProperty("content", out var content))
                {
                    var textResponse = content.GetString();
                    if (!string.IsNullOrWhiteSpace(textResponse))
                    {
                        var evaluacion = JsonSerializer.Deserialize<GroqEvaluacionResponse>(textResponse, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });
                        return evaluacion;
                    }
                }
            }

            _logger.LogWarning("Respuesta de Groq API no contiene formato esperado.");
            return GenerarFallback(fechaVencimiento);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al llamar a Groq API. Se usará evaluación por defecto.");
            return GenerarFallback(fechaVencimiento);
        }
    }

    private static GroqEvaluacionResponse GenerarFallback(DateTime fechaVencimiento)
    {
        var diasRestantes = (fechaVencimiento - DateTime.UtcNow).TotalDays;
        int score = diasRestantes switch
        {
            <= 1 => 90,
            <= 3 => 75,
            <= 7 => 55,
            _ => 30
        };

        return new GroqEvaluacionResponse
        {
            ScoreUrgencia = score,
            ContieneAlergenos = false,
            RequiereCadenaFrio = false,
            RecomendacionIa = "Evaluación automatizada inicial por contingencia (Groq API no disponible)."
        };
    }
}
