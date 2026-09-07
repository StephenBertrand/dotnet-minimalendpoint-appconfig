var builder = DistributedApplication.CreateBuilder(args);

var appConfiguration = builder.AddAzureAppConfiguration("appconfiguration");

if (builder.ExecutionContext.IsRunMode)
{
    appConfiguration.RunAsEmulator(emulator =>
    {
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
