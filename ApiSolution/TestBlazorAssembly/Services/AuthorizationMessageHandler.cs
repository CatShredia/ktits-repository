using Microsoft.JSInterop;

namespace TestBlazorAssembly.Services;

public class AuthorizationMessageHandler : DelegatingHandler
{
    private readonly IJSRuntime _js;

    public AuthorizationMessageHandler(IJSRuntime js)
    {
        _js = js;
        InnerHandler = new HttpClientHandler();
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var userId = await _js.InvokeAsync<string>("localStorage.getItem", "userId");

        if (!string.IsNullOrWhiteSpace(userId))
        {
            request.Headers.Remove("X-User-Id");
            request.Headers.Add("X-User-Id", userId);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
