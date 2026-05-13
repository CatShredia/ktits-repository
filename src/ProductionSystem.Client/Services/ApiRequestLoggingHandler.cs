using System.Diagnostics;

namespace ProductionSystem.Client.Services;

/// <summary>Перед запросом и после ответа пишет в консоль (и Debug) метод, URL и успех по HTTP.</summary>
internal sealed class ApiRequestLoggingHandler : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var uri = request.RequestUri?.ToString() ?? "(no uri)";
        var start = $"[API] {request.Method.Method} {uri}";
        Console.WriteLine(start);
        Debug.WriteLine(start);

        var response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
        var ok = response.IsSuccessStatusCode ? "успех" : "ошибка";
        var end = $"[API] {request.Method.Method} {uri} -> {ok} {(int)response.StatusCode} {response.ReasonPhrase}";
        Console.WriteLine(end);
        Debug.WriteLine(end);
        return response;
    }
}
