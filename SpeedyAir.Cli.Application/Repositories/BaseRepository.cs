using SpeedyAir.Repositories.Interfaces;

namespace SpeedyAir.Repositories;

internal abstract class BaseRepository<T> : IRepository<T>
{

    private readonly string _filePath;
    private readonly Lazy<Task<List<T>>> _lazyEntities;

    protected string FilePath => _filePath;

    protected BaseRepository(string filePath)
    {
        _filePath = filePath;
        _lazyEntities = new Lazy<Task<List<T>>>(async () => await LoadData());
    }

    protected abstract Task<List<T>> LoadData();

    public async Task<IEnumerable<T>> GetAllAsync()
        => await _lazyEntities.Value!;
}
