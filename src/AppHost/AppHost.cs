var builder = DistributedApplication.CreateBuilder(args);

var appConfiguration = builder.AddAzureAppConfiguration("appconfiguration");

if (builder.ExecutionContext.IsRunMode)
{
    appConfiguration.RunAsEmulator(emulator =>
    {
        // Aspire 13.5.3 defaults to 1.0.2, which has no linux/arm64 manifest.
        emulator.WithImageTag("1.2.0");
        emulator.WithLifetime(ContainerLifetime.Persistent);
        emulator.WithDataVolume();
    });

    appConfiguration.OnResourceReady(async (resource, _, cancellationToken) =>
        await AppConfigurationEmulatorSeeder.SeedIfMissingAsync(resource, cancellationToken));
}

builder.AddProject<Projects.Api>("api")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(appConfiguration)
    .WaitFor(appConfiguration);

builder.Build().Run();
