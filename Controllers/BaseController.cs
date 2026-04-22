using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
using webProgramming.Models;

namespace webProgramming.Controllers
{
    public abstract class BaseController : Controller{

        protected readonly AppDbContext _appDbContext;
         public BaseController (AppDbContext Context){
            _appDbContext = Context;
        }

        // check if the user logged in or not
         protected bool IsLoggedin()
         {
            return HttpContext.Session.GetString("UserId") != null;
         }

        // get the current logged user
         protected User? GetCurrentloggedUser()
         {
            var userId = HttpContext.Session.GetString("UserId");

            if(userId != null)
            {
               var currentUserId = userId;
               
               var currentUser = _appDbContext.Users.FirstOrDefault(u => u.UserId == currentUserId);
               return currentUser;
            }
            
            return null;
         }

        // Buyer Authorization
        protected bool isBuyerAuthorized(UserRole requiredRole)
         {

            if( requiredRole == UserRole.Buyer)
            {
                return true;
            }else{
                return false;
            }
         }

          // function to return the UserRole of the Current User
         protected bool isAuthorized(UserRole requiredRole)
         {

            var user = GetCurrentloggedUser();
            if(user == null)
            {
                return false;
            }

            if(requiredRole == user.Role && user.Role != UserRole.Buyer)
            {
                return true;
            }else{
                return false;
            }
         }


          //Function to redirect the User to his/her Dashboard based on their Role
          protected IActionResult RedirectToDashboard(UserRole requiredRole)
          {
            if(requiredRole == UserRole.Admin)
            {
                
                return RedirectToAction("Index", "Admin");
            }else if(requiredRole == UserRole.Seller)
            {
                return RedirectToAction("Index", "Seller");

            }else if(requiredRole == UserRole.Buyer)
            {
                return RedirectToAction("Index", "Customer");
            }

            return RedirectToAction("Login", "Auth");
          }
          
          // if the user didn't login or is it's User Type not authorized to access other Controllers 
         protected IActionResult RedirectToLogIfNotLoggedinOrAuthorized(UserRole requiredRole){
            
            if(!IsLoggedin() || !isAuthorized(requiredRole))
            {
                TempData["ErrorMessage"] = "You are not authorized to Access this Page";
                return RedirectToAction("Login", "Auth");
            }
            return Ok();
         }

        



    }
} 