namespace webProgramming.Models
{
    public class Amenity
    {
        public string? Name { get; set; }
        public string? FeatureName { get; set; }
        public string? DisplayName { get; set; }
        public string? IconClass { get; set; }
    }

    public class Baths
    {
        public string? Name { get; set; }
        public string? FeatureName { get; set; }
    }

    public class Kitchen
    {
        public string? Name { get; set; }
        public string? FeatureName { get; set; }
    }

    public class Beds
    {
        public string? Name { get; set; }
        public string? FeatureName { get; set; }
    }

    public class ViewModels
    {
        public List<Product>? AllProducts { get; set; }        // ✅ كل العقارات بدون فلترة
        public Product? Product { get; set; }
        public List<Product>? Products { get; set; }
        public List<Product>? FilteredProducts { get; set; }
        public User? User { get; set; }
        public User? Seller { get; set; }

        public User? Buyer { get; set; }

        public List<User>? Sellers { get; set; }
        public List<User>? Buyers { get; set; }
        public List<User>? Users { get; set; }

        public Settings? Settings { get; set; }
        public AboutUs? AboutUs { get; set; }

        public List <Contact>? Contacts { get; set; }


        public List<Country>? Countries { get; set; }
        public City? City { get; set; }
        public Neighb? Neighb { get; set; }
        public List<City>? Cities { get; set; }
        public List<Neighb>? Neighbs { get; set; }
        public List<Favor>? Favors { get; set; }

        public Article? Article { get; set; }
        public List<Article>? Articles { get; set; }

        public List<Chat>? Chats { get; set; }
        public List<Message>? Messages { get; set; }
        public  IFormFile? mainImage { get; set; }
        public  List<IFormFile>? arrayImages { get; set; }

       public List<Amenity> Amenities { get; set; } = new List<Amenity>
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
    new Amenity { Name = "غسيل ملابس", FeatureName = "laundry_feature", DisplayName = "غسيل ملابس", IconClass= "fas fa-tshirt" }, // تم تصحيح الأيقونة
    new Amenity { Name = "حديقة", FeatureName = "garden_feature", DisplayName = "حديقة", IconClass= "fas fa-leaf" },
    new Amenity { Name = "كراج / موقف سيارات", FeatureName = "garage_feature", DisplayName = "كراج / موقف سيارات", IconClass= "fas fa-car" },
};
      public List<Baths> Baths { get; set; } = new List<Baths>
{
    new Baths { Name = "بانيو عادي", FeatureName = "floating_baths" },
    new Baths { Name = "بانيو مساج", FeatureName = "massage_baths" },
    new Baths { Name = "بانيو واقف (قائم بذاته)", FeatureName = "floor_standing_bath" },
    new Baths { Name = "بانيو مدمج في الأرض أو الجدار", FeatureName = "built_in_bath" },
};

        public List<Beds> Beds {get; set;} = new List<Beds>
        {
            new Beds { Name = "سرير مفرد", FeatureName = "single_bed" },
            new Beds { Name = "سرير مزدوج", FeatureName = "double_bed" },
            new Beds { Name = "سريرين منفصلين", FeatureName = "twin_bed" },
            new Beds { Name = "سريرأطفال", FeatureName = "kids_bed" },
        };

        public List<Kitchen> Kitchen {get; set;} = new List<Kitchen>
        {
            new Kitchen { Name = "مطبخ حديث", FeatureName = "modern_kitchen" },
            new Kitchen { Name = "مطبخ مفتوح", FeatureName = "open_kitchen" },
            new Kitchen { Name = "مطبخ مغلق", FeatureName = "closed_kitchen" },
            new Kitchen { Name = "مطبخ نصف مفتوح", FeatureName = "semi_open_kitchen" },
        };



    }
}


