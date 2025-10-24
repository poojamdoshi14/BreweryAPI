namespace Test_PoojaDoshi.Models
{
    public class BrewerySearchRequest
    {
        public string? Query { get; set; }           // free-text search (name, city, phone)
        public string? Name { get; set; }            // filter by name
        public string? City { get; set; }            // filter by city
        public string? Phone { get; set; }            // filter by phone

        public string? SortBy { get; set; }          // name | city | distance

        public bool Desc { get; set; }               // sort descending
        public int? Limit { get; set; }              // paging limit
        public int? Offset { get; set; }             // paging offset
    }
}
