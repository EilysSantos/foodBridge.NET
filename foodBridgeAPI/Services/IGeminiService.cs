namespace foodBridgeAPI.Services;

public interface IGeminiService
{
    Task<GeminiEvaluacionResponse?> EvaluarDonacionAsync(string titulo, string? descripcion, string cantidad, DateTime fechaVencimiento);
}
