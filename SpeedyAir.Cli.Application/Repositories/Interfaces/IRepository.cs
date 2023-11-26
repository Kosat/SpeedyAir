namespace SpeedyAir.Repositories.Interfaces;

internal interface IRepository<T>
{
    Task<IEnumerable<T>> GetAllAsync();
}
