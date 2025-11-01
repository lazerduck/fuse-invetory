using Fuse.Core.Interfaces;
using Fuse.Core.Manifests;

namespace Fuse.Core.Services
{
    public class ServiceService : IServiceService
    {
        private readonly IDataRepository _dataRepository;
        private const string fileName = "ServiceManifests.json";
        public static List<ServiceManifest> _serviceManifests = new List<ServiceManifest>();

        public ServiceService(IDataRepository dataRepository)
        {
            _dataRepository = dataRepository;
            var manifests = _dataRepository.GetObjectAsync<List<ServiceManifest>>(fileName).Result;
            if (manifests == null)
            {
                _dataRepository.SaveObjectAsync(fileName, _serviceManifests).Wait();
            }
            else
            {
                _serviceManifests = manifests;
            }
        }

        public Task CreateServiceManifestAsync(ServiceManifest manifest)
        {
            if (manifest.Id == Guid.Empty)
            {
                throw new ArgumentException("Manifest must have a unique non-empty Id.");
            }
            if (_serviceManifests.Any(m => m.Id == manifest.Id))
            {
                throw new ArgumentException($"A manifest with Id {manifest.Id} already exists.");
            }

            _serviceManifests.Add(manifest);
            return _dataRepository.SaveObjectAsync(fileName, _serviceManifests);
        }

        public Task UpdateServiceManifestAsync(ServiceManifest manifest)
        {
            var existingManifest = _serviceManifests.FirstOrDefault(m => m.Id == manifest.Id)
                ?? throw new ArgumentException($"No manifest found with Id {manifest.Id}.");
                
            manifest = manifest with { CreatedAt = existingManifest.CreatedAt, UpdatedAt = DateTime.UtcNow };

            _serviceManifests.Remove(existingManifest);
            _serviceManifests.Add(manifest);
            return _dataRepository.SaveObjectAsync(fileName, _serviceManifests);
        }

        public Task<ServiceManifest?> GetServiceManifestAsync(Guid id)
        {
            var manifest = _serviceManifests.FirstOrDefault(m => m.Id == id);
            return Task.FromResult(manifest);
        }

        public Task<List<ServiceManifest>> GetAllServiceManifestsAsync()
        {
            return Task.FromResult(_serviceManifests);
        }
    }

}