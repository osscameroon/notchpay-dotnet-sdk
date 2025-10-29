using System;
using Microsoft.Extensions.Configuration;
using NotchpaySdk.Configuration;
using NotchpaySdk.Exceptions;

namespace NotchpaySdk.Extensions;

/// <summary>
/// Extension methods for <see cref="IConfiguration"/>.
/// </summary>
public static class ConfigurationExtensions
{
    /// <summary>
    /// Gets the NotchPay configuration options from the specified configuration section.
    /// </summary>
    /// <param name="configuration">The configuration.</param>
    /// <param name="sectionName">The section name. Defaults to "Notchpay".</param>
    /// <returns>The NotchPay options.</returns>
    /// <exception cref="NotchpayConfigurationException">Thrown when the configuration section is not found.</exception>
    public static NotchpayOptions GetNotchpayOptions(
        this IConfiguration configuration,
        string sectionName = NotchpayOptions.SectionName
    )
    {
        ArgumentNullException.ThrowIfNull(configuration);

        var section = configuration.GetSection(sectionName);

        if (!section.Exists())
        {
            throw new NotchpayConfigurationException(
                $"Configuration section '{sectionName}' not found. "
                    + $"Please add a '{sectionName}' section to your appsettings.json."
            );
        }

        var options =
            section.Get<NotchpayOptions>()
            ?? throw new NotchpayConfigurationException(
                $"Failed to bind configuration section '{sectionName}' to {nameof(NotchpayOptions)}."
            );
        return options;
    }
}
