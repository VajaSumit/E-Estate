using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RealEstateListingApp.Models;
using RealEstateListingApp.Services;
using System.Net.Mail;
using System.Net;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Microsoft.EntityFrameworkCore.Storage;
using RealEstateListingApp.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace RealEstateListingApp.Controllers
{
    public class UserController : Controller
    {
        private readonly IPropertyRepository propertyRepository;
        private readonly IUserRepository userRepository;
        private readonly IWishListRepository wishListRepository;
        private readonly DbContextFile db;

        public UserController(IPropertyRepository propertyRepository, IUserRepository userRepository, IWishListRepository wishListRepository, DbContextFile db)
        {
            this.propertyRepository = propertyRepository;
            this.userRepository = userRepository;
            this.wishListRepository = wishListRepository;
            this.db = db;
        }



        public IActionResult Index()
        {
            //if (HttpContext.Session.GetString("UserName") != null)
            //{
            //    int userid = Convert.ToInt32(HttpContext.Session.GetString("UserID"));
            //    IEnumerable<PropertyViewModel> data = propertyRepository.GetAllPropertiesByParam(userid);
            //    return View(data);
            //}
            //else
            //{
            //    var data = db.Properties.Select(p => new PropertyViewModel
            //    {
            //        Properties = db.Properties.Include(x => x.Agent).ToList()
            //    });

            //    return View(data);
            //}

            IEnumerable<Properties> data = propertyRepository.GetAllProperties().Take(9);
            int userid = Convert.ToInt32(HttpContext.Session.GetString("UserID"));
            var wishlist = db.Wishlists.Where(w => w.UserId == userid).Select(w => w.PropertyId).ToList();

            ViewData["Wishlist"] = wishlist;

            return View(data);

        }

        [HttpPost]
        public IActionResult ToggleWishlist(int propertyId)
        {
            if (!int.TryParse(HttpContext.Session.GetString("UserID"), out int userId))
            {
                return Json(new { success = false, message = "Please login first ..." });
            }
            int userid = Convert.ToInt32(HttpContext.Session.GetString("UserID"));
            var existingWishlistItem = db.Wishlists.FirstOrDefault(w => w.UserId == userId && w.PropertyId == propertyId);

            if (existingWishlistItem != null)
            {
                db.Wishlists.Remove(existingWishlistItem); 
                 db.SaveChanges();

                return Json(new { success = true, message = "Property removed from your wishlist." });
            }
            else
            {

                var wishlistItem = new Wishlists
                {
                    UserId = userId,
                    PropertyId = propertyId
                };

                db.Wishlists.Add(wishlistItem);
                db.SaveChanges();

                return Json(new { success = true, message = "Property added to your wishlist." });
            }
        }

        public IActionResult BuyProperty(string searchString, int pageNumber = 1, int pageSize = 6)
        {
            //if (HttpContext.Session.GetString("UserName") != null)
            //{
            IEnumerable<Properties> properties = propertyRepository.GetAllProperties().Where(x => x.Type == "Sell").ToList();

            int userid = Convert.ToInt32(HttpContext.Session.GetString("UserID"));
            var wishlist = db.Wishlists.Where(w => w.UserId == userid).Select(w => w.PropertyId).ToList();

            TempData["Wishlistforbuyproperty"] = wishlist;


            if (!string.IsNullOrEmpty(searchString))
            {
                properties = properties.Where(u =>
                    u.Agent.UserName.Contains(searchString) ||
                    u.Address.Contains(searchString) ||
                    u.PropertyName.Contains(searchString));
            }
            var totalproperty = properties.Count();
            var totalPages = (int)Math.Ceiling((double)totalproperty / pageSize);

            var paginatedproperty = properties.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            var viewModel = new PaginatedPViewModel<Properties>
            {
                Items = paginatedproperty,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = totalPages,
                CurrentSearch = searchString
            };

            return View(viewModel);
            //}
            //else
            //{
            //    return RedirectToAction("Login", "Login");
            //}
        }

        public IActionResult RentProperty(string searchString, int pageNumber = 1, int pageSize = 6)
        {
            //if (HttpContext.Session.GetString("UserName") != null)
            //{
            IEnumerable<Properties> properties = propertyRepository.GetAllProperties().Where(x => x.Type == "Rent").ToList();

            int userid = Convert.ToInt32(HttpContext.Session.GetString("UserID"));
            var wishlist = db.Wishlists.Where(w => w.UserId == userid).Select(w => w.PropertyId).ToList();

            TempData["Wishlistforrentproperty"] = wishlist;


            if (!string.IsNullOrEmpty(searchString))
            {
                properties = properties.Where(u =>
                    u.Agent.UserName.Contains(searchString) ||
                    u.Address.Contains(searchString) ||
                    u.PropertyName.Contains(searchString));
            }
            var totalproperty = properties.Count();
            var totalPages = (int)Math.Ceiling((double)totalproperty / pageSize);

            var paginatedproperty = properties.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            var viewModel = new PaginatedPViewModel<Properties>
            {
                Items = paginatedproperty,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = totalPages,
                CurrentSearch = searchString
            };

            return View(viewModel);
            //}
            //else
            //{
            //    return RedirectToAction("Login", "Login");
            //}
        }

        public IActionResult PropertyDetails(int id)
        {
            //if (HttpContext.Session.GetString("UserName") != null)
            //{
            Properties data = propertyRepository.GetPropertyById(id);
            ViewBag.agentname = data.Agent.UserName;
            ViewBag.propertytname = data.PropertyName;
            if (HttpContext.Session.GetString("UserName") != null)
            {
                ViewBag.username = HttpContext.Session.GetString("UserName").ToString();
            }
            return View(data);
            //}
            //else
            //{
            //    return RedirectToAction("Login", "Login");
            //}
        }


        //public IActionResult AddInWishlist(int id)
        //{

        //    if (HttpContext.Session.GetString("UserName") != null)
        //    {
        //        int userid = int.Parse(HttpContext.Session.GetString("UserID").ToString());
        //        Wishlists data = new Wishlists
        //        {
        //            PropertyId = id,
        //            UserId = userid

        //        };
        //        wishListRepository.AddPropertyInWishlist(data);
        //        TempData["AddWishlist"] = "<script>alert('Property Add In Wishlist')</script>";
        //        return RedirectToAction("Index", "User");
        //    }
        //    else
        //    {
        //        return RedirectToAction("Login", "Login");
        //    }
        //}

        [HttpPost]
        public IActionResult AddInWishlist(int id)
        {
            if (HttpContext.Session.GetString("UserName") != null)
            {
                int userid = int.Parse(HttpContext.Session.GetString("UserID").ToString());
                Wishlists data = new Wishlists
                {
                    PropertyId = id,
                    UserId = userid
                };
                wishListRepository.AddPropertyInWishlist(data);
                return Json(new { success = true });
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }






        public IActionResult AllAgents(string searchString, int pageNumber = 1, int pageSize = 9)
        {
            //if (HttpContext.Session.GetString("UserName") != null)
            //{
            IEnumerable<User> agents = userRepository.GetAllUsers().Where(x => x.Role == "Agent").ToList();

            if (!string.IsNullOrEmpty(searchString))
            {
                agents = agents.Where(u =>
                    u.ContactInfo.Contains(searchString) ||
                    u.UserName.Contains(searchString));
            }
            var totalagents = agents.Count();
            var totalPages = (int)Math.Ceiling((double)totalagents / pageSize);

            var paginatedpagents = agents.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            var viewModel = new PaginatedPViewModel<User>
            {
                Items = paginatedpagents,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = totalPages,
                CurrentSearch = searchString
            };

            return View(viewModel);
            //}
            //else
            //{
            //    return RedirectToAction("Login", "Login");
            //}
        }

        public IActionResult Wishlists()
        {
            if (HttpContext.Session.GetString("UserName") != null)
            {
                var userWishlists = wishListRepository.GetWishlists()
                    .Where(w => w.UserId == int.Parse(HttpContext.Session.GetString("UserID").ToString()))
                    .Select(w => w.PropertyId).ToList();


                var properties = propertyRepository.GetAllProperties()
                    .Where(p => userWishlists.Contains(p.PropertyID))
                    .ToList();

                return View(properties);

            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }

        public IActionResult DeleteFromWishlistPage(int id)
        {
            if (HttpContext.Session.GetString("UserName") != null)
            {
                var data = db.Wishlists.Where(x => x.PropertyId == id).FirstOrDefault();
                if (data != null)
                {
                    wishListRepository.DeletePropertyFromWishlist(data);
                    TempData["DeleteFromWishlist"] = "<script>alert('Property Remove Successfully...')</script>";
                }

                return RedirectToAction("Wishlists", "User");

            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }

        [HttpPost]
        public IActionResult DeleteFromWishlist(int id)
        {
            if (HttpContext.Session.GetString("UserName") != null)
            {
                var data = db.Wishlists.Where(x => x.PropertyId == id).FirstOrDefault();
                if (data != null)
                {
                    wishListRepository.DeletePropertyFromWishlist(data);
                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false, message = "Property not found in wishlist" });
                }
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }

        [HttpPost]
        public ActionResult InsertInquiry(string username, string agentname, string propertyname, string message, int agentid, int propertyid)
        {
            if (HttpContext.Session.GetString("UserName") != null)
            {
                int userid = int.Parse(HttpContext.Session.GetString("UserID").ToString());
                Inquiries data = new Inquiries();
                data.AgentId = agentid;
                data.UserId = userid;
                data.PropertyId = propertyid;
                data.Message = message;
                data.Dates = Convert.ToDateTime(DateTime.Now.ToShortDateString());

                sendEmailAgent(username, agentname, message, propertyname);
                sendEmailUser(username, agentname, message, propertyid);

                db.Inquiries.Add(data);
                db.SaveChanges();

                return Json(new { });
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }

        }



        private void sendEmailAgent(string username, string agentname, string message, string propertyname)
        {
            //var userdata = userRepository.GetAllUsers().Where(x => x.UserName == username).FirstOrDefault();


            var fromEmail = new MailAddress("jobfinder612@gmail.com", username);
            var fromEmailPassword = "jexqoebxzlzfknyr";
            var toemail = new MailAddress(agentname);
            var smtp = new SmtpClient();
            smtp.Host = "smtp.gmail.com";
            smtp.Port = 587;
            smtp.EnableSsl = true;
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword);

            var Message = new MailMessage(fromEmail, toemail);
            Message.Subject = $"Inquiry about {propertyname}";
            Message.Body = "Username : " + username + "Contact Information :" + 9615554555 + "Message : " + message;
            Message.IsBodyHtml = true;

            smtp.Send(Message);


        }

        private void sendEmailUser(string username, string agentname, string message, int propertyid)
        {
            var propertydata = propertyRepository.GetPropertyById(propertyid);


            var fromEmail = new MailAddress("jobfinder612@gmail.com", agentname);
            var fromEmailPassword = "jexqoebxzlzfknyr";
            var toemail = new MailAddress(username);
            var smtp = new SmtpClient();
            smtp.Host = "smtp.gmail.com";
            smtp.Port = 587;
            smtp.EnableSsl = true;
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword);

            var Message = new MailMessage(fromEmail, toemail);
            Message.Subject = $"Thank You For Showing Inquiry : {propertydata.PropertyName}";
            string bodydata =
            Message.Body = "Property Details : Property Name : " + propertydata.PropertyName + "Price  :" + propertydata.Price + "Type : " + propertydata.Type + "Address : " + propertydata.Address + "Descriptions : " + propertydata.Description + "Agent Name : " + propertydata.Agent.UserName + "Agent Contact information : " + propertydata.Agent.ContactInfo;
            Message.IsBodyHtml = true;

            smtp.Send(Message);


        }

        public IActionResult UserProfile()
        {
            if (HttpContext.Session.GetString("UserName") != null)
            {
                string username = HttpContext.Session.GetString("UserName").ToString();
                User data = userRepository.GetAllUsers().Where(x => x.UserName == username).FirstOrDefault();
                return View(data);
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }

        public IActionResult EditProfile(int id)
        {
            if (HttpContext.Session.GetString("UserName") != null)
            {
                if (id != null)
                {
                    User data = userRepository.GetUserById(id);
                    if (data != null)
                    {
                        ViewBag.pass = data.Password;
                        return View(data);
                    }
                }
                return RedirectToAction("UserProfile", "User");
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }

        [HttpPost]
        public IActionResult EditProfile(User user)
        {
            if (HttpContext.Session.GetString("UserName") != null)
            {
                if (user != null)
                {
                    User data = new User();
                    data.UserName = user.UserName;
                    data.Password = user.Password;
                    data.Role = user.Role;
                    data.ContactInfo = user.ContactInfo;
                    data.UserId = user.UserId;
                    userRepository.UpdateUserData(data);
                    TempData["EditMsg"] = "<script>alert('Profile edit successfully...')</script>";
                }
                return RedirectToAction("UserProfile", "User");
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }

    }
}
