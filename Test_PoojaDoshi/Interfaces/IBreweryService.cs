using Test_PoojaDoshi.Models;

namespace Test_PoojaDoshi.Interfaces
{
    public interface IBreweryService
    {
        Task<IEnumerable<Brewery>> SearchAsync(BrewerySearchRequest request);
    }
}
