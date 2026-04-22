using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json; // For JSON serialization
using Microsoft.EntityFrameworkCore;
using webProgramming.Models;
using static System.Net.Mime.MediaTypeNames;
using System.Reflection.Metadata;

namespace webProgramming.Controllers
{
    public class ArticalsController : FunctionalController
    {
        public ArticalsController(AppDbContext Context) : base(Context)
        {
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Add()
        {
            var result = RedirectToLogIfNotLoggedinOrAuthorized(UserRole.Seller);
            if (result is not OkResult)
            {
                return RedirectToAction("SetTempData", "Auth");
            }
            var user = GetCurrentloggedUser();
            if(user == null){return RedirectToAction("Login", "Auth");}
             ViewBag.PropertyCount = _appDbContext.Products.Count(p => p.SellerId == user.UserId);

             var viewModel = new ViewModels
             {
                User = user,
                Neighbs = _appDbContext.Neighs.ToList(),
                Cities = _appDbContext.Cities.ToList(),
             };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Store(IFormCollection form)
        {
           try{
             var result = RedirectToLogIfNotLoggedinOrAuthorized(UserRole.Seller);
            if (result is not OkResult)
            {
                return RedirectToAction("SetTempData", "Auth");
            }
            var user = GetCurrentloggedUser();
            if(user == null){return RedirectToAction("Login", "Auth");}

            Console.WriteLine("Authorized");

            if(!int.TryParse(form["CityId"],out int cityId)){
                ModelState.AddModelError("CityId", "Invalid City ID");
                return View(form);
            }
              Console.WriteLine("CityId:"+cityId);
            if(!int.TryParse(form["NeighbId"],out int neighbId)){
                ModelState.AddModelError("NeighbId", "Invalid Neighborhood ID");
                return View(form);
            }
            Console.WriteLine("NeighbId:"+neighbId);
            var houseTypeValue = form["houseType"].ToString();
            var houseType = (ArticleHouseType)int.Parse(houseTypeValue);
            Console.WriteLine("HouseType:"+houseType);

            var sellerId = form["sellerId"];
            Console.WriteLine("SellerId:"+sellerId);
            var metaTitle = form["meta_title"];
            Console.WriteLine("MetaTitle:"+metaTitle);
            var metaDescription = form["meta_description"];
            Console.WriteLine("MetaDescription:"+metaDescription);
            var title = form["title"];
            Console.WriteLine("Title:"+title);
            var content = form["content"];

            var artical = new Article
            {
                SellerId = sellerId,
                Title = title,
                Content = content,
                HouseType = houseType,
                CityId = cityId,
                NeighbId = neighbId,
                MetaTitle = metaTitle,
                MetaDescription = metaDescription,
                Image = string.Empty
            };

            var image = form.Files["image"];

            if(image != null && image.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "articles");
                if(!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }
                var uniqueName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueName);

                using(var stream = new FileStream(filePath, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }
                artical.Image = "/uploads/articles/" + uniqueName;
            }else 
            {
                 ModelState.AddModelError("image", "Image is required");
                return View(form);
            }

            await _appDbContext.Articles.AddAsync(artical);
            await _appDbContext.SaveChangesAsync();

            TempData["Success"] = "Article created successfully";
            
            return RedirectToAction("Add");
           }
           catch(Exception ex)
           {
             TempData["Failed"] = "An error occurred while creating the article, Please Make Sure You Fill All The Fields Correctly" + ex.Message;
            return RedirectToAction("Add",form);
           }
        }
        
        public IActionResult Edit(int id)
        {
            var result = RedirectToLogIfNotLoggedinOrAuthorized(UserRole.Seller);
            if (result is not OkResult)
            {
                return RedirectToAction("SetTempData", "Auth");
            }
            var user = GetCurrentloggedUser();
            if(user == null){return RedirectToAction("Login", "Auth");}
            var artical = _appDbContext.Articles.Find(id);
            if(artical == null){return NotFound();}

            var viewModel = new ViewModels
            {
                User = user,
                Neighb = _appDbContext.Neighs.FirstOrDefault(n => n.Id == artical.NeighbId),
                City = _appDbContext.Cities.FirstOrDefault(c => c.Id == artical.CityId),
                Article = artical,
                Cities = _appDbContext.Cities.ToList(),
                Neighbs = _appDbContext.Neighs.ToList()
            };
             ViewBag.PropertyCount = _appDbContext.Products.Count(p => p.SellerId == user.UserId);

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(IFormCollection form,int id)
        {
            try{

                var result = RedirectToLogIfNotLoggedinOrAuthorized(UserRole.Seller);
                if (result is not OkResult)
                {
                    return RedirectToAction("SetTempData", "Auth");
                }
                var user = GetCurrentloggedUser();
                if(user == null){return RedirectToAction("Login", "Auth");}

                var artical = _appDbContext.Articles.Find(id);
                if(artical == null){return NotFound();}

                if(!int.TryParse(form["CityId"],out int cityId)){
                    ModelState.AddModelError("CityId", "Invalid City ID");
                    return View(form);
                }
                if(!int.TryParse(form["NeighbId"],out int neighbId)){
                    ModelState.AddModelError("NeighbId", "Invalid Neighborhood ID");
                    return View(form);
                }
                var houseTypeValue = form["houseType"].ToString();
                var houseType = (ArticleHouseType)int.Parse(houseTypeValue);

                var sellerId = form["sellerId"];
                var metaTitle = form["meta_title"];
                var metaDescription = form["meta_description"];
                var title = form["title"];
                var content = form["content"];

                artical.SellerId = sellerId;
                artical.Title = title;
                artical.Content = content;
                artical.HouseType = houseType;
                artical.CityId = cityId;
                artical.NeighbId = neighbId;
                artical.MetaTitle = metaTitle;
                artical.MetaDescription = metaDescription;

                var image = form.Files["image"];
                if(image != null && image.Length > 0)
                {
                    var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", artical.Image.TrimStart('/'));
                    if(System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }

                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "articles");
                    if(!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }
                    var uniqueName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
                    var filePath = Path.Combine(uploadsFolder, uniqueName);

                    using(var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await image.CopyToAsync(stream);
                    }
                    artical.Image = "/uploads/articles/" + uniqueName;
                }

                await _appDbContext.SaveChangesAsync();
                 TempData["Success"] = "Article updated successfully";
                 return RedirectToAction("ShowArticles","Seller");

            }catch(Exception ex)
            {
                TempData["Failed"] = "An error occurred while updating the article, Please Make Sure You Fill All The Fields Correctly" + ex.Message;
                 return RedirectToAction("ShowArticles","Seller");
            }
        }
        public IActionResult Delete(int id)
        {
            try{
                
                var user = GetCurrentloggedUser();
                if(user == null){return RedirectToAction("Login", "Auth");}

                var result = RedirectToLogIfNotLoggedinOrAuthorized(user.Role);
                if (result is not OkResult)
                {
                    return RedirectToAction("SetTempData", "Auth");
                }

                var artical = _appDbContext.Articles.Find(id);
                if(artical == null){return NotFound();}

                if(artical.Image != null)
                {
                    var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", artical.Image.TrimStart('/'));
                    if(System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                }
                 _appDbContext.Articles.Remove(artical);
                 _appDbContext.SaveChanges();

                 TempData["Success"] = "Article deleted successfully";
                 return RedirectToAction("ShowArticles","Seller");
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("Artical Deletion Error", "An error occurred while deleting the article, Please Try Again" + ex.Message);
                return RedirectToAction("ShowArticles","Seller");
            }
        }
        
    }
}