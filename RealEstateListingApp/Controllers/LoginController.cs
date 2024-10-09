using Microsoft.AspNetCore.Mvc;
using RealEstateListingApp.Models;
using RealEstateListingApp.Services;

namespace RealEstateListingApp.Controllers
{
    public class LoginController : Controller
    {
        private readonly IUserRepository userRepository;

        public LoginController(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(User data)
        {
            if (ModelState.IsValid)
            {
                if (data != null)
                {
                    userRepository.SaveUserData(data);
                    TempData["SuccessMsg"] = "Registration Successfully...";
                    return RedirectToAction("Login", "Login");
                }
                else
                {
                    TempData["SuccessMsg"] = "Registration Faild...";
                }
            }
            return View(data);
        }

        public IActionResult Login()
        {
            // Check if the cookie exists
            if (Request.Cookies["username"] != null && Request.Cookies["password"] != null)
            {
                ViewBag.Username = Request.Cookies["username"];
                ViewBag.Password = Request.Cookies["password"];
                ViewBag.CheckValue = true;
            }
            return View();
        }

        [HttpPost]
        public IActionResult Login(string username, string password, bool rememberMe)
        {
            User record = userRepository.IsValidUser(username, password);
            if (record != null)
            {
                if (rememberMe)
                {
                    CookieOptions option = new CookieOptions
                    {
                        Expires = DateTime.Now.AddDays(30)
                    };
                    Response.Cookies.Append("username", username, option);
                    Response.Cookies.Append("password", password, option);
                }
                else
                {
                    Response.Cookies.Delete("username");
                    Response.Cookies.Delete("password");
                }

                HttpContext.Session.SetString("UserID", record.UserId.ToString());
                HttpContext.Session.SetString("UserName", record.UserName);
                HttpContext.Session.SetString("UserRole", record.Role);

                if (record.Role == "User")
                {
                    return RedirectToAction("Index", "User");
                }
                if (record.Role == "Agent")
                {
                    return RedirectToAction("Index2", "Home");
                }
                if (record.Role == "Admin")
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                TempData["SuccessMsg"] = "Login Failed...Invalid username and password";
                return View();
            }

            return View();
        }

        public IActionResult Logout()
        {
            if (HttpContext.Session.GetString("UserName") != null)
            {
                HttpContext.Session.Remove("UserName");
                HttpContext.Session.Remove("UserRole");
                HttpContext.Session.Remove("UserID");

                //HttpContext.Session.SetString("UserName", null);
                //HttpContext.Session.SetString("UserRole", null);
                //HttpContext.Session.SetString("UserID", null);
                //HttpContext.Session.Clear();
            }
            return RedirectToAction("Index", "Home");

        }
    }
}
