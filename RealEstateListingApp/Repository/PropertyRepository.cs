using Microsoft.EntityFrameworkCore;
using RealEstateListingApp.Models;
using RealEstateListingApp.Services;

namespace RealEstateListingApp.Repository
{
    public class PropertyRepository : IPropertyRepository
    {
        private readonly DbContextFile db;

        private DbSet<Properties> properties;

        public PropertyRepository(DbContextFile db)
        {
            this.db = db;
            properties = db.Set<Properties>();
        }

        public void DeletePropertyData(int id)
        {
            Properties data = GetPropertyById(id);
            db.Properties.Remove(data);
            db.SaveChanges();
        }

        public IEnumerable<Properties> GetAllProperties()
        {
            return db.Properties.Include(x => x.Agent).ToList();
        }

        public IEnumerable<PropertyViewModel> GetAllPropertiesByParam(int userid)
        {
            var properties = db.Properties.Include(x => x.Agent).ToList();

            var propertyViewModels = properties.Select(property => new PropertyViewModel
            {
                Property = property,
                IsInWishlist = db.Wishlists.Any(c => c.PropertyId == property.PropertyID && c.UserId == userid)
            }).ToList();

            return propertyViewModels;
        }


        public Properties GetPropertyById(int id)
        {
            return db.Properties.Include(x => x.Agent).Where(a => a.PropertyID == id).FirstOrDefault();
        }

        public void SavePropertyData(Properties properties)
        {
            db.Properties.Add(properties);
            db.SaveChanges();
        }

        public void UpdatePropertyData(Properties data)
        {
            properties.Attach(data);
            db.Properties.Update(data);
            db.SaveChanges();
        }

       
    }
}
