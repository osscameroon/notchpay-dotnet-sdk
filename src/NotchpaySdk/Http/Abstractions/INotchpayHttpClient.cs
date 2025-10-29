using System.Threading;
using System.Threading.Tasks;

namespace NotchpaySdk.Http.Abstractions;

/// <summary>
/// Defines HTTP client operations for communicating with the NotchPay API.
/// </summary>
public interface INotchpayHttpClient
{
    /// <summary>
    /// Sends a POST request to the specified path.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request body.</typeparam>
    /// <typeparam name="TResponse">The type of the response.</typeparam>
    /// <param name="path">The relative path to send the request to.</param>
    /// <param name="request">The request body.</param>
    /// <param name="idempotencyKey">Optional idempotency key for the request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The deserialized response.</returns>
    Task<TResponse> PostAsync<TRequest, TResponse>(
        string path,
        TRequest request,
        string? idempotencyKey = null,
        CancellationToken cancellationToken = default
    )
        where TRequest : class
        where TResponse : class;

    /// <summary>
    /// Sends a GET request to the specified path.
    /// </summary>
    /// <typeparam name="TResponse">The type of the response.</typeparam>
    /// <param name="path">The relative path to send the request to.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The deserialized response.</returns>
    Task<TResponse> GetAsync<TResponse>(string path, CancellationToken cancellationToken = default)
        where TResponse : class;

    /// <summary>
    /// Sends a PUT request to the specified path.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request body.</typeparam>
    /// <typeparam name="TResponse">The type of the response.</typeparam>
    /// <param name="path">The relative path to send the request to.</param>
    /// <param name="request">The request body.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The deserialized response.</returns>
    Task<TResponse> PutAsync<TRequest, TResponse>(
        string path,
        TRequest request,
        CancellationToken cancellationToken = default
    )
        where TRequest : class
        where TResponse : class;

    /// <summary>
    /// Sends a DELETE request to the specified path.
    /// </summary>
    /// <param name="path">The relative path to send the request to.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task DeleteAsync(string path, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a GET request with query parameters to the specified path.
    /// </summary>
    /// <typeparam name="TResponse">The type of the response.</typeparam>
    /// <param name="path">The relative path to send the request to.</param>
    /// <param name="queryParams">The query parameters to include in the URL.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The deserialized response.</returns>
    Task<TResponse> QueryAsync<TResponse>(
        string path,
        object? queryParams = null,
        CancellationToken cancellationToken = default
    )
        where TResponse : class;
}
