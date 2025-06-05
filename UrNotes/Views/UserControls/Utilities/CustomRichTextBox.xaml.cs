using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace UrNotes.Views.UserControls.Utilities { 
    public partial class CustomRichTextBox : UserControl {
    private const double ZOOM_INCREMENT = 0.1;
    private const double MIN_ZOOM = 1.0;
    private const double MAX_ZOOM = 5.0;

    public CustomRichTextBox() {
            InitializeComponent();
        }

    /// <summary>
    /// Handles mouse wheel events for zoom functionality when Ctrl key is held || Mousepad is used
    /// </summary>

    private void RichTextBox_PreviewMouseWheel(object sender, MouseWheelEventArgs e) {
      // Only zoom when Ctrl key is pressed, otherwise allow normal scrolling
      if (Keyboard.Modifiers == ModifierKeys.Control) {
        e.Handled = true; // Prevent default scroll behavior

        // Calculate zoom direction: positive delta = zoom in, negative = zoom out
        double zoomDelta = e.Delta > 0 ? ZOOM_INCREMENT : -ZOOM_INCREMENT;

        // Get current zoom level from the transform
        double currentZoom = ZoomTransform.ScaleX;

        // Calculate new zoom level within allowed bounds
        double newZoom = Math.Max(MIN_ZOOM, Math.Min(MAX_ZOOM, currentZoom + zoomDelta));

        // Apply the new zoom level to both X and Y axes
        ZoomTransform.ScaleX = newZoom;
        ZoomTransform.ScaleY = newZoom;
      }
    }
  }
}
