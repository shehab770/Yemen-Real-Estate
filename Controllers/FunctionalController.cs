using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
using webProgramming.Models;
using System.Text.Json;
using Microsoft.AspNetCore.Identity;

namespace webProgramming.Controllers
{
    public class FunctionalController : BaseController
    {
        public FunctionalController(AppDbContext context) : base(context)
        {
        }

        public static string GenerateUserId()
        {
            string firstPart = "USR";
            string middlePart = DateTime.UtcNow.ToString("yyyyMMdd"); 
            string lastPart = Guid.NewGuid().ToString("N").Substring(0, 6).ToUpper();

            return $"{firstPart}-{middlePart}-{lastPart}";
        }


        
        protected async Task<User> CreateSeller(Seller model)
        {
          try{
             var passwordHasher = new PasswordHasher<User>();
           var user = new User()
           {
            UserId = GenerateUserId(),
            Name = model.Name,
            Email = model.Email,
            Phone = model.Phone,
            Role = UserRole.Seller,
            IsBlocked = true,
            IsVerified = model.IsVerified,
            PasswordHash = string.Empty
           };
           user.PasswordHash = passwordHasher.HashPassword(user,model.Password);

            //Set Sellers Path Photos
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

           // Uplaoding Images and Docs
           if(model.Image != null && model.Image.Length>0)
           {
            var imageFileName = Guid.NewGuid().ToString() + Path.GetExtension(model.Image.FileName);
            var filePath = Path.Combine(sellerImageFolder, imageFileName);
            using(var stream = new FileStream(filePath, FileMode.Create))
            {
                await model.Image.CopyToAsync(stream);
            }
            user.Image = "/uploads/Sellers/" + imageFileName;
           }


           // Upload Business Documents
           if(model.BusinessDocuments != null && model.BusinessDocuments.Length>0 && model.OfficePhoto != null && model.OfficePhoto.Length>0)
           {
                var uniqueDocName = Guid.NewGuid().ToString() + Path.GetExtension(model.BusinessDocuments.FileName);
                var docPath = Path.Combine(sellerDocsFolder, uniqueDocName);

                using (var stream = new FileStream(docPath, FileMode.Create))
                {
                    await model.BusinessDocuments.CopyToAsync(stream);
                }

                user.BusinessDocuments = "/uploads/SellerDocs/" + uniqueDocName;

                var officePhotoFileName = Guid.NewGuid().ToString() + Path.GetExtension(model.OfficePhoto.FileName);
                var officePhotoPath = Path.Combine(sellerDocsFolder, officePhotoFileName);
                using(var stream = new FileStream(officePhotoPath, FileMode.Create))
                {
                    await model.OfficePhoto.CopyToAsync(stream);
                }
                 user.OfficePhoto = "/uploads/SellerDocs/" + officePhotoFileName;
           }else{
            ModelState.AddModelError("BusinessDocuments", "Please upload business documents.");
            ModelState.AddModelError("OfficePhoto", "Please upload office photo.");
           }

           return user;

          }catch(Exception ex)
          {
            ModelState.AddModelError("Registration", "Registration failed: " + ex.Message);
            return null;
          }
        }

        protected async Task<User> CreateBuyer(Buyer model)
        {
          try{
             var passwordHasher = new PasswordHasher<User>();
           var user = new User()
           {
            UserId = GenerateUserId(),
            Name = model.Name,
            Email = model.Email,
            Phone = model.Phone,
            Role = UserRole.Buyer,
            PasswordHash = string.Empty
           };
           user.PasswordHash = passwordHasher.HashPassword(user,model.Password);

            //Set Buyer Path Photos
           var buyerImageFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "Buyer");
           if(!Directory.Exists(buyerImageFolder))
           {
              Directory.CreateDirectory(buyerImageFolder);
           }

           // Uplaoding Images
           if(model.Image != null && model.Image.Length>0)
           {
            var imageFileName = Guid.NewGuid().ToString() + Path.GetExtension(model.Image.FileName);
            var filePath = Path.Combine(buyerImageFolder, imageFileName);
            using(var stream = new FileStream(filePath, FileMode.Create))
            {
                await model.Image.CopyToAsync(stream);
            }
            user.Image = "/uploads/Buyer/" + imageFileName;
           }else{
            user.Image = null;
           }

           return user;

          }catch(Exception ex)
          {
           ModelState.AddModelError("Registration", "Registration failed: " + ex.Message);
           return null;
          }
        }
        

         protected async Task<Product> CreateProductFromForm(IFormCollection form)
        {
            var sellerId = form["SellerId"];
            if(string.IsNullOrEmpty(sellerId))
            {
                throw new ArgumentException("Seller ID is required");
            }

            ValidateRequiredFields(form);
            var (countryId, cityId, neighbId) = ParseIds(form);
            var (pType, hType, cType) = ParsePropertyTypes(form);
            var properitiesJson = CreatePropertiesJson(form);
            var product = CreateBaseProduct(form, sellerId, countryId, cityId, neighbId, pType, hType, cType, properitiesJson);
            
            await HandleImageUploads(product, form);
            _appDbContext.Products.Add(product);
            return product;
        }

        protected void ValidateRequiredFields(IFormCollection form)
        {
            if (string.IsNullOrEmpty(form["CountryId"]) || 
                string.IsNullOrEmpty(form["CityId"]) || 
                string.IsNullOrEmpty(form["NeighbId"]))
            {
                throw new ArgumentException("Required fields are missing");
            }
        }

        protected (int countryId, int cityId, int neighbId) ParseIds(IFormCollection form)
        {
            if (!int.TryParse(form["CountryId"], out int countryId) ||
                !int.TryParse(form["CityId"], out int cityId) ||
                !int.TryParse(form["NeighbId"], out int neighbId))
            {
                throw new ArgumentException("Invalid ID format");
            }
            return (countryId, cityId, neighbId);
        }

        protected (PropertyType pType, HouseType hType, PropertyCategoryType cType) ParsePropertyTypes(IFormCollection form)
        {
            if (!int.TryParse(form["prop_type"], out int propTypeValue) ||
                !int.TryParse(form["estate_type"], out int estateTypeValue))
            {
                throw new ArgumentException("Invalid property type format");
            }

            int rentTypeValue = 0; // Default value for Sell properties
            if (propTypeValue == 2) // If property type is Rent
            {
                if (!int.TryParse(form["rent_type"], out rentTypeValue))
                {
                    throw new ArgumentException("Invalid rent type format");
                }
            }

            return ((PropertyType)propTypeValue, (HouseType)estateTypeValue, (PropertyCategoryType)rentTypeValue);
        }

        protected string CreatePropertiesJson(IFormCollection form)
        {
            var properitiesData = new
            {
                prop_size = form["prop_size"].ToString() ?? "0",
                rooms_num = form["rooms_num"].ToString() ?? "0",
                rooms_size = form["rooms_size"].ToString() ?? "0",
                bedrooms_num = form["bedrooms_num"].ToString() ?? "0",
                bedrooms_size = form["bedrooms_size"].ToString() ?? "0",
                bathrooms_num = form["bathrooms_num"].ToString() ?? "0",
                bathrooms_size = form["bathrooms_size"].ToString() ?? "0",
                kitchen_num = form["kitchen_num"].ToString() ?? "0",
                kitchen_size = form["kitchen_size"].ToString() ?? "0",
                //baths
                floating_baths = form.ContainsKey("floating_baths") ? "1" : "0",
                massage_baths = form.ContainsKey("massage_baths") ? "1" : "0",
                floor_standing_bath = form.ContainsKey("floor_standing_bath") ? "1" : "0",
                built_in_bath = form.ContainsKey("built_in_bath") ? "1" : "0",
                //Beds
                twin_bed = form.ContainsKey("twin_bed") ? "1" : "0",
                bunk_bed = form.ContainsKey("double_bed") ? "1" : "0",
                kids_bed = form.ContainsKey("kids_bed") ? "1" : "0",
                single_bed = form.ContainsKey("single_bed") ? "1" : "0",
                //Kitchen
                modern_kitchen = form.ContainsKey("modern_kitchen") ? "1" : "0",
                open_kitchen = form.ContainsKey("open_kitchen") ? "1" : "0",
                closed_kitchen = form.ContainsKey("closed_kitchen") ? "1" : "0",
                semi_open_kitchen = form.ContainsKey("semi_open_kitchen") ? "1" : "0",
                //Amenities
                air_conditioning_feature = form.ContainsKey("air_conditioning_feature") ? "1" : "0",
                swimming_pool_feature = form.ContainsKey("swimming_pool_feature") ? "1" : "0",
                microwave_feature = form.ContainsKey("microwave_feature") ? "1" : "0",
                balcony_feature = form.ContainsKey("balcony_feature") ? "1" : "0",
                gym_feature = form.ContainsKey("gym_feature") ? "1" : "0",
                shower_feature = form.ContainsKey("shower_feature") ? "1" : "0",
                washer_feature = form.ContainsKey("washer_feature") ? "1" : "0",
                window_coverings_feature = form.ContainsKey("window_coverings_feature") ? "1" : "0",
                garage_feature = form.ContainsKey("garage_feature") ? "1" : "0",
                wifi_feature = form.ContainsKey("wifi_feature") ? "1" : "0",
                refrigerator_feature = form.ContainsKey("refrigerator_feature") ? "1" : "0",
                tv_cable_feature = form.ContainsKey("tv_cable_feature") ? "1" : "0",
                laundry_feature = form.ContainsKey("laundry_feature") ? "1" : "0",
                garden_feature = form.ContainsKey("garden_feature") ? "1" : "0",
            };

            return JsonConvert.SerializeObject(properitiesData);
        }

        protected Product CreateBaseProduct(IFormCollection form, string sellerId, int countryId, int cityId, int neighbId, 
            PropertyType pType, HouseType hType, PropertyCategoryType cType, string properitiesJson)
        {
            return new Product()
                {
                    ProductId = GenerateProductId(),
                    Title = form["prop_tilte"].ToString() ?? string.Empty,
                    Price = decimal.Parse(form["prop_price"].ToString() ?? "0"),
                    YearBuilt = form["prop_date"].ToString() ?? string.Empty,
                    SellerId = sellerId,
                    CountryId = countryId,
                    CityId = cityId,
                    NeighbsId = neighbId,
                    Address = form["prop_address"].ToString() ?? string.Empty,
                    Type = pType,
                    EstateType = hType,
                    CategoryType = cType,
                    Description = form["prop_desc"].ToString() ?? string.Empty,
                    Properties = properitiesJson,
                    CreatedAt = DateTime.UtcNow
                };
        }

        protected async Task HandleImageUploads(Product product, IFormCollection form)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "products");
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

            // Handle main image
            var mainImage = form.Files["mainImage"];
            if (mainImage != null && mainImage.Length > 0)
            {
                    var mainImageFileName = Guid.NewGuid().ToString() + Path.GetExtension(mainImage.FileName);
                    var mainImagePath = Path.Combine(uploadsFolder, mainImageFileName);

                    using (var fileStream = new FileStream(mainImagePath, FileMode.Create))
                    {
                        await mainImage.CopyToAsync(fileStream);
                    }

                    product.MainImage = "/uploads/products/" + mainImageFileName;
                }

            // Handle listing images
            var arrayImagesJson = form["arrayImagesJson"].ToString();
            if (!string.IsNullOrEmpty(arrayImagesJson))
            {
                try
                {
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
                    throw new Exception("Error processing listing images: " + ex.Message);
                }
            }
            else
            {
                // If no new images provided, keep existing images
                product.ListingImages = product.ListingImages;
            }
        }

        protected class ImageData
        {
            public string Name { get; set; }
            public string Type { get; set; }
            public long Size { get; set; }
            public string Data { get; set; }
        }

        protected string GenerateProductId()
        {
            // Get current year and month
            string yearMonth = DateTime.Now.ToString("yyyy-MM");

            // Generate random 10-character string (letters + numbers)
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            string randomPart = new string(Enumerable.Repeat(chars, 10)
                .Select(s => s[random.Next(s.Length)]).ToArray());

            return $"{yearMonth}-{randomPart}";
        }
        
      protected List<Product> FilterProducts(IQueryable<Product> query, IFormCollection form)
{
    var products = query.ToList();

    var minPrice = form["minPrice"].ToString();
    var maxPrice = form["maxPrice"].ToString();
    var minSize  = form["minSize"].ToString();
    var maxSize  = form["maxSize"].ToString();
    var bedrooms = form["bedrooms"].ToString();
    var bathrooms = form["bathrooms"].ToString();
    var bathType  = form["bathType"].ToString();
    var bedType   = form["bedType"].ToString();
    var selectedAmenities = form["amenities"].ToList();

    // السعر
    if (!string.IsNullOrWhiteSpace(minPrice) && minPrice != "0" && decimal.TryParse(minPrice, out var minP))
        products = products.Where(p => p.Price >= minP).ToList();

    if (!string.IsNullOrWhiteSpace(maxPrice) && maxPrice != "0" && decimal.TryParse(maxPrice, out var maxP))
        products = products.Where(p => p.Price <= maxP).ToList();

    // المساحة
    if (!string.IsNullOrWhiteSpace(minSize) && minSize != "0" && int.TryParse(minSize, out var minS))
    {
        products = products.Where(p =>
        {
            var props = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(p.Properties ?? "{}");
            return props != null && props.TryGetValue("prop_size", out var s) && int.TryParse(s, out var sizeVal) && sizeVal >= minS;
        }).ToList();
    }

    if (!string.IsNullOrWhiteSpace(maxSize) && maxSize != "0" && int.TryParse(maxSize, out var maxS))
    {
        products = products.Where(p =>
        {
            var props = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(p.Properties ?? "{}");
            return props != null && props.TryGetValue("prop_size", out var s) && int.TryParse(s, out var sizeVal) && sizeVal <= maxS;
        }).ToList();
    }

    // غرف النوم
    if (!string.IsNullOrWhiteSpace(bedrooms) && bedrooms != "0" && int.TryParse(bedrooms, out var beds))
    {
        products = products.Where(p =>
        {
            var props = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(p.Properties ?? "{}");
            return props != null && props.TryGetValue("bedrooms_num", out var v) && int.TryParse(v, out var val) && val >= beds;
        }).ToList();
    }

    // الحمامات
    if (!string.IsNullOrWhiteSpace(bathrooms) && bathrooms != "0" && int.TryParse(bathrooms, out var baths))
    {
        products = products.Where(p =>
        {
            var props = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(p.Properties ?? "{}");
            return props != null && props.TryGetValue("bathrooms_num", out var v) && int.TryParse(v, out var val) && val >= baths;
        }).ToList();
    }

    // نوع الحمام
    if (!string.IsNullOrWhiteSpace(bathType) && bathType != "0")
    {
        products = products.Where(p =>
        {
            var props = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(p.Properties ?? "{}");
            return props != null && props.TryGetValue(bathType, out var v) && v == "1";
        }).ToList();
    }

    // نوع السرير
    if (!string.IsNullOrWhiteSpace(bedType) && bedType != "0")
    {
        products = products.Where(p =>
        {
            var props = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(p.Properties ?? "{}");
            return props != null && props.TryGetValue(bedType, out var v) && v == "1";
        }).ToList();
    }

    // المرافق
    if (selectedAmenities.Count > 0)
    {
        products = products.Where(p =>
        {
            var props = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(p.Properties ?? "{}");
            if (props == null) return false;

            foreach (var key in selectedAmenities)
            {
                if (string.IsNullOrWhiteSpace(key)) continue;
                if (!props.TryGetValue(key, out var value) || value != "1")
                    return false;
            }
            return true;
        }).ToList();
    }

    // ✅ ضبط MainImage فقط بدون تغيير الصور تحت
    foreach (var product in products)
    {
        var images = JsonConvert.DeserializeObject<List<string>>(product.ListingImages ?? "[]") ?? new List<string>();

        if (images.Count > 0)
        {
            // نعيّن MainImage من أول صورة إذا فاضي
            if (string.IsNullOrEmpty(product.MainImage))
                product.MainImage = images[0];
        }
        else
        {
            // fallback صورة افتراضية
            product.MainImage = "/images/no-image.png";
        }

        // نخلي ListingImages كما هي بدون تعديل
        product.ListingImages = JsonConvert.SerializeObject(images);
    }

    return products;
}
    }
}