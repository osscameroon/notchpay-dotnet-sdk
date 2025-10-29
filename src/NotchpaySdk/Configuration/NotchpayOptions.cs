using System;
using System.ComponentModel.DataAnnotations;

namespace NotchpaySdk.Configuration;

/// <summary>
/// Configuration option for the Notchpay SDK.
/// </summary>
public sealed class NotchpayOptions
{
    /// <summary>
    /// The default configuration section name in appsettings.json.
    /// </summary>
    public const string SectionName = "Notchpay";

    private const string NotchpayBaseUrl = "https://api.notchpay.co";
    private const uint DefaultTimeOut = 30;
    private const uint DefaultMaxRetries = 3;

    /// <summary>
    /// Gets or sets the Notchpay API key (public key). <br/>
    /// Required. Must start with 'b.', 'sb.' 'pk.', or 'pk_test'.
    /// </summary>
    [Required(ErrorMessage = "API key is requierd")]
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the NotchPay private key for transfer operations.  <br/>
    /// Optional. Used for sensitive operations requiring additional authorization.
    /// </summary>
    public string? PrivateKey { get; set; }

    /// <summary>
    /// Gets or sets the Sync ID for request tracking.  <br/>
    /// Optional. Used for debugging and request correlation.
    /// </summary>
    public string? SyncId { get; set; }

    /// <summary>
    /// Gets or sets the base URL for the NotchPay API.  <br/>
    /// Defaults to 'https://api.notchpay.co'.
    /// </summary>
    [Url(ErrorMessage = "BaseUrl must be a valid URL")]
    public string BaseUrl { get; set; } = NotchpayBaseUrl;

    /// <summary>
    /// Gets or sets the request timeout duration.  <br/>
    /// Defaults to 30 seconds.
    /// </summary>
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(DefaultTimeOut);

    /// <summary>
    /// Gets or sets the maximum number of retry attempts for failed requests.  <br/>
    /// Defaults to 3. Must be greater than or equal to 0.
    /// </summary>
    public uint MaxRetries { get; set; } = DefaultMaxRetries;

    /// <summary>
    /// Gets or sets a value indicating whether telemetry collection is enabled.  <br/>
    /// Defaults to false.
    /// </summary>
    public bool EnableTelemetry { get; set; } = false;
}
