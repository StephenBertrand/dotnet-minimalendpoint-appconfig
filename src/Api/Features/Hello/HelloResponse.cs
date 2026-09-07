namespace Api.Slices.Hello;

public sealed record HelloResponse(string Message, string FeatureFlag, bool FeatureEnabled);
