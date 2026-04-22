using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using webProgramming.Models;

namespace webProgramming.Controllers
{
    public class AdminController : BaseController
    {
        public AdminController(AppDbContext Context) : base(Context){

        }


        public IActionResult Index()
        {
            var result = RedirectToLogIfNotLoggedinOrAuthorized(UserRole.Admin);
            if(result is not OkResult)
            return result;

            var user = GetCurrentloggedUser();
            if(user == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            var users = _appDbContext.Users.ToList();
            var properties = _appDbContext.Products.Where(p => p.IsFeatured).ToList(); // العقارات المميزة
            var allProperties = _appDbContext.Products.ToList(); // جميع العقارات
            ViewBag.PropertyCount = _appDbContext.Products.Count();
            ViewBag.ArticlesCount = _appDbContext.Articles.Count();
            ViewBag.ContactCount = _appDbContext.Contacts.Count();
            ViewBag.UserCount = _appDbContext.Users.Count();
            var viewModel = new ViewModels(){
                User = user,
                Users = users,
                Products = properties, // العقارات المميزة للأنشطة الحديثة
                AllProducts = allProperties // جميع العقارات للإحصائيات
            };
            return View(viewModel);
        }

        public IActionResult ShowSellers(){
            var result = RedirectToLogIfNotLoggedinOrAuthorized(UserRole.Admin);
            if(result is not OkResult)
            return result;

            var user = GetCurrentloggedUser();
            if(user == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var sellers = _appDbContext.Users.Where(u => u.Role == UserRole.Seller).ToList();
            var products = _appDbContext.Products.ToList();
            var viewModel = new ViewModels(){
                User = user,
                Sellers = sellers,
                Products = products
            };
            return View(viewModel);
        }

        public IActionResult ShowBuyers()
        {
            var result = RedirectToLogIfNotLoggedinOrAuthorized(UserRole.Admin);
            if(result is not OkResult)
            return result;

            var user = GetCurrentloggedUser();
            if(user == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var buyers = _appDbContext.Users.Where(u => u.Role == UserRole.Buyer).ToList();
            var viewModel = new ViewModels(){
                User = user,
                Buyers = buyers
            };
            return View(viewModel); 
        }

        [HttpPost]
        public IActionResult ToggleUserStatus(IFormCollection form)
        {
            var result = RedirectToLogIfNotLoggedinOrAuthorized(UserRole.Admin);
            if(result is not OkResult)
            return result;

            var loggedUser = GetCurrentloggedUser();
            if(loggedUser == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var userId = form["userId"].ToString();
            Console.WriteLine("Current user to block userId: " + userId);
            var user = _appDbContext.Users.FirstOrDefault(u => u.UserId == userId);
            if (user == null)
            { 
                TempData["ErrorMessage"] = "المستخدم غير موجود";
                return RedirectToAction("Index");
            }

            Console.WriteLine("user.IsBlocked: " + user.IsBlocked);

            user.IsBlocked = !user.IsBlocked;
            user.BlockReason = form["blockReason"];
            user.BlockedBy = loggedUser.Id;
            _appDbContext.SaveChanges();

            if(user.Role == UserRole.Seller)
            {
                BlckProp(user.UserId);
                TempData["SuccessMessage"] = "تم تحديث حالة البائع";
                return RedirectToAction("ShowSellers");
            }
            else{
                 TempData["SuccessMessage"] = "تم تحديث حالة المشتري";
                return RedirectToAction("ShowBuyers");
            }
        }

        public void BlckProp(string id)
        {
            var user = _appDbContext.Users.FirstOrDefault(u => u.UserId == id);
            if (user.IsBlocked)
            {
                var blockProp = _appDbContext.Products.Where(p => p.SellerId == user.UserId);
                foreach (var item in blockProp)
                {
                    Console.WriteLine("item id" + item.ProductId);
                    item.isAccepted = PropertyStatus.Pending;
                }
                _appDbContext.SaveChanges();

            }
            else
            {
                var blockProp = _appDbContext.Products.Where(p => p.SellerId == user.UserId);
                foreach (var item in blockProp)
                {
                    Console.WriteLine("item id" + item.ProductId);
                    item.isAccepted = PropertyStatus.Approved;
                }
                _appDbContext.SaveChanges();
            }
        }

        public IActionResult ShowProperties(string status)
        {

            var user = GetCurrentloggedUser();
            if(user == null)
            {
                TempData["ErrorMessage"] = "يرجى تسجيل الدخول أولاً";
                return RedirectToAction("Login", "Auth"); 
            }

            var result = RedirectToLogIfNotLoggedinOrAuthorized(UserRole.Admin);
            if (result is not OkResult)
                return result;
                

                List<Product> myProducts;
                if(status == null)
                {
                    myProducts = _appDbContext.Products.OrderByDescending(p => p.CreatedAt).ToList();
                }
                else{
                    var propertyStatus = (PropertyStatus)Enum.Parse(typeof(PropertyStatus),status);
                    myProducts = _appDbContext.Products.Where( p => p.isAccepted == propertyStatus).ToList();
                }

                ViewBag.PropertyCount = _appDbContext.Products.Count();
                var viewModel = new ViewModels
                {
                    User = user,
                    Products = myProducts
                };

            return View(viewModel);
        }

       
        public IActionResult TogglePropertyStatus(IFormCollection form)
        {
            Console.WriteLine("this is TogglePropertyStatus");
            var result = RedirectToLogIfNotLoggedinOrAuthorized(UserRole.Admin);
            if(result is not OkResult)
            return result;

            var loggedUser = GetCurrentloggedUser();
            if(loggedUser == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var propertyId = form["productId"].ToString();
            Console.WriteLine("this is propertyId: " + propertyId);
            var property = _appDbContext.Products.FirstOrDefault(p => p.ProductId == propertyId);
            Console.WriteLine("this is property: " + property.Title);
            if (property == null)
            {
                TempData["ErrorMessage"] = "العقار غير موجود";
                return RedirectToAction("ShowProperties");
            }

           if(property.isAccepted == PropertyStatus.Pending || property.isAccepted == PropertyStatus.Rejected)
           {
            property.isAccepted = PropertyStatus.Approved;
            property.BlockedReason = string.Empty;
            Console.WriteLine("this is property.isAccepted: " + property.isAccepted);
           }
           else{
            property.isAccepted = PropertyStatus.Rejected;
            property.BlockedReason = form["blockReason"];
            Console.WriteLine("this is property.BlockedReason: " + property.BlockedReason);
           }

           _appDbContext.SaveChanges();
            
            TempData["SuccessMessage"] = "تم تحديث حالة العقار";
            return RedirectToAction("ShowProperties");          
        }

        public IActionResult FeaturedProperties()
        {
            var result = RedirectToLogIfNotLoggedinOrAuthorized(UserRole.Admin);
            if(result is not OkResult)
                return result;

            var user = GetCurrentloggedUser();
            if(user == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var featuredProperties = _appDbContext.Products.Where(p => p.IsFeatured).ToList();
            var sellers = _appDbContext.Users.Where(u => u.Role == UserRole.Seller).ToList();
            var viewModel = new ViewModels(){
                User = user,
                Products = featuredProperties,
                Sellers = sellers
            };
            return View(viewModel);
        }

        [HttpPost]
        public IActionResult ToggleFeatured(IFormCollection form)
        {
            var result = RedirectToLogIfNotLoggedinOrAuthorized(UserRole.Admin);
            if(result is not OkResult)
                return result;

            var loggedUser = GetCurrentloggedUser();
            if(loggedUser == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var productId = form["productId"].ToString();
            var property = _appDbContext.Products.FirstOrDefault(p => p.ProductId == productId);
            if (property == null)
            {
                TempData["ErrorMessage"] = "العقار غير موجود";
                return RedirectToAction("ShowProperties");
            }

            property.IsFeatured = !property.IsFeatured;
            _appDbContext.SaveChanges();
            
            TempData["SuccessMessage"] = property.IsFeatured ? "تم تمييز العقار" : "تم إلغاء تمييز العقار";
            return RedirectToAction("FeaturedProperties");
        }

        public IActionResult UserProfile(string userId)
        {
            var result = RedirectToLogIfNotLoggedinOrAuthorized(UserRole.Admin);
            if(result is not OkResult)
                return result;

            var loggedUser = GetCurrentloggedUser();
            if(loggedUser == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var user = _appDbContext.Users.FirstOrDefault(u => u.UserId == userId);
            if (user == null)
            {
                TempData["ErrorMessage"] = "المستخدم غير موجود";
                return RedirectToAction("Index");
            }

            var viewModel = new ViewModels(){
                User = loggedUser,
                Seller = user,
                Products = user.Role == UserRole.Seller ? _appDbContext.Products.Where(p => p.SellerId == user.UserId).ToList() : null
            };
            return View(viewModel);
        }

        public IActionResult ShowArticles(string status)
        {

            var user = GetCurrentloggedUser();
            if(user == null)
            {
                TempData["ErrorMessage"] = "يرجى تسجيل الدخول أولاً";
                return RedirectToAction("Login", "Auth"); 
            }

            var result = RedirectToLogIfNotLoggedinOrAuthorized(UserRole.Admin);
            if (result is not OkResult)
                return result;
                

                List<Article> myArticles;
                if(status == null)
                {
                    myArticles = _appDbContext.Articles.ToList();
                }
                else{
                    var articleStatus = bool.Parse(status);
                    myArticles = _appDbContext.Articles.Where( a => a.IsPublished == articleStatus).ToList();
                }

                ViewBag.ArticleCount = _appDbContext.Articles.Count();
                var viewModel = new ViewModels
                {
                    User = user,
                    Articles = myArticles
                };

            return View(viewModel);
        }

    
        public IActionResult ToggleArticleStatus(int id)
        {
            var result = RedirectToLogIfNotLoggedinOrAuthorized(UserRole.Admin);
            if(result is not OkResult)
            return result;

            var loggedUser = GetCurrentloggedUser();
            if(loggedUser == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var article = _appDbContext.Articles.FirstOrDefault(a => a.Id == id);
            if (article == null)
            {
                TempData["ErrorMessage"] = "Article not found";
                return RedirectToAction("ShowArticles");
            }

            article.IsPublished = !article.IsPublished;
            _appDbContext.SaveChanges();

            TempData["SuccessMessage"] = "تم تحديث حالة المقالة";
            return RedirectToAction("ShowArticles");
            
        }
            
        public IActionResult AboutUs()
        {
            var result = RedirectToLogIfNotLoggedinOrAuthorized(UserRole.Admin);
            if(result is not OkResult)
            return result;

            var user = GetCurrentloggedUser();
            if(user == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var viewModel = new ViewModels
            {
                User = user,
                AboutUs = _appDbContext.AboutUs.Find(1)
            };

            return View(viewModel);
            
        }

         public async Task<IActionResult> UpdateAboutUs(IFormCollection form)
        {
            var result = RedirectToLogIfNotLoggedinOrAuthorized(UserRole.Admin);
            if(result is not OkResult)
            return result;

            var user = GetCurrentloggedUser();
            if(user == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var existingAboutUs = _appDbContext.AboutUs.Find(1);

            var Image = form.Files["image"];
            var des = form["description"].ToString();
            Console.WriteLine("this is des: " + des);
            existingAboutUs.Description = des;
            
             if(Image != null && Image.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "aboutus");
                if(!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }
                var uniqueName = Guid.NewGuid().ToString() + Path.GetExtension(Image.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueName);
                using(var stream = new FileStream(filePath, FileMode.Create))
                {
                    await Image.CopyToAsync(stream);
                }
                existingAboutUs.Image = "/uploads/aboutus/" + uniqueName;
            }

            _appDbContext.SaveChanges();

            return RedirectToAction("AboutUs");
            
        }

         public IActionResult Contacts()
        {
            var result = RedirectToLogIfNotLoggedinOrAuthorized(UserRole.Admin);
                if(result is not OkResult) { return result; }

            var user = GetCurrentloggedUser();
            if(user == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var contacts = _appDbContext.Contacts.OrderByDescending(c => c.CreatedAt).ToList();

            var viewModels = new ViewModels
            {
                User = user,
                Contacts = contacts
            };

            return View(viewModels);
        }

        public IActionResult RemoveContact(int id)
        {
            var result = RedirectToLogIfNotLoggedinOrAuthorized(UserRole.Admin);
            if (result is not OkResult) { return result; }

            var user = GetCurrentloggedUser();
            if (user == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var contact = _appDbContext.Contacts.Find(id);

            if(contact != null)
            {
                _appDbContext.Remove(contact);
                _appDbContext.SaveChanges();
                TempData["SuccessMessage"] = "تم حذف الاتصال";
                return RedirectToAction("Contacts");
            }

            TempData["ErrorMessage"] = "الاتصال غير موجود";
            return RedirectToAction("Contacts");
        }
         public IActionResult AdminProfile()
        {
             var result = RedirectToLogIfNotLoggedinOrAuthorized(UserRole.Admin);
            if(result is not OkResult)
            return result;

            var user = GetCurrentloggedUser();
            if(user == null)
            {
                TempData["ErrorMessage"] = "يرجى تسجيل الدخول أولاً";
                return RedirectToAction("Login", "Auth");
            }

            var viewModel = new ViewModels
            {
                User = user
            };

            return View(viewModel);
        }

         public async Task<IActionResult> UpdateProfile(IFormCollection form)
        {
             var result = RedirectToLogIfNotLoggedinOrAuthorized(UserRole.Admin);
                if(result is not OkResult)
                return result;
            
            var user = GetCurrentloggedUser();
            if(user == null)
            {
                TempData["ErrorMessage"] = "يرجى تسجيل الدخول أولاً";
                return RedirectToAction("Login", "Auth");
            }

           try{

                var imageFile = form.Files["Image"];

                var adminImageFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "Admin");

                if(!Directory.Exists(adminImageFolder))
                {
                Directory.CreateDirectory(adminImageFolder);
                }

                var oldImage = user.Image ;

                    if(imageFile != null)
                    {
                        var imageFileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                        var filePath = Path.Combine(adminImageFolder, imageFileName);
                        using(var stream = new FileStream(filePath, FileMode.Create))
                        {
                        imageFile.CopyTo(stream);
                    }
                        user.Image = "/uploads/Admin/" + imageFileName;
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
                    return RedirectToAction("AdminProfile");
           }
           catch(Exception ex)
           {
           TempData["ErrorMessage"] = "Error updating profile: " + ex.Message;
            return RedirectToAction("AdminProfile");
           }
        }
  

        public IActionResult Settings()
        {
            var result = RedirectToLogIfNotLoggedinOrAuthorized(UserRole.Admin);
            if(result is not OkResult)
            return result;

            var user = GetCurrentloggedUser();
            if(user == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var viewModel = new ViewModels
            {
                User = user,
                Settings = _appDbContext.Settings.Find(1)
            };

            return View(viewModel);
        }

        public async Task<IActionResult> UpdateSettings(IFormCollection form)
        {
            var result = RedirectToLogIfNotLoggedinOrAuthorized(UserRole.Admin);
            if(result is not OkResult)
            return result;

            var user = GetCurrentloggedUser();
            if(user == null)
            {
                return RedirectToAction("Login", "Auth");
            }

           var settings = _appDbContext.Settings.Find(1);

           settings.SiteAddress = form["siteAddress"].ToString();
           settings.SiteEmail = form["siteEmail"].ToString();
           settings.SitePhone = form["sitePhone"].ToString();  
           settings.CopyrightText = form["copyrightText"].ToString();
           
           var logoImage = form.Files["LogoImage"];
           if(logoImage != null && logoImage.Length > 0)
           {
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "settings");
            if(!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }
            var uniqueName = Guid.NewGuid().ToString() + Path.GetExtension(logoImage.FileName);
            var filePath = Path.Combine(uploadsFolder, uniqueName);
            using(var stream = new FileStream(filePath, FileMode.Create))
            {
               await logoImage.CopyToAsync(stream);
            }
            settings.LogoImage = "/uploads/settings/" + uniqueName;
           }
           
           var footerLogoImage = form.Files["FooterLogoImage"];
           if(footerLogoImage != null && footerLogoImage.Length > 0)
           {
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "settings");
            if(!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }
            var uniqueName = Guid.NewGuid().ToString() + Path.GetExtension(footerLogoImage.FileName);
            var filePath = Path.Combine(uploadsFolder, uniqueName);
            using(var stream = new FileStream(filePath, FileMode.Create))
            {
                await footerLogoImage.CopyToAsync(stream);
            }
            settings.FooterLogoImage = "/uploads/settings/" + uniqueName;
           }
           
           var socialLinks = new
           {
             facebook = form["facebookLink"].ToString(),
             twitter = form["twitterLink"].ToString(),
             instagram = form["instagramLink"].ToString(),
             whatsapp = form["whatsappLink"].ToString()
           };
           settings.SocialMediaLink = JsonConvert.SerializeObject(socialLinks);
           
           _appDbContext.SaveChanges();
            return RedirectToAction("Settings");
        }
    
    }
}
