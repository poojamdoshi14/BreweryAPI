using Test_PoojaDoshi.Interfaces;
using Test_PoojaDoshi.Models;

namespace Test_PoojaDoshi.Services
{
    public class BreweryService : IBreweryService
    {
        private readonly IBreweryRepository _repository;
        private readonly ILogger<BreweryService> _logger;

        public BreweryService(IBreweryRepository repository, ILogger<BreweryService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<IEnumerable<Brewery>> SearchAsync(BrewerySearchRequest request)
        {
            var items = await _repository.GetAllAsync();

            IEnumerable<Brewery> q = items;

            // Free text search across name, city, phone
            if (!string.IsNullOrWhiteSpace(request.Query))
            {
                var term = request.Query.Trim();
                q = q.Where(b =>
                    (b.Name?.Contains(term, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (b.City?.Contains(term, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (b.Phone?.Contains(term, StringComparison.OrdinalIgnoreCase) ?? false));
            }

            if (!string.IsNullOrWhiteSpace(request.Name))
            {
                q = q.Where(b => string.Equals(b.Name, request.Name, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(request.City))
            {
                q = q.Where(b => string.Equals(b.City, request.City, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(request.Phone))
            {
                q = q.Where(b => string.Equals(b.Phone, request.Phone, StringComparison.OrdinalIgnoreCase));
            }


            // Sorting
            q = (request.SortBy?.ToLowerInvariant()) switch
            {
                "name" => request.Desc ? q.OrderByDescending(b => b.Name) : q.OrderBy(b => b.Name),
                "city" => request.Desc ? q.OrderByDescending(b => b.City) : q.OrderBy(b => b.City),
                _ => q
            };

            // Paging
            if (request.Offset.HasValue)
                q = q.Skip(Math.Max(0, request.Offset.Value));
            if (request.Limit.HasValue)
                q = q.Take(Math.Max(0, request.Limit.Value));

            return q.ToList();
        }

    }
}
