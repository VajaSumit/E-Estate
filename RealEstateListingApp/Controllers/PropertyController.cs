using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore;
using RealEstateListingApp.Models;
using RealEstateListingApp.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using RealEstateListingApp.ViewModel;
using System.Linq.Expressions;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace RealEstateListingApp.Controllers
{
    public class PropertyController : Controller
    {
        private readonly IPropertyRepository propertyRepository;
        private readonly IWebHostEnvironment environment;
        private readonly IUserRepository userRepository;
        private readonly IWishListRepository wishListRepository;
        private readonly DbContextFile db;

        public PropertyController(IPropertyRepository propertyRepository, IWebHostEnvironment _environment, IUserRepository userRepository, IWishListRepository wishListRepository, DbContextFile db)
        {
            this.propertyRepository = propertyRepository;
            environment = _environment;
            this.userRepository = userRepository;
            this.wishListRepository = wishListRepository;
            this.db = db;
        }

        public IActionResult Index(string searchString, int pageNumber = 1, int pageSize = 10, string sortColumn = "PropertyName", string sortOrder = "asc")
        {
            if (HttpContext.Session.GetString("UserName") != null)
            {
                if (HttpContext.Session.GetString("UserRole") == "Admin")
                {
                    var properties = propertyRepository.GetAllProperties().AsQueryable();

                    if (!string.IsNullOrEmpty(searchString))
                    {
                        properties = properties.Where(p =>
                            p.PropertyName.Contains(searchString) ||
                            p.Description.Contains(searchString) ||
                            p.Agent.UserName.Contains(searchString) ||
                            p.Type.Contains(searchString));
                    }

                    string sortExpression = $"{sortColumn} {sortOrder}";
                    properties = properties.OrderBy(sortExpression);

                    var totalProperties = properties.Count();
                    var totalPages = (int)Math.Ceiling((double)totalProperties / pageSize);

                    var paginatedProperties = properties.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

                    var viewModel = new PaginatedPViewModel<Properties>
                    {
                        Items = paginatedProperties,
                        PageNumber = pageNumber,
                        PageSize = pageSize,
                        TotalPages = totalPages,
                        CurrentSearch = searchString
                    };

                    return View(viewModel);
                }
                if (HttpContext.Session.GetString("UserRole") == "Agent")
                {
                    string agentname = HttpContext.Session.GetString("UserName").ToString();
                    var properties = propertyRepository.GetAllProperties().Where(x => x.Agent.UserName == agentname).AsQueryable();

                    if (!string.IsNullOrEmpty(searchString))
                    {
                        properties = properties.Where(p =>
                            p.PropertyName.Contains(searchString) ||
                            p.Description.Contains(searchString) ||
                            p.Agent.UserName.Contains(searchString) ||
                            p.Type.Contains(searchString));
                    }

                    string sortExpression = $"{sortColumn} {sortOrder}";
                    properties = properties.OrderBy(sortExpression);

                    var totalProperties = properties.Count();
                    var totalPages = (int)Math.Ceiling((double)totalProperties / pageSize);

                    var paginatedProperties = properties.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

                    var viewModel = new PaginatedPViewModel<Properties>
                    {
                        Items = paginatedProperties,
                        PageNumber = pageNumber,
                        PageSize = pageSize,
                        TotalPages = totalPages,
                        CurrentSearch = searchString
                    };

                    return View(viewModel);
                }
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }
        public IActionResult Create()
        {
            if (HttpContext.Session.GetString("UserName") != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Properties property, List<IFormFile> Images)
        {
            property.AgentId = Convert.ToInt32(HttpContext.Session.GetString("UserID"));

            if (ModelState.IsValid)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                property.Images = new List<string>();

                foreach (var formFile in Images)
                {
                    if (formFile.Length > 0)
                    {
                        // Create a unique file name
                        var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(formFile.FileName);
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        // Save the file to the uploads folder
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            formFile.CopyTo(stream);
                        }

                        // Add the file path relative to wwwroot to the property image paths
                        property.Images.Add("/uploads/" + uniqueFileName);
                    }
                }

                // Save the property details in the database
                propertyRepository.SavePropertyData(property);
                TempData["InsertMsg"] = "Property Add Successfully...";
                return RedirectToAction("Index", "Property");
            }
            return View(property);
        }

        public IActionResult Details(int id)
        {
            if (HttpContext.Session.GetString("UserName") != null)
            {
                Properties data = propertyRepository.GetPropertyById(id);
                return View(data);
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }

        public IActionResult Delete(int id)
        {
            if (HttpContext.Session.GetString("UserName") != null)
            {
                Properties data = propertyRepository.GetPropertyById(id);
                return View(data);
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }

        [HttpPost]
        public IActionResult Delete(int id, Properties data)
        {
            Properties property = propertyRepository.GetPropertyById(id);
            if (property != null)
            {
                var wishlistdata = wishListRepository.GetWishlists().Any(x => x.PropertyId == id);
                var inquiresdata = db.Inquiries.Any(x => x.PropertyId == id);

                if (wishlistdata || inquiresdata)
                {
                    TempData["AlreadyUse"] = "You don't delete this property because it already use in another table";
                    return View(property);
                }
                else
                {
                    foreach (var imagePath in property.Images)
                    {
                        var fullPath = Path.Combine(environment.WebRootPath, "wwwroot/uploads", imagePath);
                        if (System.IO.File.Exists(fullPath))
                        {
                            System.IO.File.Delete(fullPath);
                        }
                    }
                    propertyRepository.DeletePropertyData(id);
                    TempData["DeleteMsg"] = "Property Delete Successfully...";
                    return RedirectToAction("Index", "Property");
                }
                return View(data);
            }
            else
            {
                TempData["DeleteMsg"] = "Property Not Delete...";
                return View(data);
            }

           
        }

        public IActionResult Edit(int id)
        {
            if (HttpContext.Session.GetString("UserName") != null)
            {
                Properties data = propertyRepository.GetPropertyById(id);
                return View(data);
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Properties property, List<IFormFile> newImages, string[] ExistingImages, string[] RemovedImages)
        {
            if (ModelState.IsValid)
            {
                var existingProperty = propertyRepository.GetPropertyById(id);

                if (existingProperty == null)
                {
                    return NotFound();
                }

                var agentExists = userRepository.GetAllUsers().Any(u => u.UserId == property.AgentId);
                if (!agentExists)
                {
                    ModelState.AddModelError("AgentId", "Invalid agent ID.");
                    return View(property);
                }

                // Update property fields
                existingProperty.PropertyName = property.PropertyName;
                existingProperty.Type = property.Type;
                existingProperty.Address = property.Address;
                existingProperty.Price = property.Price;
                existingProperty.Description = property.Description;
                existingProperty.AgentId = property.AgentId;

                // Handle removal of images
                foreach (var removedImage in RemovedImages)
                {
                    var imageToRemove = existingProperty.Images.FirstOrDefault(img => img == removedImage);
                    if (imageToRemove != null)
                    {
                        existingProperty.Images.Remove(imageToRemove);
                        var fullPath = Path.Combine(environment.WebRootPath, imageToRemove.TrimStart('/'));
                        if (System.IO.File.Exists(fullPath))
                        {
                            System.IO.File.Delete(fullPath);
                        }
                    }
                }

                // Handle addition of new images
                if (newImages != null && newImages.Count > 0)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    foreach (var formFile in newImages)
                    {
                        if (formFile.Length > 0)
                        {
                            var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(formFile.FileName);
                            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                formFile.CopyTo(stream);
                            }

                            existingProperty.Images.Add("/uploads/" + uniqueFileName);

                        }
                    }
                }

                propertyRepository.UpdatePropertyData(existingProperty);
                TempData["UpdateMsg"] = "Property updated successfully.";
                return RedirectToAction("Index", "Property");
            }
            return View(property);
        }
    }
}

