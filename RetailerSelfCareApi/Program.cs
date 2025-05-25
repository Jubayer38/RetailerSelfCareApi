///************************************************************************
///	|| Creation History ||
///-----------------------------------------------------------------------
///	Copyright     :	Copyright© NAAS Solutions Limited. All rights reserved.
///	Author	      :	Arafat Hossain
///	Purpose	      :	An entry point of an application
///	Creation Date :	13-Dec-2023
/// =======================================================================
///  || Modification History ||
///  ----------------------------------------------------------------------
///  Sl No.	Date:		    Author:			    Ver:	    Area of Change:
///  1.     
///	 ----------------------------------------------------------------------
///	***********************************************************************

using Domain.StaticClass;
using RetailerSelfCareApi;
using System.Reflection;
using System.Runtime.InteropServices;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel((options) =>
{
    // Prevent runaway FD growth
    options.Limits.MaxConcurrentConnections = 50000;
    options.Limits.MaxConcurrentUpgradedConnections = 10000;

    // Idle timeout — align with Nginx keepalive_timeout
    options.Limits.KeepAliveTimeout = TimeSpan.FromSeconds(15);
});

bool isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

builder.Configuration.AddJsonFile(
                    $"appsettings.Production.json",
                    optional: true,
                    reloadOnChange: true
);

builder.Configuration.GetSection("DBConnectionStrings").Get<Connections>();
builder.Configuration.GetSection("Authentication").Get<JweKeysModel>();
builder.Configuration.GetSection("TextLogging").Get<TextLogging>();
builder.Configuration.GetSection("AppAllowedVersion").Get<AppAllowedVersion>();
builder.Configuration.GetSection("ResponseMessages").Get<ResponseMessages>();
builder.Configuration.GetSection("FeatureStatus").Get<FeatureStatus>();
builder.Configuration.GetSection("ExternalKeys").Get<ExternalKeys>();
builder.Configuration.GetSection("BiometricKeys").Get<BiometricKeys>();
builder.Configuration.GetSection("LMSKyes").Get<LMSKyes>();
builder.Configuration.GetSection("EmailKeys").Get<EmailKeys>();
builder.Configuration.GetSection("AppSettingsKeys").Get<AppSettingsKeys>();

ConfigDecryptor.DecryptConnectionString();
ConfigDecryptor.DecryptResponseMessages();
ConfigDecryptor.DeecryptExternalKeysString();
ConfigDecryptor.DecryptBiometricKeysString();
ConfigDecryptor.DecryptLMSKyesString();
ConfigDecryptor.DecryptEmailKeysString();

if (isWindows)
{
    TextLogging.TextLogPath = "D:\\RetailerAPILog\\" + TextLogging.ApplicationTitle;
    AppSettingsKeys.IsWindows = true;
    ExternalKeys.ImagePhisycalDirPath = "D:\\Published\\5.RetailerAppImages\\";
}
else
{
    string fullPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
    int lastSeparatorIndex = fullPath.LastIndexOf(Path.DirectorySeparatorChar);
    TextLogging.ApplicationTitle = fullPath.Substring(lastSeparatorIndex + 1);

    TextLogging.TextLogPath = "/data/datalogs/applicationLogs/" + TextLogging.ApplicationTitle;
    ExternalKeys.ImagePhisycalDirPath = "/data/datalogs/published/RetailerAppImages/";
}

// Add services to the container.
IServiceCollection serviceCollection = builder.Services;
Startup.ConfigureServices(serviceCollection, isWindows);
builder.Logging.ClearProviders();
var app = builder.Build();

//app.UseSerilogRequestLogging();

// Configure the HTTP request pipeline.
IWebHostEnvironment webHostEnvironment = app.Environment;
Startup.ConfigureMethod(app, webHostEnvironment, isWindows);

app.Run();
