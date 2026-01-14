using System.Windows;
using System.Windows.Controls;

namespace ThunderCut.Views
{
    public partial class CutPreviewView : UserControl
    {
        public event EventHandler? BackToDesign;
        public event EventHandler? ProceedToPouchDesign;

        public CutPreviewView()
        {
            InitializeComponent();
        }

        public void SetModel(PhoneModel model, TemplateType type)
        {
            PreviewCanvas.SelectedModel = model;
            PreviewCanvas.SelectedTemplateType = type;
            MaterialSizeText.Text = $"{model.WidthMm:0.0} x {model.HeightMm:0.0} mm";
            
            // Auto zoom to fit in preview
            PreviewCanvas.CenterTemplate((int)ActualWidth, (int)ActualHeight);
        }

        private void OnBackToDesign(object sender, RoutedEventArgs e)
        {
            BackToDesign?.Invoke(this, EventArgs.Empty);
        }

        private async void OnSendToCutter(object sender, RoutedEventArgs e)
        {
            SendToCutterBtn.IsEnabled = false;
            SendToCutterBtn.Content = "PREPARING...";

            // Simulate Data Preparation
            await Task.Delay(1000);

            var result = MessageBox.Show("Cutter is ready. Begin cutting sequence?", "ThunderCut Machine", MessageBoxButton.YesNo, MessageBoxImage.Question);
            
            if (result == MessageBoxResult.Yes)
            {
                SendToCutterBtn.Content = "CUTTING: 0%";
                
                // Simulate cutting progress
                for (int i = 0; i <= 100; i += 5)
                {
                    SendToCutterBtn.Content = $"CUTTING: {i}%";
                    await Task.Delay(200);
                }

                MessageBox.Show("Cutting complete! Proceeding to Pouch Design...", "Machine Status", MessageBoxButton.OK, MessageBoxImage.Information);
                ProceedToPouchDesign?.Invoke(this, EventArgs.Empty);
            }

            SendToCutterBtn.Content = "SEND TO CUTTER";
            SendToCutterBtn.IsEnabled = true;
        }
    }
}
