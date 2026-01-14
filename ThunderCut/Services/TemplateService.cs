using System.Collections.Generic;
using System.Linq;

namespace ThunderCut.Services
{
    public class TemplateService
    {
        public List<TierGroup> GetTieredTemplates()
        {
            var tiers = new List<TierGroup>();

            // --- TIER 1 ---
            var tier1 = new TierGroup { TierName = "Tier-1 (Must-Have)" };
            
            // Apple
            var apple = new BrandGroup { BrandName = "Apple" };
            apple.Models.AddRange(new[] {
                CreateModel("iPhone 15 Pro Max", "Apple", 76.7f, 159.9f, 12, GetIPhoneBackPaths(), GetIPhoneFrontPaths()),
                CreateModel("iPhone 15 Pro", "Apple", 70.6f, 146.6f, 12, GetIPhoneBackPaths(), GetIPhoneFrontPaths()),
                CreateModel("iPhone 14 Pro Max", "Apple", 77.6f, 160.7f, 12, GetIPhoneBackPaths(), GetIPhoneFrontPaths()),
                CreateModel("iPhone 13", "Apple", 71.5f, 146.7f, 10, GetIPhoneBackPaths(), GetIPhoneFrontPaths()),
                CreateModel("iPhone 12", "Apple", 71.5f, 146.7f, 10, GetIPhoneBackPaths(), GetIPhoneFrontPaths()),
                CreateModel("iPhone 11", "Apple", 75.7f, 150.9f, 15, GetIPhoneBackPaths(), GetIPhoneFrontPaths())
            });
            tier1.Brands.Add(apple);

            // Samsung
            var samsung = new BrandGroup { BrandName = "Samsung" };
            samsung.Models.AddRange(new[] {
                CreateModel("Galaxy S24 Ultra", "Samsung", 79.0f, 162.3f, 2, GetSamsungBackPaths(), GetSamsungFrontPaths()),
                CreateModel("Galaxy S23", "Samsung", 70.9f, 146.3f, 12, GetSamsungBackPaths(), GetSamsungFrontPaths()),
                CreateModel("Galaxy A55", "Samsung", 77.4f, 161.1f, 10, GetGenericBackPaths(), GetGenericFrontPaths()),
                CreateModel("Galaxy M34", "Samsung", 77.2f, 161.7f, 10, GetGenericBackPaths(), GetGenericFrontPaths())
            });
            tier1.Brands.Add(samsung);

            tiers.Add(tier1);

            // --- TIER 2 ---
            var tier2 = new TierGroup { TierName = "Tier-2 (Important)" };
            
            var google = new BrandGroup { BrandName = "Google" };
            google.Models.AddRange(new[] {
                CreateModel("Pixel 8 Pro", "Google", 76.5f, 162.6f, 15, GetGenericBackPaths(), GetGenericFrontPaths()),
                CreateModel("Pixel 7a", "Google", 72.9f, 152.0f, 15, GetGenericBackPaths(), GetGenericFrontPaths())
            });
            tier2.Brands.Add(google);
            
            var motorola = new BrandGroup { BrandName = "Motorola" };
            motorola.Models.Add(CreateModel("Edge 40", "Motorola", 72.0f, 158.4f, 8, GetGenericBackPaths(), GetGenericFrontPaths()));
            tier2.Brands.Add(motorola);

            tiers.Add(tier2);

            return tiers;
        }

        private PhoneModel CreateModel(string name, string brand, float w, float h, float r, List<CutPath> back, List<CutPath> front)
        {
            var model = new PhoneModel { Name = name, Brand = brand, WidthMm = w, HeightMm = h, CornerRadius = r };
            model.Templates.Add(new DesignTemplate { Type = TemplateType.Back, Name = "Back Shell", CutPaths = back });
            model.Templates.Add(new DesignTemplate { Type = TemplateType.Front, Name = "Front Screen", CutPaths = front });
            return model;
        }

        private List<CutPath> GetIPhoneBackPaths() => new List<CutPath> { 
            new CutPath { Label="Camera Island", Type="Rect", X=5, Y=5, Width=35, Height=35, Radius=8, IsHollow=false },
            new CutPath { Label="Apple Logo", Type="Circle", X=38, Y=80, Radius=5, IsHollow=true }
        };

        private List<CutPath> GetIPhoneFrontPaths() => new List<CutPath> { 
            new CutPath { Label="Dynamic Island", Type="Rect", X=25, Y=8, Width=26, Height=6, Radius=3, IsHollow=false },
            new CutPath { Label="Earpiece", Type="Rect", X=32, Y=4, Width=12, Height=1, Radius=0.5f, IsHollow=false }
        };

        private List<CutPath> GetSamsungBackPaths() => new List<CutPath> { 
            new CutPath { Label="Camera 1", Type="Circle", X=10, Y=10, Radius=4 },
            new CutPath { Label="Camera 2", Type="Circle", X=10, Y=22, Radius=4 },
            new CutPath { Label="Camera 3", Type="Circle", X=10, Y=34, Radius=4 }
        };

        private List<CutPath> GetSamsungFrontPaths() => new List<CutPath> { 
            new CutPath { Label="Punch Hole", Type="Circle", X=39.5f, Y=8, Radius=1.5f }
        };

        private List<CutPath> GetGenericBackPaths() => new List<CutPath> { new CutPath { Type="Circle", X=35, Y=10, Radius=3 } };
        private List<CutPath> GetGenericFrontPaths() => new List<CutPath> { new CutPath { Type="Circle", X=35, Y=5, Radius=1.5f } };
    }
}

