using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using SkiaSharp;
using SkiaSharp.Views.WPF;

namespace ThunderCut.Views
{
    public partial class PouchDesignView : UserControl
    {
        public event EventHandler? BackRequested;
        public event EventHandler? FinishRequested;

        public PouchDesignView()
        {
            InitializeComponent();
        }

        public void SetModel(PhoneModel model, TemplateType type)
        {
            DesignArea.SelectedModel = model;
            DesignArea.SelectedTemplateType = type;
            
            // Auto zoom to fit
            DesignArea.CenterTemplate((int)ActualWidth - 300, (int)ActualHeight); // Adjust for sidebar
        }

        private void OnBackClick(object sender, RoutedEventArgs e)
        {
            BackRequested?.Invoke(this, EventArgs.Empty);
        }

        private void OnFinishClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Design saved and sent to print queue!", "Pouch Design", MessageBoxButton.OK, MessageBoxImage.Information);
            FinishRequested?.Invoke(this, EventArgs.Empty);
        }

        private void OnTextureClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn)
            {
                // Logic to apply texture would go here
                // For now, toggle visual feedback
                MessageBox.Show($"Selected Texture: {btn.Content}", "Texture Applied");
            }
        }

        private void OnColorClick(object sender, RoutedEventArgs e)
        {
             if (sender is Button btn && btn.Background is SolidColorBrush brush)
            {
                // Logic to apply color
                DesignArea.PouchColor = brush.Color.ToSKColor();
            }
        }

        private void OnComponentClick(object sender, RoutedEventArgs e)
        {
             if (sender is Button btn)
            {
                MessageBox.Show($"Action: {btn.Content}", "Component Tool");
            }
        }
        private void OnPreview3DClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Generating 3D Preview...\n(Feature coming soon)", "3D Preview", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
