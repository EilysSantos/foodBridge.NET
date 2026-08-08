namespace foodBridgeAPI.Services;

public interface IGroqService
{
    Task<GroqEvaluacionResponse?> EvaluarDonacionAsync(string titulo, string? descripcion, string cantidad, DateTime fechaVencimiento);
}
