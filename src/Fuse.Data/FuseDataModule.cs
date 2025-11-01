using Fuse.Core.Interfaces;
using Fuse.Data.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Fuse.Data
{
    public static class FuseDataModule
    {
        public static void RegisterServices(IServiceCollection services)
        {
            services.AddSingleton<IDataRepository, DataRepository>();
        }
    }
}