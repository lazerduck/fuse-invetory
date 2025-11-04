using Fuse.Core.Interfaces;
using Fuse.Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Fuse.Core;

public static class FuseCodeModule
{
    public static void Register(IServiceCollection services)
    {
        services.AddScoped<IEnvironmentService, EnvironmentService>();
    }
}