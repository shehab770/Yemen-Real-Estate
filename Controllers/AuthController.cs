using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using webProgramming.Models;

namespace webProgramming.Controllers
{
    public class AuthController : FunctionalController
    {
        private readonly PasswordHasher<User> _passwordHasher;

        public AuthController(AppDbContext Context) : base(Context)
        {
            _passwordHasher = new PasswordHasher<User>();
        }

        public IActionResult SetTempData()
        {
            var user = GetCurrentloggedUser();
            if(user == null)
            {
                return RedirectToAction("Login");
            }
            TempData["NoPermission"] = "You are not authorized to Perform this Action";
            return RedirectToDashboard(user.Role);
        }

        public IActionResult Login()
        {
            var LoggedUser = GetCurrentloggedUser();

            if (LoggedUser != null)
            {
                return RedirectToDashboard(LoggedUser.Role);
            }
            
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        public IActionResult SellerRegister()
        {
            return View();
        }

        public IActionResult BuyerRegister()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> AddUSeller(Seller model)
        {

            if (ModelState.IsValid)
            {
                
                var user = await CreateSeller(model);
                if(user == null)
                {
                    TempData["Status"] = "Your Registration Failed! Please try again.";
                    return View("SellerRegister", model);
                }
                await _appDbContext.Users.AddAsync(user);
                await _appDbContext.SaveChangesAsync();

                TempData["Status"] = "Your Registration was successful! Please Wait for Email Approval.";
                return RedirectToAction("Login");
            }

            return View("SellerRegister", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
         public async Task<IActionResult> AddUBuyer(Buyer model)
        {

            if (ModelState.IsValid)
            {
                
                var user = await CreateBuyer(model);
                if(user == null)
                {
                    TempData["Status"] = "Your Registration Failed! Please try again.";
                    return View("BuyerRegister", model);
                }
                await _appDbContext.Users.AddAsync(user);
                await _appDbContext.SaveChangesAsync();

                TempData["Status"] = "Your registration was successful! Please login.";
                return RedirectToAction("Login");
            }

            return View("BuyerRegister", model);
        }

        [HttpGet]
        public JsonResult IsEmailAvailable(string email)
        {
            var emailExists = _appDbContext.Users.Any(u => u.Email == email);
            return Json(!emailExists); 
        }

        [HttpGet]
        public JsonResult IsPhoneAvailable(string phone)
        {
            var phoneExists = _appDbContext.Users.Any(u => u.Phone == phone);
            return Json(!phoneExists);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CheckLogin(LoginModel model)
        {

           try{

             if (!ModelState.IsValid)
             {
                 return View("Login", model);
             }

             var user = _appDbContext.Users.SingleOrDefault(u => u.Email == model.Email);

             if (user == null)
             {
                 ModelState.AddModelError("Email", "Email Not Found");
                 return View("Login", model);
             }

             if(user.IsBlocked)
             {
                TempData["Blocked"] = "Your account is blocked. Reason: " + user.BlockReason + " Please contact the administrator.";
                return RedirectToAction("Login");
             }

             var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, model.Password);

             if (result == PasswordVerificationResult.Failed)
             {
                 TempData["Wrong"] = "Password is incorrect.";
                 return RedirectToAction("Login");
             }

             HttpContext.Session.SetString("UserId", user.UserId.ToString());

             //Here we Redirect based on role
             return RedirectToDashboard(user.Role);

           }catch(Exception ex)
           {
            TempData["Error"] = "An error occurred while logging in. Please try again later." + ex.Message;
            return RedirectToAction("Login");
           }
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            // Clear authentication cookie
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            
            // Clear all session data
            HttpContext.Session.Clear();
            
            // Clear response cache to prevent back button from showing cached pages
            Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "0";
            
            return RedirectToAction("Login");
        }
    }
}
