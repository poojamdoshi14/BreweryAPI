using Test_PoojaDoshi.Models;

namespace Test_PoojaDoshi.Interfaces
{
    public interface IBreweryRepository
    {
        Task<IReadOnlyList<Brewery>> GetAllAsync(CancellationToken cancellationToken = default);
    }
}
