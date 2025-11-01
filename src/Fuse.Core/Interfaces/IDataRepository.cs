namespace Fuse.Core.Interfaces
{
    public interface IDataRepository
    {
        Task SaveObjectAsync<T>(string name, T obj);

        Task<T?> GetObjectAsync<T>(string name);
    }
}