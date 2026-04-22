using System.Collections;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using webProgramming.Models;

namespace webProgramming.Controllers;

public class HomeController : FunctionalController
{

    public HomeController(AppDbContext Context) : base(Context)
    {
    }
public IActionResult Index(int? cityId)
{
    var Sellers = _appDbContext.Users.Where(u => u.Role == UserRole.Seller).ToList();

    var approvedProducts = _appDbContext.Products
        .Where(p => p.isAccepted == PropertyStatus.Approved
                 && p.IsFeatured
                 && (p.Type == PropertyType.Sale || p.Type == PropertyType.Rent));

    // فلترة حسب المدينة لو فيه cityId
    if (cityId != null)
    {
        approvedProducts = approvedProducts.Where(p => p.CityId == cityId);
        ViewBag.SelectedCity = _appDbContext.Cities.FirstOrDefault(c => c.Id == cityId)?.Name;
    }

    var articles = _appDbContext.Articles
        .Where(a => a.IsPublished == true)
        .OrderBy(a => a.ViewsCount)
        .Take(3)
        .ToList();

    var Cities = _appDbContext.Cities.ToList();
    var Neighbs = _appDbContext.Neighs.ToList();
    var Favors = _appDbContext.Favors.ToList();
    var LoggedUser = GetCurrentloggedUser();
    var settings = _appDbContext.Settings.FirstOrDefault();

    var amenities = new List<Amenity>
    {
        new Amenity { Name = "تكييف هواء", FeatureName = "air_conditioning_feature", DisplayName = "تكييف هواء", IconClass= "fas fa-snowflake" },
        new Amenity { Name = "مسبح", FeatureName = "swimming_pool_feature", DisplayName = "مسبح", IconClass= "fas fa-swimming-pool" },
        new Amenity { Name = "ميكروويف", FeatureName = "microwave_feature", DisplayName = "ميكروويف", IconClass= "fas fa-microwave" },
        new Amenity { Name = "شرفة", FeatureName = "balcony_feature", DisplayName = "شرفة", IconClass= "fas fa-building" },
        new Amenity { Name = "صالة رياضية", FeatureName = "gym_feature", DisplayName = "صالة رياضية", IconClass= "fas fa-dumbbell" },
        new Amenity { Name = "دش / حمام", FeatureName = "shower_feature", DisplayName = "دش / حمام", IconClass= "fas fa-shower" },
        new Amenity { Name = "غسالة", FeatureName = "washer_feature", DisplayName = "غسالة", IconClass= "fas fa-soap" },
        new Amenity { Name = "ستائر نوافذ", FeatureName = "window_coverings_feature", DisplayName = "ستائر نوافذ", IconClass= "fas fa-window-maximize" },
        new Amenity { Name = "واي فاي", FeatureName = "wifi_feature", DisplayName = "واي فاي", IconClass= "fas fa-wifi" },
        new Amenity { Name = "ثلاجة", FeatureName = "refrigerator_feature", DisplayName = "ثلاجة", IconClass= "fas fa-snowflake" },
        new Amenity { Name = "تلفاز كابل", FeatureName = "tv_cable_feature", DisplayName = "تلفاز كابل", IconClass= "fas fa-tv" },
        new Amenity { Name = "غسيل ملابس", FeatureName = "laundry_feature", DisplayName = "غسيل ملابس", IconClass= "fas fa-tshirt" },
        new Amenity { Name = "حديقة", FeatureName = "garden_feature", DisplayName = "حديقة", IconClass= "fas fa-leaf" },
        new Amenity { Name = "كراج / موقف سيارات", FeatureName = "garage_feature", DisplayName = "كراج / موقف سيارات", IconClass= "fas fa-car" },
    };

    var viewModels = new ViewModels()
    {
        Sellers = Sellers,
        Products = approvedProducts.OrderBy(p => p.ProductId).Take(5).ToList(),
        Cities = Cities,
        Neighbs = Neighbs,
        Favors = Favors,
        User = LoggedUser,
        Articles = articles,
        Settings = settings,
        Amenities = amenities,
    };

    return View(viewModels);
}public IActionResult Listing()
{
    // ✅ جلب كل العقارات سواء مقبولة أو لا
    var allProducts = _appDbContext.Products
        .Where(p => p.Type == PropertyType.Sale || p.Type == PropertyType.Rent)
        .ToList();

    var Cities = _appDbContext.Cities.ToList();
    var Neighbs = _appDbContext.Neighs.ToList();
    var Favors = _appDbContext.Favors.ToList();
    var LoggedUser = GetCurrentloggedUser();
    var sellers = _appDbContext.Users.Where(u => u.Role == UserRole.Seller).ToList();
    var settings = _appDbContext.Settings.FirstOrDefault();

    var amenities = new List<Amenity>
    {
        new Amenity { Name = "تكييف هواء", FeatureName = "air_conditioning_feature", DisplayName = "تكييف هواء", IconClass= "fas fa-snowflake" },
        new Amenity { Name = "مسبح", FeatureName = "swimming_pool_feature", DisplayName = "مسبح", IconClass= "fas fa-swimming-pool" },
        new Amenity { Name = "ميكروويف", FeatureName = "microwave_feature", DisplayName = "ميكروويف", IconClass= "fas fa-microwave" },
        new Amenity { Name = "شرفة", FeatureName = "balcony_feature", DisplayName = "شرفة", IconClass= "fas fa-building" },
        new Amenity { Name = "صالة رياضية", FeatureName = "gym_feature", DisplayName = "صالة رياضية", IconClass= "fas fa-dumbbell" },
        new Amenity { Name = "دش / حمام", FeatureName = "shower_feature", DisplayName = "دش / حمام", IconClass= "fas fa-shower" },
        new Amenity { Name = "غسالة", FeatureName = "washer_feature", DisplayName = "غسالة", IconClass= "fas fa-soap" },
        new Amenity { Name = "ستائر نوافذ", FeatureName = "window_coverings_feature", DisplayName = "ستائر نوافذ", IconClass= "fas fa-window-maximize" },
        new Amenity { Name = "واي فاي", FeatureName = "wifi_feature", DisplayName = "واي فاي", IconClass= "fas fa-wifi" },
        new Amenity { Name = "ثلاجة", FeatureName = "refrigerator_feature", DisplayName = "ثلاجة", IconClass= "fas fa-snowflake" },
        new Amenity { Name = "تلفاز كابل", FeatureName = "tv_cable_feature", DisplayName = "تلفاز كابل", IconClass= "fas fa-tv" },
        new Amenity { Name = "غسيل ملابس", FeatureName = "laundry_feature", DisplayName = "غسيل ملابس", IconClass= "fas fa-tshirt" },
        new Amenity { Name = "حديقة", FeatureName = "garden_feature", DisplayName = "حديقة", IconClass= "fas fa-leaf" },
        new Amenity { Name = "كراج / موقف سيارات", FeatureName = "garage_feature", DisplayName = "كراج / موقف سيارات", IconClass= "fas fa-car" },
    };

    var viewModels = new ViewModels()
    {
        AllProducts = allProducts, // ✅ الكل
        Products = allProducts,    // لو تبغى تستخدم نفس الخاصية القديمة
        Favors = Favors,
        Sellers = sellers,
        User = LoggedUser,
        Cities = Cities,
        Neighbs = Neighbs,
        Settings = settings,
        Amenities = amenities
    };

    ViewBag.QueryString = Request.QueryString.ToString();

    return View(viewModels);
}
    public IActionResult About()
    {
        var LoggedUser = GetCurrentloggedUser();
        var about = _appDbContext.AboutUs.FirstOrDefault();
        var settings = _appDbContext.Settings.FirstOrDefault();
        ViewBag.Count = _appDbContext.Products.Where(p => p.Type == PropertyType.Sold).Count();
        var viewModel = new ViewModels()
        {
            User = LoggedUser,
            AboutUs = about,
            Settings = settings
        };
        return View(viewModel);
    }

    public IActionResult Agents()
    {
        var LoggedUser = GetCurrentloggedUser();
        var settings = _appDbContext.Settings.FirstOrDefault();
        var agents = _appDbContext.Users.Where(u => u.Role == UserRole.Seller).ToList();
        var viewModel = new ViewModels()
        {
            Sellers = agents,
            User = LoggedUser,
            Settings = settings
        };
        return View(viewModel);
    }

    public IActionResult AgentDetail(string id)
    {
        var LoggedUser = GetCurrentloggedUser();
        var settings = _appDbContext.Settings.FirstOrDefault();
        var agent = _appDbContext.Users.FirstOrDefault(u => u.UserId == id && u.Role == UserRole.Seller);
        var products = _appDbContext.Products.Where(p => p.SellerId == id && p.isAccepted == PropertyStatus.Approved).OrderByDescending(p => p.FavorCount).Take(3).ToList();
        var articles = _appDbContext.Articles.Where(a => a.SellerId == id && a.IsPublished == true).OrderByDescending(a => a.ViewsCount).Take(3).ToList();
        if(agent == null)
        {
            return View("PageNotFound");
        }
        var viewModel = new ViewModels()
        {
            User = LoggedUser,
            Settings = settings,
            Seller = agent,
            Products = products,
            Articles = articles
        };
        return View(viewModel);
    }
    public IActionResult pageNotFound()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    [HttpPost]
    public IActionResult ToggleFavorite(string productId)
    {
        try
        {

            var user = GetCurrentloggedUser();

            if (user == null)
            {
                return Json(new { success = false, message = "User not authenticated, please login first", showSweetAlert = true, title = "Authentication Required", icon = "warning" });
            }

            var product = _appDbContext.Products.FirstOrDefault(p => p.ProductId == productId);
            if (product == null)
            {
                return Json(new { success = false, message = "Product not found" });
            }

            var existingFavorite = _appDbContext.Favors.FirstOrDefault(f => f.ProductId == productId && f.UserId == user.UserId);

            if (existingFavorite != null)
            {
                _appDbContext.Favors.Remove(existingFavorite);
                product.FavorCount = product.FavorCount == 0 ? 0 : product.FavorCount - 1; 
            }
            else
            {
                _appDbContext.Favors.Add(new Favor
                {
                    ProductId = productId,
                    UserId = user.UserId
                });
                product.FavorCount++;
            }

            _appDbContext.SaveChanges();
            return Json(new { 
                success = true,
                isFavorited = existingFavorite == null,
                newCount = product.FavorCount,
                showSweetAlert = true,
                title = existingFavorite == null ? "Added to Favorites" : "Removed from Favorites", 
                icon = "success",
                message = existingFavorite == null ? "Property has been added to your favorites" : "Property has been removed from your favorites"
            });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpPost]public IActionResult Search(IFormCollection form)
{
    if (form == null)
        return RedirectToAction("Listing");

    // ===============================
    // قراءة القيم بأمان
    // ===============================
    PropertyType propertyType = 0;
    HouseType houseType = 0;
    int cityId = 0;
    int neighbId = 0;

    int.TryParse(form["propertyType"], out int propTypeValue);
    propertyType = (PropertyType)propTypeValue;

    int.TryParse(form["houseType"], out int houseTypeValue);
    houseType = (HouseType)houseTypeValue;

    int.TryParse(form["cityId"], out cityId);
    int.TryParse(form["neighbId"], out neighbId);

    // ===============================
    // الأساس: عقارات مقبولة فقط
    // ===============================
    var query = _appDbContext.Products
        .Where(p => p.isAccepted == PropertyStatus.Approved)
        .AsQueryable();

    // ===============================
    // فلترة حسب الاختيارات
    // ===============================
    if (propertyType != 0)
        query = query.Where(p => p.Type == propertyType);

    if (houseType != 0)
        query = query.Where(p => p.EstateType == houseType);

    if (cityId != 0)
        query = query.Where(p => p.CityId == cityId);

    if (neighbId != 0)
        query = query.Where(p => p.NeighbsId == neighbId);

    // ===============================
    // فلترة إضافية (السعر – المساحة – إلخ)
    // ===============================
    var filteredProducts = FilterProducts(query, form);

    // ===============================
    // ViewModel
    // ===============================
    var viewModels = new ViewModels
    {
        FilteredProducts = filteredProducts,
        Products = filteredProducts, // نفس النتائج
        Sellers = _appDbContext.Users
                    .Where(u => u.Role == UserRole.Seller)
                    .ToList(),
        Favors = _appDbContext.Favors.ToList(),
        User = GetCurrentloggedUser(),
        Cities = _appDbContext.Cities.ToList(),
        Neighbs = _appDbContext.Neighs.ToList(),
        Settings = _appDbContext.Settings.Find(1)
    };

    return View("Listing", viewModels);
}

    public IActionResult PropDetail(string id)
    {
        try
        {
            var properity = _appDbContext.Products.FirstOrDefault(p => p.ProductId == id);
            if (properity != null)
            {
                 var seller = _appDbContext.Users.FirstOrDefault(s => s.UserId == properity.SellerId);
                 var city = _appDbContext.Cities.FirstOrDefault(c => c.Id == properity.CityId);
                 var neighb = _appDbContext.Neighs.FirstOrDefault(c => c.Id == properity.NeighbsId);

                 var relatedProperities = seller != null ? _appDbContext.Products.Where(
                    p => p.isAccepted == PropertyStatus.Approved && p.SellerId == seller.UserId
                    && (p.Type == PropertyType.Sale || p.Type == PropertyType.Rent)
                    ).Take(3).ToList() : new List<Product>();
                    
                var loggedUser = GetCurrentloggedUser();
                var relatedChats = _appDbContext.Chats.Where(c => c.SellerId == seller.UserId).ToList();
                var favors = _appDbContext.Favors.ToList();

                var settings = _appDbContext.Settings.FirstOrDefault();
                var viewModel = new ViewModels()
                {
                    Product = properity,
                    Products = relatedProperities,
                    Favors = favors,
                    User = loggedUser,
                    Seller = seller,
                    City = city,
                    Chats = relatedChats,
                    Neighb = neighb,
                    Settings = settings
                };

                return View(viewModel);
                
            }else
            {
                TempData["Wrong"] = "There is No Data for This Product" + id;
                return View();
            }
           

           
        }
        catch(Exception ex)
        {
            TempData["Wrong"] = "Technicall Error Happended While Cllaing The Properity with id: " + id + ex.Message;
            return View();
        }

    }

    public IActionResult Articles()
    {
        var articles = _appDbContext.Articles.Where(a => a.IsPublished == true).OrderByDescending(a => a.ViewsCount).ToList();
        var user = GetCurrentloggedUser();
        var settings = _appDbContext.Settings.FirstOrDefault();
        var viewModel = new ViewModels()
        {
            Articles = articles,
            User = user,
            Settings = settings
        };
        return View(viewModel);
    }
    public IActionResult ArticleDetail(int id)
    {
        var article = _appDbContext.Articles.FirstOrDefault(a => a.Id == id && a.IsPublished == true);
        if(article == null)
        {
            return View("PageNotFound");
        }
        article.ViewsCount++;
        _appDbContext.SaveChanges();
        var user = GetCurrentloggedUser();
        var seller = _appDbContext.Users.FirstOrDefault(s => s.UserId == article.SellerId);
        var articles = _appDbContext.Articles.Where(a => a.IsPublished == true && a.SellerId == seller.UserId).OrderByDescending(a => a.ViewsCount).Take(3).ToList();
        var settings = _appDbContext.Settings.FirstOrDefault();
        var viewModel = new ViewModels()
        {
            Article = article,
            Seller = seller,
            User = user,
            Articles = articles,
            Settings = settings
        };
        return View(viewModel);

    }

    public IActionResult ContactUs()
    {
        var LoggedUser = GetCurrentloggedUser();
        var settings = _appDbContext.Settings.FirstOrDefault();
        var viewModel = new ViewModels()
        {
            User = LoggedUser,
            Settings = settings
        };

        return View(viewModel);
    }

    public IActionResult StoreContact(IFormCollection form)
    {
        try
        {
            var contact = new Contact()
            {
                Name = form["name"].ToString(),
                Email = form["email"].ToString(),
                Subject = form["subject"].ToString(),
                Message = form["message"].ToString(),
                CreatedAt = DateTime.Now
            };
            _appDbContext.Contacts.Add(contact);
            _appDbContext.SaveChanges();
            TempData["SuccessMessage"] = "Message Sent Successfully";
            return RedirectToAction("ContactUs");
        }
        catch(Exception ex)
        {
            TempData["ErrorMessage"] = "Error Sending Message: " + ex.Message;
            return RedirectToAction("ContactUs");
        }
    }

    public IActionResult Track(IFormCollection form)
    {
        var user = GetCurrentloggedUser();
            
        var prodcutId = form["productId"].ToString();
        Console.WriteLine("Product ID: " + prodcutId);

        var Sellers = _appDbContext.Users.Where(u => u.Role == UserRole.Seller).ToList();
        var Favors = _appDbContext.Favors.ToList();

        var product = _appDbContext.Products.FirstOrDefault(p => p.ProductId == prodcutId);

        if(product == null)
        {
            TempData["ErrorMessage"] = "Wrong Product Id";
            return RedirectToAction("Index");
        }

         var viewModel = new ViewModels()
        {
            Product = product,
            Sellers = Sellers,
            Favors = Favors,
            User = user,
            Settings = _appDbContext.Settings.FirstOrDefault()
        };

        return View(viewModel);

    }
}
