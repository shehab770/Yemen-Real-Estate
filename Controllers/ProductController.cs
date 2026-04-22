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
    public class ProductController : FunctionalController
    {
        public ProductController(AppDbContext Context) : base(Context)
        {
        }


        public IActionResult Add()
        {

            var user = GetCurrentloggedUser();
            if(user == null){return RedirectToAction("Login", "Auth");}
            var result = RedirectToLogIfNotLoggedinOrAuthorized(UserRole.Seller);
            if (result is not OkResult)
            {
                return RedirectToAction("SetTempData", "Auth");
            }
            
             ViewBag.PropertyCount = _appDbContext.Products.Count(p => p.SellerId == user.UserId);
            var neighbs = _appDbContext.Neighs.ToList();
            var viewModel = new ViewModels
            {
                User = user,
                Neighbs = neighbs,
                Cities = _appDbContext.Cities.ToList(),
                Countries = _appDbContext.Countries.ToList()
            };


            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Store(IFormCollection form)
        {
            var result = RedirectToLogIfNotLoggedinOrAuthorized(UserRole.Seller);
            if (result is not OkResult)
            {
                return RedirectToAction("SetTempData", "Auth");
            }

            try
            {
                var product = await CreateProductFromForm(form);
                await _appDbContext.SaveChangesAsync();
                TempData["True"] = "Property Created Successfully";
                return RedirectToAction("Add");
            }
            catch (Exception ex)
            {
                TempData["Wrong"] = "Property Creation Failed: " + ex.Message;
                return RedirectToAction("Add");
            }
        }

    
       public IActionResult PropertyDetails(string id)
       {
         var user = GetCurrentloggedUser();
         if(user == null){return RedirectToAction("Login", "Auth");}


        var product = _appDbContext.Products.FirstOrDefault(product=>product.ProductId == id);  
        if(product == null){return NotFound();}

            var seller = _appDbContext.Users.FirstOrDefault(seller=>seller.UserId == product.SellerId);
            var city = _appDbContext.Cities.FirstOrDefault(c => c.Id == product.CityId);
            var neighb = _appDbContext.Neighs.FirstOrDefault(c => c.Id == product.NeighbsId);

             ViewBag.PropertyCount = _appDbContext.Products.Count(p => p.SellerId == user.UserId);
            // Set up the view model with necessary data
            var viewModel = new ViewModels
            {
                User = user,
                Seller = seller,
                Product = product,
                City = city,
                Neighb = neighb
            };

            return View(viewModel);

       }

       public IActionResult Edit(string id){

             var user = GetCurrentloggedUser();
             if(user == null)
             {
                TempData["ErrorMessage"] = "Please Login First";
                return RedirectToAction("Login", "Auth");
             }

             var result = RedirectToLogIfNotLoggedinOrAuthorized(UserRole.Seller);
             if (result is not OkResult)
             {
                 return RedirectToAction("SetTempData", "Auth");
             }

            var product = _appDbContext.Products.FirstOrDefault(p=>p.ProductId == id);
            if(product == null){return NotFound();}

            var city = _appDbContext.Cities.FirstOrDefault(c=>c.Id == product.CityId);
            var neighb = _appDbContext.Neighs.FirstOrDefault(c=>c.Id == product.NeighbsId);

            var Cities = _appDbContext.Cities.ToList();
            var Neighbs = _appDbContext.Neighs.ToList();

            
            // Set up the view model with necessary data
            var viewModel = new ViewModels{
                User = user,
                Product = product,
                City = city,
                Neighb = neighb,
                Cities = Cities,
                Neighbs = Neighbs
            };
            ViewBag.Countries = _appDbContext.Countries.ToList();

            return View(viewModel);
       }

       public async Task<IActionResult> Delete(string id)
       {
         try{

            var user = GetCurrentloggedUser();
            if(user == null){return RedirectToAction("Login", "Auth");}

            var result = RedirectToLogIfNotLoggedinOrAuthorized(user.Role);
            if (result is not OkResult){ return result;}

            var product = _appDbContext.Products.FirstOrDefault(p =>
                user.Role == UserRole.Seller ? p.ProductId == id && p.SellerId == user.UserId 
                : p.ProductId == id
                );
            if(product == null)
                {
                    TempData["Wrong"] = "Property Deletion Failed: ";
                    return RedirectToAction("ShowProperties", "Seller");
                }

            // Delete main image if exists
            if (!string.IsNullOrEmpty(product.MainImage))
            {
                var mainImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", product.MainImage.TrimStart('/'));
                if (System.IO.File.Exists(mainImagePath))
                {
                    System.IO.File.Delete(mainImagePath);
                }
            }

            // Delete listing images if exist
            if (!string.IsNullOrEmpty(product.ListingImages))
            {
                var listingImages = JsonConvert.DeserializeObject<List<string>>(product.ListingImages);
                if (listingImages != null)
                {
                    foreach (var imagePath in listingImages)
                    {
                        var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", imagePath.TrimStart('/'));
                        if (System.IO.File.Exists(fullPath))
                        {
                            System.IO.File.Delete(fullPath);
                        }
                    }
                }
            }

            _appDbContext.Products.Remove(product);
            await _appDbContext.SaveChangesAsync();
            TempData["True"] = "Property Deleted Successfully";
            return RedirectToAction("ShowProperties", "Seller");
         }
         catch(Exception ex){
            TempData["Wrong"] = "Property Deletion Failed: " + ex.Message;
            return RedirectToAction("ShowProperties", "Seller");
         }
       }


       public async Task<IActionResult> Update(IFormCollection form)
       {
            try{
                
                var user = GetCurrentloggedUser();
                if(user == null){return RedirectToAction("Login", "Auth");}

                 var result = RedirectToLogIfNotLoggedinOrAuthorized(UserRole.Seller);
                 if (result is not OkResult)
                 {
                     return RedirectToAction("SetTempData", "Auth");
                 }

                var productId = form["ProductId"].ToString();
                var sellerId = form["SellerId"].ToString();

                var product = _appDbContext.Products.FirstOrDefault(p=>p.ProductId == productId);
                if(product == null){return NotFound();}

                ValidateRequiredFields(form);
                var (countryId, cityId, neighbId) = ParseIds(form);
                var (pType, hType, cType) = ParsePropertyTypes(form);
                var properitiesJson = CreatePropertiesJson(form);
           
                // Update basic product information
                product.SellerId = sellerId;
                product.Address = form["prop_address"].ToString() ?? string.Empty;
                product.CountryId = countryId;
                product.CityId = cityId;
                product.NeighbsId = neighbId;
                product.Type = pType;
                product.EstateType = hType;
                product.CategoryType = cType;
                product.Description = form["prop_desc"].ToString() ?? string.Empty;
                product.Properties = properitiesJson;
                product.UpdatedAt = DateTime.UtcNow;

                // Handle image updates
                var oldImage = product.MainImage;
                var mainImage = form.Files["mainImage"];
                if (mainImage != null && mainImage.Length > 0)
                {

                    // Upload new main image
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "products");
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    var mainImageFileName = Guid.NewGuid().ToString() + Path.GetExtension(mainImage.FileName);
                    var mainImagePath = Path.Combine(uploadsFolder, mainImageFileName);

                    using (var fileStream = new FileStream(mainImagePath, FileMode.Create))
                    {
                        await mainImage.CopyToAsync(fileStream);
                    }

                    product.MainImage = "/uploads/products/" + mainImageFileName;

                    if(product.MainImage != null) // check if the new image is uploaded or not
                    {

                        if (!string.IsNullOrEmpty(oldImage))
                        {
                            var oldMainImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", oldImage.TrimStart('/'));
                            if (System.IO.File.Exists(oldMainImagePath))
                            {
                                System.IO.File.Delete(oldMainImagePath);
                            }
                        }
                    }
                }

                // Handle listing images
                var arrayImagesJson = form["arrayImagesJson"].ToString();
                if (!string.IsNullOrEmpty(arrayImagesJson))
                {
                    try
                    {
                        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "products");
                        if (!Directory.Exists(uploadsFolder))
                            Directory.CreateDirectory(uploadsFolder);

                        var imageData = JsonConvert.DeserializeObject<List<ImageData>>(arrayImagesJson);
                        var listingImagePaths = new List<string>();
                        var existingImages = JsonConvert.DeserializeObject<List<string>>(product.ListingImages ?? "[]");
                        
                        if(imageData != null)
                           {
                             foreach (var img in imageData)
                            {
                                if (img.Data.StartsWith("data:"))
                                {
                                    // Handle new image
                                    var base64Data = img.Data.Split(',')[1];
                                    var imageBytes = Convert.FromBase64String(base64Data);
                                    var fileName = $"{Guid.NewGuid()}.jpg";
                                    var filePath = Path.Combine(uploadsFolder, fileName);
                                    await System.IO.File.WriteAllBytesAsync(filePath, imageBytes);
                                    listingImagePaths.Add($"/uploads/products/{fileName}");
                                }
                                else
                                {
                                    // Keep existing image
                                    listingImagePaths.Add(img.Data);
                                }
                            }
                           }

                        // Delete removed images from server
                        if (existingImages != null)
                        {
                            foreach (var existingImage in existingImages)
                            {
                                if (!listingImagePaths.Contains(existingImage))
                                {
                                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", existingImage.TrimStart('/'));
                                    if (System.IO.File.Exists(filePath))
                                    {
                                        System.IO.File.Delete(filePath);
                                    }
                                }
                            }
                        }

                        product.ListingImages = JsonConvert.SerializeObject(listingImagePaths);
                    }
                    catch (Exception ex)
                    {
                        return Json(new { success = false, message = $"Error processing listing images: {ex.Message}" });
                    }
                }
                else
                {
                    // If no new images provided, keep existing images
                    product.ListingImages = product.ListingImages;
                }

                await _appDbContext.SaveChangesAsync();
                TempData["True"] = "Property Updated Successfully";
                return RedirectToAction("ShowProperties", "Seller");
            }
            catch(Exception ex){
                TempData["Wrong"] = "Property Update Failed: " + ex.Message;
                return RedirectToAction("ShowProperties", "Seller");
            }
       }
    }
}

