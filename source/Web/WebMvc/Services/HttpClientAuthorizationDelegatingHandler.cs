using Microsoft.AspNetCore.Authentication;
using System.Net.Http.Headers;

namespace EShopOnContainers.WebMvc.Services;

/// <summary>
/// Un <see cref="DelegatingHandler"/> que agrega un encabezado de autorización a las solicitudes HTTP.
/// </summary>
public class HttpClientAuthorizationDelegatingHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _httpContextAccesor;
    private readonly ILogger<HttpClientAuthorizationDelegatingHandler> _logger;

    /// <summary>
    /// Inicializa una nueva instancia de la clase <see cref="HttpClientAuthorizationDelegatingHandler"/>
    /// </summary>
    /// <param name="httpContextAccesor">El contexto HTTP actual de la solicitud.</param>
    /// <param name="logger">Registro de ejecución de la clase.</param>
    public HttpClientAuthorizationDelegatingHandler(IHttpContextAccessor httpContextAccesor, ILogger<HttpClientAuthorizationDelegatingHandler> logger)
    {
        _httpContextAccesor = httpContextAccesor;
        _logger = logger;
    }

    /// <summary>
    /// Maneja la solicitud HTTP delegada, agregando un encabezado de autorización si está presente en el contexto HTTP.
    /// </summary>
    /// <param name="request">La solicitud HTTP que se va a procesar.</param>
    /// <param name="cancellationToken">El token de cancelación asociado con la solicitud.</param>
    /// <returns>
    /// Una <see cref="Task"/> que representa la operación asincrónica y contiene la respuesta HTTP.
    /// </returns>
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // Recuperar el encabezado de autorización del contexto HTTP
        var authorizationHeader = _httpContextAccesor.HttpContext
            .Request.Headers["Authorization"];

        if (!string.IsNullOrEmpty(authorizationHeader))
        {
            // Agregar el encabezado de autorización a la solicitud HTTP
            request.Headers.Add("Authorization", new List<string>() { authorizationHeader });
        }

        var token = await GetToken();

        if (token != null)
        {
            // Agregar el encabezado de autorización a la solicitud HTTP
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        return await base.SendAsync(request, cancellationToken);
    }

    /// <summary>
    /// Recupera un token de autenticación de manera asincrónica.
    /// </summary>
    /// <returns>
    /// Una <see cref="Task"/> que representa la operación asincrónica y contiene el token de autenticación.
    /// </returns>
    private async Task<string> GetToken()
    {
        const string ACCESS_TOKEN = "access_token";

        return (await _httpContextAccesor.HttpContext!.GetTokenAsync(ACCESS_TOKEN))!;
    }
}
