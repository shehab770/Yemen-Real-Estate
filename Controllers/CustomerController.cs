using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webProgramming.Models;

namespace webProgramming.Controllers
{
    public class CustomerController : BaseController
    {

        public CustomerController(AppDbContext Context):base(Context)
        {
        }
        public IActionResult Index()
        {
            var user = GetCurrentloggedUser();
            if(user == null)
            {
                TempData["ErrorMessage"] = "Please Login First";
                return RedirectToAction("SetTempData", "Auth");
            }

            var result = isBuyerAuthorized(user.Role);
            if(result == false)
            {
                TempData["ErrorMessage"] = "You are not authorized to Access this Page";
                return RedirectToAction("SetTempData", "Auth");
            }

            var favoriteProductIds = _appDbContext.Favors
                .Where(f => f.UserId == user.UserId)
                .Select(f => f.ProductId)
                .ToList();

            var favoriteProperties = _appDbContext.Products
                .Where(p => favoriteProductIds.Contains(p.ProductId))
                .OrderByDescending(p => p.CreatedAt)
                .Take(3)
                .ToList();

            ViewBag.FavCount = favoriteProductIds.Count;
            ViewBag.unreadMessages = _appDbContext.Messages.Where(m => m.ReceiverId == user.UserId && m.IsRead == 0).Count();

            var viewModel = new ViewModels
            {
                User = user,
                Products = favoriteProperties,
            };

            return View(viewModel);
        }

        public IActionResult FavoriteProperties()
        {
            var user = GetCurrentloggedUser();
            if(user == null)
            {
                TempData["ErrorMessage"] = "Please Login First";    
                return RedirectToAction("SetTempData", "Auth");
            }

            var result = isBuyerAuthorized(user.Role);
            if(result == false)
            {
                TempData["ErrorMessage"] = "You are not authorized to Access this Page";
                return RedirectToAction("SetTempData", "Auth");
            }

            var favoriteProductIds = _appDbContext.Favors.Where(f => f.UserId == user.UserId).Select(f => f.ProductId).ToList();

            var favoriteProperties = _appDbContext.Products
                .Where(p => favoriteProductIds.Contains(p.ProductId) )
                .OrderByDescending(p => p.CreatedAt)
                .ToList();

            ViewBag.FavCount = favoriteProductIds.Count;
            
            var viewModel = new ViewModels
            {
                User = user,
                Products = favoriteProperties,
            };

            return View(viewModel);
        }
        
        public IActionResult RemoveFavorite(string id)
        {
            try
            {
                var user = GetCurrentloggedUser();
                if (user == null)
                {
                    TempData["ErrorMessage"] = "Please Login First";
                    return RedirectToAction("Login", "Auth");
                }


                var result = isBuyerAuthorized(user.Role);
                if (result == false)
                {
                    TempData["ErrorMessage"] = "You are not authorized to Access this Page";
                    return RedirectToAction("SetTempData", "Auth");
                }

                Console.WriteLine("Product Id"+ id);

                var favorite = _appDbContext.Favors.FirstOrDefault(f => f.UserId == user.UserId && f.ProductId == id);
                Console.WriteLine("Fav : " + favorite.ProductId);
                if (favorite != null)
                {
                    var product = _appDbContext.Products.FirstOrDefault(p => p.ProductId == id);
                    product.FavorCount--;
                    _appDbContext.Favors.Remove(favorite);
                    _appDbContext.SaveChanges();

                    TempData["SuccessMessage"] = "Property removed from favorites";
                    return RedirectToAction("FavoriteProperties");
                }


                TempData["Error"] = "Property Failed Removing from favorites";
                return RedirectToAction("FavoriteProperties");
            }
            catch(Exception ex)
            {
                TempData["Error"] = "Property Failed Removing from favorites" +ex.Message;
                return RedirectToAction("FavoriteProperties");
            }
            
            
        }
       

        public IActionResult BuyerProfile()
        {
             var user = GetCurrentloggedUser();
            if(user == null)
            {
                TempData["ErrorMessage"] = "Please Login First";
                return RedirectToAction("Login", "Auth");
            }

            var result = isBuyerAuthorized(user.Role);
            if(result == false)
            {
                TempData["ErrorMessage"] = "You are not authorized to Access this Page";
                return RedirectToAction("SetTempData", "Auth");
            }

            ViewBag.FavCount = _appDbContext.Favors.Where(f => f.UserId == user.UserId).Count();
            var viewModel = new ViewModels
            {
                User = user
            };

            return View(viewModel);
        }

         public async Task<IActionResult> UpdateProfile(IFormCollection form)
        {
            var user = GetCurrentloggedUser();
            if(user == null)
            {
                TempData["ErrorMessage"] = "Please Login First";
                return RedirectToAction("Login", "Auth");
            }

            var result = isBuyerAuthorized(user.Role);
            if(result == false)
            {
                TempData["ErrorMessage"] = "You are not authorized to Access this Page";
                return RedirectToAction("SetTempData", "Auth");
            }

           try{

                var imageFile = form.Files["Image"];

                var buyerImageFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "Buyer");

                if(!Directory.Exists(buyerImageFolder))
                {
                Directory.CreateDirectory(buyerImageFolder);
                }

                var oldImage = user.Image ;

                    if(imageFile != null)
                    {
                        var imageFileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                        var filePath = Path.Combine(buyerImageFolder, imageFileName);
                        using(var stream = new FileStream(filePath, FileMode.Create))
                        {
                        await imageFile.CopyToAsync(stream);
                    }
                        user.Image = "/uploads/Buyer/" + imageFileName;
                    }else
                    {
                        user.Image = oldImage;
                    }

                    var newName = form["Name"].ToString();
                    var newPhone = form["Phone"].ToString();

                    user.Name = newName != null ? newName : user.Name;
                    user.Phone = newPhone != null ? newPhone : user.Phone;
                    user.UpdatedAt = DateTime.UtcNow;

                    _appDbContext.SaveChanges();
                    TempData["SuccessMessage"] = "Profile updated successfully";
                    return RedirectToAction("BuyerProfile");
           }
           catch(Exception ex)
           {
           TempData["ErrorMessage"] = "Error updating profile: " + ex.Message;
            return RedirectToAction("BuyerProfile");
           }
        }
    }
}
