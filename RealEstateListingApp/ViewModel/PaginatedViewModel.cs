using RealEstateListingApp.Models;

namespace RealEstateListingApp.ViewModel
{
    public class PaginatedPViewModel<T>
    {
        public List<T> Items { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public string CurrentSearch { get; set; }
        public string CurrentSortOrder { get; set; }
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;
    }
}
