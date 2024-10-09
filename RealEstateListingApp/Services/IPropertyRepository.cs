using RealEstateListingApp.Models;

namespace RealEstateListingApp.Services
{
    public interface IPropertyRepository
    {
        IEnumerable<Properties> GetAllProperties();

        IEnumerable<PropertyViewModel> GetAllPropertiesByParam(int userid);

        Properties GetPropertyById(int id);

        void SavePropertyData(Properties properties);
        void UpdatePropertyData(Properties properties);

        void DeletePropertyData(int id);
    }
}
