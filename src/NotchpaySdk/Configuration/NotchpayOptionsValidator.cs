using System;
using System.Text.RegularExpressions;
using FluentValidation;

namespace NotchpaySdk.Configuration;

/// <summary>
/// Validator for <see cref="NotchpayOptions"/> configuration.
/// </summary>
public sealed class NotchpayOptionsValidator : AbstractValidator<NotchpayOptions>
{
    private const string ApiKeyRegexPattern = @"^(b\.|sb\.|pk\.|pk_test\.)";
    private const uint MinRetries = 0;
    private const uint MaxRetriesLimit = 10;
    private const int MaxTimeoutMinutes = 5;

    private static readonly Regex ApiKeyPattern = new Regex(
        ApiKeyRegexPattern,
        RegexOptions.Compiled | RegexOptions.IgnoreCase
    );

    /// <summary>
    /// Initializes a new instance of the <see cref="NotchpayOptionsValidator"/> class.
    /// </summary>
    public NotchpayOptionsValidator()
    {
        RuleFor(x => x.ApiKey)
            .NotEmpty()
            .WithMessage("API key is required")
            .Must(HaveValidApiKeyFormat)
            .WithMessage("API key must start with 'b.', 'sb.', 'pk.', or 'pk_test.'");

        RuleFor(x => x.BaseUrl)
            .NotEmpty()
            .WithMessage("BaseUrl is required")
            .Must(BeValidUrl)
            .WithMessage("BaseUrl must be a valid URL");

        RuleFor(x => x.Timeout)
            .GreaterThan(TimeSpan.Zero)
            .WithMessage("Timeout must be greater than zero")
            .LessThanOrEqualTo(TimeSpan.FromMinutes(MaxTimeoutMinutes))
            .WithMessage($"Timeout must not exceed {MaxTimeoutMinutes} minutes");

        RuleFor(x => x.MaxRetries)
            .GreaterThanOrEqualTo(MinRetries)
            .WithMessage($"MaxRetries must be greater than or equal to {MinRetries}")
            .LessThanOrEqualTo(MaxRetriesLimit)
            .WithMessage($"MaxRetries must not exceed {MaxRetriesLimit}");
    }

    private static bool HaveValidApiKeyFormat(string apiKey)
    {
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            return false;
        }

        return ApiKeyPattern.IsMatch(apiKey);
    }

    private static bool BeValidUrl(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return false;
        }

        return Uri.TryCreate(url, UriKind.Absolute, out var uri)
            && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
    }
}
