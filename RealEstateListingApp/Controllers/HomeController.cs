using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RealEstateListingApp.Models;
using RealEstateListingApp.Services;
using RealEstateListingApp.ViewModel;
using System.Diagnostics;
using System.Linq.Dynamic.Core;

namespace RealEstateListingApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUserRepository userRepository;
        private readonly IPropertyRepository propertyRepository;
        private readonly IWishListRepository wishListRepository;
        private readonly DbContextFile db;

        public HomeController(IUserRepository userRepository, IPropertyRepository propertyRepository, IWishListRepository wishListRepository,DbContextFile db)
        {
            this.userRepository = userRepository;
            this.propertyRepository = propertyRepository;
            this.wishListRepository = wishListRepository;
            this.db = db;
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("UserName") != null)
            {
                ViewBag.Property = propertyRepository.GetAllProperties().Count();
                ViewBag.User = userRepository.GetAllUsers().Where(x => x.Role == "User").Count();
                ViewBag.Agent = userRepository.GetAllUsers().Where(x => x.Role == "Agent").Count();
                    
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }

        public IActionResult Index2()
        {
            if (HttpContext.Session.GetString("UserName") != null)
            {
               
                    string agentname = HttpContext.Session.GetString("UserName").ToString();
                    ViewBag.Property = propertyRepository.GetAllProperties().Where(x => x.Agent.UserName == agentname).Count();
                    return View();
                
              
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }

        public IActionResult AllUsers(string searchString, int pageNumber = 1, int pageSize = 4, string sortColumn = "UserName", string sortOrder = "asc")
        {
            if (HttpContext.Session.GetString("UserName") != null)
            {
                var users = userRepository.GetAllUsers().Where(x => x.Role == "User").AsQueryable();

                if (!string.IsNullOrEmpty(searchString))
                {
                    users = users.Where(u =>
                        u.UserName.Contains(searchString) ||
                        u.ContactInfo.Contains(searchString));
                }

                string sortExpression = $"{sortColumn} {sortOrder}";
                users = users.OrderBy(sortExpression);

                var totalUsers = users.Count();
                var totalPages = (int)Math.Ceiling((double)totalUsers / pageSize);

                var paginatedUsers = users.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

                var viewModel = new PaginatedPViewModel<User>
                {
                    Items = paginatedUsers,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalPages = totalPages,
                    CurrentSearch = searchString
                };

                return View(viewModel);
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }

        public IActionResult AllAgent(string searchString, int pageNumber = 1, int pageSize = 4, string sortColumn = "UserName", string sortOrder = "asc")
        {
            if (HttpContext.Session.GetString("UserName") != null)
            {
                var users = userRepository.GetAllUsers().Where(x => x.Role == "Agent").AsQueryable();

                if (!string.IsNullOrEmpty(searchString))
                {
                    users = users.Where(u =>
                        u.UserName.Contains(searchString) ||
                        u.ContactInfo.Contains(searchString));
                }

                string sortExpression = $"{sortColumn} {sortOrder}";
                users = users.OrderBy(sortExpression);

                var totalUsers = users.Count();
                var totalPages = (int)Math.Ceiling((double)totalUsers / pageSize);

                var paginatedUsers = users.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

                var viewModel = new PaginatedPViewModel<User>
                {
                    Items = paginatedUsers,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalPages = totalPages,
                    CurrentSearch = searchString
                };

                return View(viewModel);
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }

       

        public IActionResult DeleteUser(int id)
        {
            if (HttpContext.Session.GetString("UserName") != null)
            {
                if (id != null)
                {

                    var wishlistdata = wishListRepository.GetWishlists().Any(x => x.UserId == id) ;
                    var inquiresdata = db.Inquiries.Any(x => x.UserId == id);

                    if (wishlistdata || inquiresdata)
                    {
                        TempData["AlreadyUse"] = "You don't delete user because it already use in another table";
                    }
                    else
                    {
                        userRepository.DeleteUserData(id);
                        TempData["DeleteUser"] = "User Record Delete Successfully...";
                        return RedirectToAction("AllUsers", "Home");
                    }
                   
                }
                else
                {
                    TempData["DeleteUser"] = "User Record Not Deleted ...";
                }
                return RedirectToAction("AllUsers", "Home");
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }

        public IActionResult DeleteAgent(int id)
        {
            if (HttpContext.Session.GetString("UserName") != null)
            {
                if (id != null)
                {
                    var inquiresdata = db.Inquiries.Any(x => x.AgentId == id);
                    var propertydata = propertyRepository.GetAllProperties().Any(x => x.AgentId == id);

                    if (inquiresdata || propertydata)
                    {
                        TempData["AlreadyUse"] = "You don't delete agent because it already use in another table";
                    }
                    else
                    {
                        userRepository.DeleteUserData(id);
                        TempData["DeleteAgent"] = "Agent Record Delete Successfully...";
                        return RedirectToAction("AllAgent", "Home");

                    }
                }
                else
                {
                    TempData["DeleteAgent"] = "Agent Record Not Deleted ...";
                }
                return RedirectToAction("AllAgent", "Home");
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
