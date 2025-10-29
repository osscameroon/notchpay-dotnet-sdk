using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NotchpaySdk.Configuration;
using NotchpaySdk.Exceptions;
using NotchpaySdk.Http.Abstractions;
using NotchpaySdk.Models.Common;

namespace NotchpaySdk.Http;

/// <summary>
/// HTTP client implementation for communicating with the NotchPay API.
/// </summary>
public sealed class NotchpayHttpClient : INotchpayHttpClient
{
    private readonly HttpClient _httpClient;
    private readonly NotchpayOptions _options;
    private readonly ILogger<NotchpayHttpClient> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    /// <summary>
    /// Initializes a new instance of the <see cref="NotchpayHttpClient"/> class.
    /// </summary>
    /// <param name="httpClient">The HTTP client.</param>
    /// <param name="options">The NotchPay configuration options.</param>
    /// <param name="logger">The logger.</param>
    public NotchpayHttpClient(
        HttpClient httpClient,
        IOptions<NotchpayOptions> options,
        ILogger<NotchpayHttpClient> logger
    )
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
        };
    }

    /// <inheritdoc/>
    public async Task<TResponse> PostAsync<TRequest, TResponse>(
        string path,
        TRequest request,
        string? idempotencyKey = null,
        CancellationToken cancellationToken = default
    )
        where TRequest : class
        where TResponse : class
    {
        var json = JsonSerializer.Serialize(request, _jsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        using var requestMessage = new HttpRequestMessage(HttpMethod.Post, path);
        requestMessage.Content = content;

        if (!string.IsNullOrWhiteSpace(idempotencyKey))
        {
            requestMessage.Headers.Add("X-Idempotency-Key", idempotencyKey);
        }

        return await SendRequestAsync<TResponse>(requestMessage, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<TResponse> GetAsync<TResponse>(string path, CancellationToken cancellationToken = default)
        where TResponse : class
    {
        using var requestMessage = new HttpRequestMessage(HttpMethod.Get, path);
        return await SendRequestAsync<TResponse>(requestMessage, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<TResponse> PutAsync<TRequest, TResponse>(
        string path,
        TRequest request,
        CancellationToken cancellationToken = default
    )
        where TRequest : class
        where TResponse : class
    {
        var json = JsonSerializer.Serialize(request, _jsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        using var requestMessage = new HttpRequestMessage(HttpMethod.Put, path);
        requestMessage.Content = content;

        return await SendRequestAsync<TResponse>(requestMessage, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task DeleteAsync(string path, CancellationToken cancellationToken = default)
    {
        using var requestMessage = new HttpRequestMessage(HttpMethod.Delete, path);

        _logger.LogDebug("Sending DELETE request to {Path}", path);

        var response = await _httpClient.SendAsync(requestMessage, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            await HandleErrorResponseAsync(response);
        }

        _logger.LogDebug("DELETE request to {Path} completed with status {StatusCode}", path, response.StatusCode);
    }

    /// <inheritdoc/>
    public async Task<TResponse> QueryAsync<TResponse>(
        string path,
        object? queryParams = null,
        CancellationToken cancellationToken = default
    )
        where TResponse : class
    {
        var url = path;

        if (queryParams != null)
        {
            var queryString = BuildQueryString(queryParams);
            url = $"{path}?{queryString}";
        }

        using var requestMessage = new HttpRequestMessage(HttpMethod.Get, url);
        return await SendRequestAsync<TResponse>(requestMessage, cancellationToken);
    }

    private async Task<TResponse> SendRequestAsync<TResponse>(
        HttpRequestMessage requestMessage,
        CancellationToken cancellationToken
    )
        where TResponse : class
    {
        _logger.LogDebug(
            "Sending {Method} request to {Path}",
            requestMessage.Method,
            requestMessage.RequestUri?.PathAndQuery
        );

        var response = await _httpClient.SendAsync(requestMessage, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            await HandleErrorResponseAsync(response);
        }

        var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);

        _logger.LogDebug(
            "{Method} request to {Path} completed with status {StatusCode}",
            requestMessage.Method,
            requestMessage.RequestUri?.PathAndQuery,
            response.StatusCode
        );

        var result =
            JsonSerializer.Deserialize<TResponse>(responseBody, _jsonOptions)
            ?? throw new NotchpayApiException("Failed to deserialize response", (int)response.StatusCode);
        return result;
    }

    private async Task HandleErrorResponseAsync(HttpResponseMessage response)
    {
        response.Headers.TryGetValues("X-Request-Id", out var requestIdValues);
        var requestId = requestIdValues?.FirstOrDefault();

        if (requestId == null)
        {
            response.Headers.TryGetValues("Request-Id", out var altRequestIdValues);
            requestId = altRequestIdValues?.FirstOrDefault();
        }

        var statusCode = (int)response.StatusCode;
        var responseBody = await response.Content.ReadAsStringAsync();

        _logger.LogError(
            "API request failed with status {StatusCode}. Request ID: {RequestId}. Body: {ResponseBody}",
            statusCode,
            requestId,
            responseBody
        );

        ApiErrorResponse? errorResponse = null;

        try
        {
            errorResponse = JsonSerializer.Deserialize<ApiErrorResponse>(responseBody, _jsonOptions);
        }
        catch (JsonException ex)
        {
            _logger.LogWarning(ex, "Failed to deserialize error response");
        }

        var message = errorResponse?.Message ?? response.ReasonPhrase ?? "An error occurred";
        var validationErrors =
            errorResponse?.Errors != null ? new Dictionary<string, string[]>(errorResponse.Errors) : null;

        throw new NotchpayApiException(message, statusCode, validationErrors, requestId);
    }

    private string BuildQueryString(object queryParams)
    {
        var json = JsonSerializer.Serialize(queryParams, _jsonOptions);
        var dictionary = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json, _jsonOptions);

        if (dictionary == null || dictionary.Count == 0)
        {
            return string.Empty;
        }

        var queryParts = dictionary
            .Where(kvp => kvp.Value.ValueKind != JsonValueKind.Null)
            .Select(kvp =>
            {
                var value = kvp.Value.ValueKind == JsonValueKind.String ? kvp.Value.GetString() : kvp.Value.ToString();

                return $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(value ?? string.Empty)}";
            });

        return string.Join("&", queryParts);
    }
}
