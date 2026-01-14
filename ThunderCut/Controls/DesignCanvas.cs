using System;
using System.Windows;
using System.Windows.Input;
using SkiaSharp;
using SkiaSharp.Views.WPF;
using SkiaSharp.Views.Desktop;

namespace ThunderCut
{
    public enum DesignTool
    {
        Select,
        Draw,
        Text,
        Import
    }

    public class DesignCanvas : SKElement
    {
        private float _zoom = 1.0f;
        private float _offsetX = 0;
        private float _offsetY = 0;
        private Point _lastMousePos;
        private bool _isFirstPaint = true;
        private bool _isPanning = false;
        private bool _isDragging = false;
        private CutPath? _selectedPath;

        public event EventHandler<float>? ZoomChanged;

        public DesignCanvas()
        {
            this.MouseDown += OnMouseDown;
            this.MouseMove += OnMouseMove;
            this.MouseUp += OnMouseUp;
            this.MouseWheel += OnMouseWheel;
        }

        private DesignTool _currentTool = DesignTool.Select;
        public DesignTool CurrentTool
        {
            get => _currentTool;
            set
            {
                _currentTool = value;
                _selectedPath = null;
                InvalidateVisual();
            }
        }

        private PhoneModel? _selectedModel;
        public PhoneModel? SelectedModel
        {
            get => _selectedModel;
            set
            {
                _selectedModel = value;
                _selectedPath = null;
                InvalidateVisual();
            }
        }

        private SKColor _pouchColor = SKColor.Parse("#252525");
        public SKColor PouchColor
        {
            get => _pouchColor;
            set
            {
                _pouchColor = value;
                InvalidateVisual();
            }
        }

        private TemplateType _selectedTemplateType = TemplateType.Back;
        public TemplateType SelectedTemplateType
        {
            get => _selectedTemplateType;
            set
            {
                _selectedTemplateType = value;
                _selectedPath = null;
                InvalidateVisual();
            }
        }

        public void CenterTemplate(int viewWidth, int viewHeight)
        {
            float templateWidth = _selectedModel?.WidthMm ?? 77.0f;
            float templateHeight = _selectedModel?.HeightMm ?? 160.0f;

            _zoom = 3.5f;
            _offsetX = (viewWidth / 2.0f) - (templateWidth / 2.0f * _zoom);
            _offsetY = (viewHeight / 2.0f) - (templateHeight / 2.0f * _zoom);
            
            ZoomChanged?.Invoke(this, _zoom);
            InvalidateVisual();
        }

        protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
        {
            base.OnPaintSurface(e);

            if (_isFirstPaint)
            {
                CenterTemplate(e.Info.Width, e.Info.Height);
                _isFirstPaint = false;
            }

            var canvas = e.Surface.Canvas;
            canvas.Clear(SKColor.Parse("#1A1A1A"));

            // Apply transforms (Pan & Zoom)
            canvas.Translate(_offsetX, _offsetY);
            canvas.Scale(_zoom);

            DrawGrid(canvas, e.Info.Width, e.Info.Height);
            
            if (_selectedModel != null)
                DrawWorkspace(canvas);
        }

        private void DrawGrid(SKCanvas canvas, int width, int height)
        {
            float gridSize = 10.0f; // 10mm grid
            float majorGridSize = 50.0f; // 50mm major grid

            using var paint = new SKPaint
            {
                Color = SKColors.White.WithAlpha(15),
                StrokeWidth = 0.5f / _zoom,
                Style = SKPaintStyle.Stroke
            };

            using var majorPaint = new SKPaint
            {
                Color = SKColors.White.WithAlpha(30),
                StrokeWidth = 1.0f / _zoom,
                Style = SKPaintStyle.Stroke
            };

            // Calculate visible bounds in world coordinates
            float startX = -_offsetX / _zoom;
            float startY = -_offsetY / _zoom;
            float endX = startX + (width / _zoom);
            float endY = startY + (height / _zoom);

            float currentX = (float)Math.Floor(startX / gridSize) * gridSize;
            while (currentX < endX)
            {
                var p = (currentX % majorGridSize == 0) ? majorPaint : paint;
                canvas.DrawLine(currentX, startY, currentX, endY, p);
                currentX += gridSize;
            }

            float currentY = (float)Math.Floor(startY / gridSize) * gridSize;
            while (currentY < endY)
            {
                var p = (currentY % majorGridSize == 0) ? majorPaint : paint;
                canvas.DrawLine(startX, currentY, endX, currentY, p);
                currentY += gridSize;
            }
        }

        private void DrawWorkspace(SKCanvas canvas)
        {
            if (_selectedModel == null) return;

            float width = _selectedModel.WidthMm;
            float height = _selectedModel.HeightMm;
            float radius = _selectedModel.CornerRadius;

            // Draw shadow
            using var shadowPaint = new SKPaint
            {
                Color = SKColors.Black.WithAlpha(100),
                MaskFilter = SKMaskFilter.CreateBlur(SKBlurStyle.Normal, 5.0f)
            };
            canvas.DrawRoundRect(new SKRect(2, 2, width + 2, height + 2), radius, radius, shadowPaint);

            // Main Background
            using var fillPaint = new SKPaint { Color = _pouchColor, Style = SKPaintStyle.Fill };
            canvas.DrawRoundRect(new SKRect(0, 0, width, height), radius, radius, fillPaint);

            // Border
            using var borderPaint = new SKPaint
            {
                Color = _selectedTemplateType == TemplateType.Front ? SKColors.LimeGreen : SKColors.DeepSkyBlue,
                StrokeWidth = 1.5f / _zoom,
                Style = SKPaintStyle.Stroke,
                IsAntialias = true
            };
            canvas.DrawRoundRect(new SKRect(0, 0, width, height), radius, radius, borderPaint);

            // Find current template
            var template = _selectedModel.Templates.Find(t => t.Type == _selectedTemplateType);
            if (template != null)
            {
                // Cut Paths
                using var cutPaint = new SKPaint
                {
                    Color = SKColors.Red.WithAlpha(180),
                    StrokeWidth = 0.8f / _zoom,
                    Style = SKPaintStyle.Stroke,
                    IsAntialias = true
                };

                using var selectPaint = new SKPaint
                {
                    Color = SKColors.Cyan.WithAlpha(200),
                    StrokeWidth = 1.0f / _zoom,
                    Style = SKPaintStyle.Stroke,
                    PathEffect = SKPathEffect.CreateDash(new float[] { 4 / _zoom, 4 / _zoom }, 0)
                };

                foreach (var cp in template.CutPaths)
                {
                    var p = (cp == _selectedPath) ? selectPaint : cutPaint;
                    
                    if (cp.Type == "Circle")
                        canvas.DrawCircle(cp.X, cp.Y, cp.Radius, p);
                    else if (cp.Type == "Text")
                    {
                        using var textPaintObj = new SKPaint
                        {
                            Color = p.Color,
                            IsAntialias = true,
                            Style = SKPaintStyle.Stroke
                        };
                        using var textFont = new SKFont(SKTypeface.FromFamilyName("Arial", SKFontStyle.Bold), 5);
                        canvas.DrawText(cp.Text, cp.X, cp.Y, textFont, textPaintObj);
                    }
                    else
                        canvas.DrawRoundRect(new SKRect(cp.X, cp.Y, cp.X + cp.Width, cp.Y + cp.Height), cp.Radius, cp.Radius, p);
                    
                    if (cp == _selectedPath)
                    {
                        // Draw handles if needed
                    }
                }
            }

            // Label
            using var textPaint = new SKPaint
            {
                Color = SKColors.White.WithAlpha(180),
                IsAntialias = true
            };
            using var font = new SKFont(SKTypeface.FromFamilyName("Arial", SKFontStyle.Bold), 8 / _zoom);
            string label = $"{_selectedModel.Name} - {_selectedTemplateType} [{_currentTool}]";
            canvas.DrawText(label, 0, -5, font, textPaint);
        }

        public void AddDefaultCutout()
        {
            if (_selectedModel == null) return;
            
            var template = _selectedModel.Templates.Find(t => t.Type == _selectedTemplateType);
            if (template == null) return;

            var newCut = new CutPath 
            { 
                Label = "New Cutout", 
                Type = "Circle", 
                X = _selectedModel.WidthMm / 2, 
                Y = _selectedModel.HeightMm / 2, 
                Radius = 5 
            };
            template.CutPaths.Add(newCut);
            _selectedPath = newCut;
            _currentTool = DesignTool.Select;
            
            InvalidateVisual();
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            _lastMousePos = e.GetPosition(this);
            var worldPos = ScreenToWorld(_lastMousePos);

            if (e.MiddleButton == MouseButtonState.Pressed || (e.LeftButton == MouseButtonState.Pressed && Keyboard.IsKeyDown(Key.Space)))
            {
                _isPanning = true;
                this.CaptureMouse();
                this.Cursor = Cursors.SizeAll;
                return;
            }

            if (_currentTool == DesignTool.Select && e.LeftButton == MouseButtonState.Pressed)
            {
                _selectedPath = HitTest(worldPos);
                if (_selectedPath != null)
                {
                    _isDragging = true;
                    this.CaptureMouse();
                }
                InvalidateVisual();
            }
            else if (_currentTool == DesignTool.Text && e.LeftButton == MouseButtonState.Pressed)
            {
                var template = _selectedModel?.Templates.Find(t => t.Type == _selectedTemplateType);
                if (template != null)
                {
                    var textCut = new CutPath
                    {
                        Type = "Text",
                        Text = "THUNDER",
                        X = worldPos.X,
                        Y = worldPos.Y,
                        Label = "Text Cut"
                    };
                    template.CutPaths.Add(textCut);
                    _selectedPath = textCut;
                    _currentTool = DesignTool.Select;
                    InvalidateVisual();
                }
            }
            else if (_currentTool == DesignTool.Draw && e.LeftButton == MouseButtonState.Pressed)
            {
                var template = _selectedModel?.Templates.Find(t => t.Type == _selectedTemplateType);
                if (template != null)
                {
                    var rectCut = new CutPath
                    {
                        Type = "Rect",
                        X = worldPos.X - 10,
                        Y = worldPos.Y - 10,
                        Width = 20,
                        Height = 20,
                        Radius = 2,
                        Label = "Rect Cut"
                    };
                    template.CutPaths.Add(rectCut);
                    _selectedPath = rectCut;
                    _currentTool = DesignTool.Select;
                    InvalidateVisual();
                }
            }
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            var currentPos = e.GetPosition(this);
            var worldPos = ScreenToWorld(currentPos);
            var lastWorldPos = ScreenToWorld(_lastMousePos);
            float dx = worldPos.X - lastWorldPos.X;
            float dy = worldPos.Y - lastWorldPos.Y;

            if (_isPanning)
            {
                _offsetX += (float)(currentPos.X - _lastMousePos.X);
                _offsetY += (float)(currentPos.Y - _lastMousePos.Y);
                _lastMousePos = currentPos;
                InvalidateVisual();
            }
            else if (_isDragging && _selectedPath != null)
            {
                _selectedPath.X += dx;
                _selectedPath.Y += dy;
                _lastMousePos = currentPos;
                InvalidateVisual();
            }
        }

        private void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (_isPanning || _isDragging)
            {
                _isPanning = false;
                _isDragging = false;
                this.ReleaseMouseCapture();
                this.Cursor = Cursors.Arrow;
            }
        }

        private void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var mousePos = e.GetPosition(this);
            float oldZoom = _zoom;

            if (e.Delta > 0)
                _zoom *= 1.1f;
            else
                _zoom /= 1.1f;

            _zoom = Math.Max(0.1f, Math.Min(_zoom, 50f));

            float focalX = (float)mousePos.X;
            float focalY = (float)mousePos.Y;

            _offsetX = focalX - (focalX - _offsetX) * (_zoom / oldZoom);
            _offsetY = focalY - (focalY - _offsetY) * (_zoom / oldZoom);

            ZoomChanged?.Invoke(this, _zoom);
            InvalidateVisual();
        }

        private SKPoint ScreenToWorld(Point screen)
        {
            return new SKPoint(
                ((float)screen.X - _offsetX) / _zoom,
                ((float)screen.Y - _offsetY) / _zoom
            );
        }

        private CutPath? HitTest(SKPoint world)
        {
            if (_selectedModel == null) return null;
            var template = _selectedModel.Templates.Find(t => t.Type == _selectedTemplateType);
            if (template == null) return null;

            // Iterate backwards to pick top-most
            for (int i = template.CutPaths.Count - 1; i >= 0; i--)
            {
                var cp = template.CutPaths[i];
                if (cp.Type == "Circle")
                {
                    float distSq = (world.X - cp.X) * (world.X - cp.X) + (world.Y - cp.Y) * (world.Y - cp.Y);
                    if (distSq <= cp.Radius * cp.Radius) return cp;
                }
                else
                {
                    if (world.X >= cp.X && world.X <= cp.X + cp.Width &&
                        world.Y >= cp.Y && world.Y <= cp.Y + cp.Height) return cp;
                }
            }
            return null;
        }
    }
}
