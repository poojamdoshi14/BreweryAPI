using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;
using Test_PoojaDoshi.Interfaces;
using Test_PoojaDoshi.Models;

namespace Test_PoojaDoshi.Repositories
{
    public class BreweryRepository : IBreweryRepository
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _cache;
        private readonly ILogger<BreweryRepository> _logger;
        private const string CacheKey = "openbrewerydb_all";

        public BreweryRepository(HttpClient httpClient, IMemoryCache cache, ILogger<BreweryRepository> logger)
        {
            _httpClient = httpClient;
            _cache = cache;
            _logger = logger;
        }

        public async Task<IReadOnlyList<Brewery>> GetAllAsync()
        {
            if (_cache.TryGetValue(CacheKey, out IReadOnlyList<Brewery>? cached) && cached is not null)
            {
                return cached;
            }

            try
            {
                // Pull multiple pages to get a decent dataset; cap to avoid over-fetching.
                var all = new List<Brewery>();
                int page = 1;
                const int perPage = 100;
                const int maxPages = 5; // up to 500 records

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                while (page <= maxPages)
                {
                    var url = $"/v1/breweries?per_page={perPage}&page={page}";
                    using var resp = await _httpClient.GetAsync(url);
                    resp.EnsureSuccessStatusCode();
                    var stream = await resp.Content.ReadAsStreamAsync();
                    var nodes = await JsonSerializer.DeserializeAsync<List<JsonElement>>(stream, options)
                                ?? new List<JsonElement>();

                    if (nodes.Count == 0) break;

                    foreach (var b in nodes)
                    {
                        // Map to generic model
                        string id = b.TryGetProperty("id", out var n1) ? n1.GetString() ?? string.Empty : string.Empty;
                        string name = b.TryGetProperty("name", out var n) ? n.GetString() ?? string.Empty : string.Empty;
                        string city = b.TryGetProperty("city", out var c) ? c.GetString() ?? string.Empty : string.Empty;
                        string? phone = b.TryGetProperty("phone", out var p) ? p.GetString() : null;

                        all.Add(new Brewery
                        {
                            Id = id,
                            Name = name,
                            City = city,
                            Phone = phone,
                        });
                    }

                    page++;
                }

                // Cache for 10 minutes
                _cache.Set(CacheKey, all.AsReadOnly(), new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                });

                return all.AsReadOnly();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch breweries from OpenBreweryDB");
                // Return empty list on failure to keep API resilient
                return Array.Empty<Brewery>();
            }
        }
    }
}
