using System;
using System.Net;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Extensions.Http;

namespace NotchpaySdk.Http.Resilience;

/// <summary>
/// Provides resilience policies for HTTP requests to the NotchPay API.
/// </summary>
public static class NotchpayResiliencePolicies
{
    /// <summary>
    /// Creates a retry policy for transient HTTP errors.
    /// Retries on 408 Request Timeout, 429 Too Many Requests, 5xx Server Errors, and network failures.
    /// </summary>
    /// <param name="maxRetries">The maximum number of retry attempts.</param>
    /// <param name="logger">Optional logger for recording retry attempts.</param>
    /// <returns>An async policy for handling HTTP responses.</returns>
    public static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy(int maxRetries, ILogger? logger = null)
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .Or<TimeoutException>()
            .OrResult(r => r.StatusCode == HttpStatusCode.RequestTimeout)
            .OrResult(r => r.StatusCode == HttpStatusCode.TooManyRequests)
            .WaitAndRetryAsync(
                maxRetries,
                retryAttempt => CalculateDelay(retryAttempt, null),
                onRetry: (outcome, timespan, retryCount, context) =>
                {
                    logger?.LogWarning(
                        "Retry {RetryCount}/{MaxRetries} after {Delay}ms. Status: {StatusCode}",
                        retryCount,
                        maxRetries,
                        timespan.TotalMilliseconds,
                        outcome.Result?.StatusCode
                    );
                }
            );
    }

    /// <summary>
    /// Creates a timeout policy for HTTP requests.
    /// </summary>
    /// <param name="timeout">The timeout duration.</param>
    /// <returns>An async policy for handling HTTP responses.</returns>
    public static IAsyncPolicy<HttpResponseMessage> GetTimeoutPolicy(TimeSpan timeout)
    {
        return Policy.TimeoutAsync<HttpResponseMessage>(timeout);
    }

    private static TimeSpan CalculateDelay(int retryAttempt, DelegateResult<HttpResponseMessage>? result)
    {
        if (result?.Result?.Headers.RetryAfter?.Delta.HasValue == true)
        {
            return result.Result.Headers.RetryAfter.Delta.Value;
        }

        return TimeSpan.FromSeconds(Math.Pow(2, retryAttempt));
    }
}
