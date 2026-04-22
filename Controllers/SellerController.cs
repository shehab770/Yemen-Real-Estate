using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using webProgramming.Models;

namespace webProgramming.Controllers
{
    public class SellerController : BaseController
    {
        public SellerController(AppDbContext Context) : base(Context)
        {
        }

        public IActionResult Index()
        {
            var user = GetCurrentloggedUser();
            if(user == null)
            {
                TempData["ErrorMessage"] = "Please Login First";
                return RedirectToAction("Login", "Auth");
            }

            var result = RedirectToLogIfNotLoggedinOrAuthorized(UserRole.Seller);
            if(result is not OkResult)
            {
                return result;
            }
            
             ViewBag.FavCount = _appDbContext.Favors.Where(f => f.UserId == user.UserId).Count();
             ViewBag.PropertyCount = _appDbContext.Products.Count(p => p.SellerId == user.UserId);
             var products = _appDbContext.Products.Where(p=>p.SellerId == user.UserId).ToList();
             ViewBag.unreadMessages = _appDbContext.Messages.Where(m => m.ReceiverId == user.UserId && m.IsRead == 0).Count();

            var viewModel = new ViewModels
            {
                User = user,
                Products = products
            };
            return View(viewModel);
        }

        public IActionResult ShowProperties(string status)
        {

            var user = GetCurrentloggedUser();
            if(user == null)
            {
                TempData["ErrorMessage"] = "Please Login First";
                return RedirectToAction("Login", "Auth"); 
            }

            var result = RedirectToLogIfNotLoggedinOrAuthorized(UserRole.Seller);
            if (result is not OkResult)
                return result;

            List<Product> myProducts;
            if (!string.IsNullOrEmpty(Request.Query["featured"]))
            {
                // عرض العقارات المميزة فقط لهذا المستخدم
                myProducts = _appDbContext.Products.Where(p => p.SellerId == user.UserId && p.IsFeatured).ToList();
            }
            else if (status == null)
            {
                myProducts = _appDbContext.Products.Where(p => p.SellerId == user.UserId).ToList();
            }
            else
            {
                var propertyStatus = (PropertyStatus)Enum.Parse(typeof(PropertyStatus), status);
                myProducts = _appDbContext.Products.Where(p => p.SellerId == user.UserId && p.isAccepted == propertyStatus).ToList();
            }

            ViewBag.FavCount = _appDbContext.Favors.Where(f => f.UserId == user.UserId).Count();
            ViewBag.PropertyCount = _appDbContext.Products.Count(p => p.SellerId == user.UserId);
            var viewModel = new ViewModels
            {
                User = user,
                Products = myProducts
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

           var result = RedirectToLogIfNotLoggedinOrAuthorized(UserRole.Seller);
            if(result is not OkResult)
            {
                return result;
            }

            var favoriteProductIds = _appDbContext.Favors
                .Where(f => f.UserId == user.UserId)
                .Select(f => f.ProductId)
                .ToList();

            var favoriteProperties = _appDbContext.Products
                .Where(p => favoriteProductIds.Contains(p.ProductId))
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
            var user = GetCurrentloggedUser();
            if(user == null)
            {
                TempData["ErrorMessage"] = "Please Login First";
                return RedirectToAction("Login", "Auth");
            }

            var result = RedirectToLogIfNotLoggedinOrAuthorized(UserRole.Seller);
            if(result is not OkResult)
            {
                return result;
            }

            var favorite = _appDbContext.Favors.FirstOrDefault(f => f.UserId == user.UserId && f.ProductId == id);
            if(favorite != null)
            {
                var product = _appDbContext.Products.FirstOrDefault(p => p.ProductId == id);
                if(product != null)
                {
                    product.FavorCount--;
                }
                _appDbContext.Favors.Remove(favorite);
                _appDbContext.SaveChanges();
            }
            TempData["SuccessMessage"] = "Property removed from favorites";
            return RedirectToAction("FavoriteProperties");
            
            
        }
        public IActionResult SellerProfile(string userId = null)
        {
            var currentUser = GetCurrentloggedUser();
            if(currentUser == null)
            {
                TempData["ErrorMessage"] = "Please Login First";
                return RedirectToAction("Login", "Auth");
            }

            // إذا لم يتم تمرير userId، اعرض الملف الشخصي للبائع الحالي
            string targetUserId = userId ?? currentUser.UserId;
            var user = _appDbContext.Users.FirstOrDefault(u => u.UserId == targetUserId);
            if(user == null)
            {
                TempData["ErrorMessage"] = "المستخدم غير موجود";
                return RedirectToAction("Index", "Home");
            }

            // السماح فقط للادمن أو نفس البائع
            if(currentUser.Role != UserRole.Admin && currentUser.UserId != user.UserId)
            {
                TempData["ErrorMessage"] = "غير مصرح لك بعرض هذه الصفحة";
                return RedirectToAction("Index", "Home");
            }

            ViewBag.FavCount = _appDbContext.Favors.Where(f => f.UserId == user.UserId).Count();
            ViewBag.PropertyCount = _appDbContext.Products.Count(p => p.SellerId == user.UserId);
            ViewBag.ApprovedCount = _appDbContext.Products.Count(p => p.SellerId == user.UserId && p.isAccepted == PropertyStatus.Approved);
            ViewBag.PendingCount = _appDbContext.Products.Count(p => p.SellerId == user.UserId && p.isAccepted == PropertyStatus.Pending);

            // Get recent properties
            var recentProperties = _appDbContext.Products.Where(p => p.SellerId == user.UserId).OrderByDescending(p => p.CreatedAt).Take(4).ToList();

            var profileDetails = new ViewModels()
            {
                User = user,
                Products = recentProperties
            };

            return View(profileDetails);
        }

        public IActionResult ShowArticles()
        {
            var user = GetCurrentloggedUser();
            if(user == null)
            {
                TempData["ErrorMessage"] = "Please Login First";
                return RedirectToAction("Login", "Auth");
            }
              var result = RedirectToLogIfNotLoggedinOrAuthorized(UserRole.Seller);
               if (result is not OkResult)
                   return result;

            var articles = _appDbContext.Articles.Where(a => a.SellerId == user.UserId).ToList();
            ViewBag.FavCount = _appDbContext.Favors.Where(f => f.UserId == user.UserId).Count();
            ViewBag.PropertyCount = _appDbContext.Products.Count(p => p.SellerId == user.UserId);
            var viewModel = new ViewModels
            {
                User = user,
                Articles = articles
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

            var result = RedirectToLogIfNotLoggedinOrAuthorized(UserRole.Seller);
            if(result is not OkResult)
            {
                return result;
            }

           try{

                var imageFile = form.Files["Image"];
                var officePhotoFile = form.Files["OfficePhoto"];

                var sellerImageFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "Sellers");
                //Set Seller's Path Documents and office photo
                var sellerDocsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "SellerDocs");

                if(!Directory.Exists(sellerImageFolder))
                {
                Directory.CreateDirectory(sellerImageFolder);
                }
                if(!Directory.Exists(sellerDocsFolder))
                {
                 Directory.CreateDirectory(sellerDocsFolder);
                }

                var oldImage = user.Image ;
                var oldOfficePhoto = user.OfficePhoto;

                    if(imageFile != null)
                    {
                        var imageFileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                        var filePath = Path.Combine(sellerImageFolder, imageFileName);
                        using(var stream = new FileStream(filePath, FileMode.Create))
                        {
                        await imageFile.CopyToAsync(stream);
                    }
                        user.Image = "/uploads/Sellers/" + imageFileName;
                    }else
                    {
                        user.Image = oldImage;
                    }

                    if(officePhotoFile != null )
                    {
                        var officePhotoFileName = Guid.NewGuid().ToString() + Path.GetExtension(officePhotoFile.FileName);
                        var officePhotoPath = Path.Combine(sellerDocsFolder, officePhotoFileName);
                        using(var stream = new FileStream(officePhotoPath, FileMode.Create))
                        {
                            await officePhotoFile.CopyToAsync(stream);
                        }
                         user.OfficePhoto = "/uploads/SellerDocs/" + officePhotoFileName;
                    }else
                    {
                        user.OfficePhoto = oldOfficePhoto;
                    }


                    var newName = form["Name"].ToString();
                    var newPhone = form["Phone"].ToString();

                    user.Name = newName != null ? newName : user.Name;
                    user.Phone = newPhone != null ? newPhone : user.Phone;
                    user.UpdatedAt = DateTime.UtcNow;

                    _appDbContext.SaveChanges();
                    TempData["SuccessMessage"] = "Profile updated successfully";
                    return RedirectToAction("SellerProfile");
           }
           catch(Exception ex)
           {
           TempData["ErrorMessage"] = "Error updating profile: " + ex.Message;
            return RedirectToAction("SellerProfile");
           }
        }
    }
}
