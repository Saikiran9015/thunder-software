using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ThunderCut.Services;
using ThunderCut.Views;

namespace ThunderCut
{
    public partial class MainWindow : Window
    {
        private readonly TemplateService _templateService = new TemplateService();
        private CutPreviewView? _previewView;
        private PouchDesignView? _pouchView;

        public MainWindow()
        {
            InitializeComponent();
            
            // Populate the TreeView with Tiered Data
            TemplateTree.ItemsSource = _templateService.GetTieredTemplates();
            
            // Handle Model Selection
            TemplateTree.SelectedItemChanged += (s, e) => {
                if (e.NewValue is PhoneModel model)
                {
                    MainCanvas.SelectedModel = model;
                    MainCanvas.CenterTemplate((int)MainCanvas.ActualWidth, (int)MainCanvas.ActualHeight);
                }
            };

            MainCanvas.ZoomChanged += (s, zoom) => {
                ZoomStatus.Text = $"{zoom * 100:0}%";
            };
        }

        private void OnAddCutout(object sender, RoutedEventArgs e)
        {
            MainCanvas.AddDefaultCutout();
        }

        private void OnCutPreviewClick(object sender, RoutedEventArgs e)
        {
            if (MainCanvas.SelectedModel == null)
            {
                MessageBox.Show("Please select a mobile model first.", "ThunderCut", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (_previewView == null)
            {
                _previewView = new CutPreviewView();
                _previewView.BackToDesign += (s, args) => 
                {
                    DesignView.Visibility = Visibility.Visible;
                    ViewContainer.Visibility = Visibility.Collapsed;
                };

                _previewView.ProceedToPouchDesign += (s, args) =>
                {
                    if (_pouchView == null)
                    {
                        _pouchView = new PouchDesignView();
                        _pouchView.BackRequested += (bs, bargs) => 
                        {
                            // Back to cut preview
                             ViewContainer.Content = _previewView;
                        };
                        _pouchView.FinishRequested += (fs, fargs) =>
                        {
                            // Back to main design or reset
                            DesignView.Visibility = Visibility.Visible;
                            ViewContainer.Visibility = Visibility.Collapsed;
                        };
                    }
                    
                    _pouchView.SetModel(MainCanvas.SelectedModel!, MainCanvas.SelectedTemplateType);
                    ViewContainer.Content = _pouchView;
                };
            }

            _previewView.SetModel(MainCanvas.SelectedModel, MainCanvas.SelectedTemplateType);
            ViewContainer.Content = _previewView;
            
            DesignView.Visibility = Visibility.Collapsed;
            ViewContainer.Visibility = Visibility.Visible;
        }

        private void OnToolClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is string toolStr)
            {
                if (System.Enum.TryParse(toolStr, out DesignTool tool))
                {
                    MainCanvas.CurrentTool = tool;
                }
            }
        }

        private void OnTemplateTypeClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is string tag)
            {
                if (System.Enum.TryParse(tag, out TemplateType type))
                {
                    MainCanvas.SelectedTemplateType = type;
                    
                    // Update button styles
                    BackViewBtn.Background = Brushes.Transparent;
                    FrontViewBtn.Background = Brushes.Transparent;
                    btn.Background = new SolidColorBrush(Color.FromArgb(60, 0, 204, 255));
                }
            }
        }

        private void OnScrollUp(object sender, RoutedEventArgs e)
        {
            var scrollViewer = GetScrollViewer(TemplateTree);
            scrollViewer?.LineUp();
        }

        private void OnScrollDown(object sender, RoutedEventArgs e)
        {
            var scrollViewer = GetScrollViewer(TemplateTree);
            scrollViewer?.LineDown();
        }

        private ScrollViewer? GetScrollViewer(DependencyObject depObj)
        {
            if (depObj is ScrollViewer viewer) return viewer;
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                var child = VisualTreeHelper.GetChild(depObj, i);
                var result = GetScrollViewer(child);
                if (result != null) return result;
            }
            return null;
        }
    }
}
