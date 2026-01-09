// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.Logging;
using Moryx.Drivers.OpcUa.Exceptions;
using Opc.Ua;
using Opc.Ua.Configuration;

namespace Moryx.Drivers.OpcUa.Factories;

internal class ApplicationConfigurationFactory
{
    private ILogger _logger;

    public string ApplicationName { get; set; } = "";

    public virtual async Task<ApplicationConfiguration> Create(ILogger logger, string configPath = null, CancellationToken cancellationToken = default)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        var application = new ApplicationInstance
        {
            ApplicationName = ApplicationName,
            ApplicationType = ApplicationType.Client,
            ConfigSectionName = "Moryx.OpcUa.Client",
        };

        ApplicationConfiguration config;
        var defaultPath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "Config\\Opc.Ua.Default.Config.xml"));
        var filePath = string.IsNullOrEmpty(configPath) ? defaultPath : configPath;
        try
        {
            config = await application.LoadApplicationConfigurationAsync(filePath, false, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError("{Message}", ex.Message);
            return null;
        }

        ApplicationName = config.ApplicationName;
        // check the application certificate
        var haveAppCertificate = await application.CheckApplicationInstanceCertificatesAsync(false, 0, cancellationToken);
        if (!haveAppCertificate)
        {
            throw new InvalidCertificateException("Application instance certificate invalid!");
        }
        else
        {
            config.ApplicationUri = X509Utils.GetApplicationUriFromCertificate(config.SecurityConfiguration.ApplicationCertificate.Certificate);
            config.CertificateValidator.CertificateValidation += CertificateValidatorCertificateValidation;
        }
        return config;
    }

    private void CertificateValidatorCertificateValidation(CertificateValidator validator, CertificateValidationEventArgs e)
    {
        _logger.Log(LogLevel.Error, "{StatusCode}", e.Error.StatusCode);
        if (e.Error.StatusCode == StatusCodes.BadCertificateUntrusted)
        {
            if (validator.AutoAcceptUntrustedCertificates)
            {
                e.Accept = true;
                if (validator.AutoAcceptUntrustedCertificates)
                {
                    _logger.Log(LogLevel.Information, "Accepted Certificate: {Subject}",
                    e.Certificate.Subject);
                }
                else
                {
                    _logger.Log(LogLevel.Information, "Rejected Certificate: {Subject}",
                    e.Certificate.Subject);
                }
            }
        }
    }
}
