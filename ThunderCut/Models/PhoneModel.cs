using System.Collections.Generic;
using SkiaSharp;

namespace ThunderCut
{
    public enum TemplateType
    {
        Back,
        Front,
        FullBody,
        CameraLens
    }

    public class PhoneModel
    {
        public string Name { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public string Tier { get; set; } = "Tier-1";
        public float WidthMm { get; set; }
        public float HeightMm { get; set; }
        public float CornerRadius { get; set; }
        
        public List<DesignTemplate> Templates { get; set; } = new List<DesignTemplate>();

        // For backward compatibility or simple access
        public List<CutPath> CutPaths => Templates.Find(t => t.Type == TemplateType.Back)?.CutPaths ?? new List<CutPath>();
    }

    public class DesignTemplate
    {
        public TemplateType Type { get; set; } = TemplateType.Back;
        public string Name { get; set; } = "Back Design";
        public List<CutPath> CutPaths { get; set; } = new List<CutPath>();
    }

    public class CutPath
    {
        public string Type { get; set; } = "Circle"; // Circle, Rect, Path, Text
        public string Label { get; set; } = "Cut";
        public string Text { get; set; } = string.Empty;
        public float X { get; set; }
        public float Y { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public float Radius { get; set; }
        public bool IsHollow { get; set; } = true;
    }

    public class BrandGroup
    {
        public string BrandName { get; set; } = string.Empty;
        public List<PhoneModel> Models { get; set; } = new List<PhoneModel>();
    }

    public class TierGroup
    {
        public string TierName { get; set; } = string.Empty;
        public List<BrandGroup> Brands { get; set; } = new List<BrandGroup>();
    }
}

