using Microsoft.AspNetCore.Routing;

namespace Api.Hosting;

/// <summary>
/// One vertical slice: a class maps its own routes. Add a new slice by implementing this
/// in a folder; <see cref="EndpointDiscovery"/> registers it automatically.
/// </summary>
public interface IEndpoint
{
    void Map(IEndpointRouteBuilder app);
}
