using Fuse.Core.Manifests;

namespace Fuse.Core.Interfaces
{
    public interface IServiceService
    {
        Task CreateServiceManifestAsync(ServiceManifest manifest);
        Task UpdateServiceManifestAsync(ServiceManifest manifest);
        Task<ServiceManifest?> GetServiceManifestAsync(Guid id);
        Task<List<ServiceManifest>> GetAllServiceManifestsAsync();
    }
}