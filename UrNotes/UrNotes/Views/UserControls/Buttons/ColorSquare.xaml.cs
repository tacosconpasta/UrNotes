using Microsoft.UI;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System;
using Windows.UI;

namespace UrNotes.Views.UserControls.Buttons {
  public sealed partial class ColorSquare : UserControl {
    //Fired when the user picks a color on the color wheel, carries the picked color
    public event EventHandler<Color>? ColorChanged;

    public ColorSquare() {
      this.InitializeComponent();

      //White is the starting color
      Picker.Color = Colors.White;
    }

    private void Picker_ColorChanged(ColorPicker sender, ColorChangedEventArgs args) {
      //The square always reflects the current color
      ColorRect.Fill = new SolidColorBrush(args.NewColor);
      ColorChanged?.Invoke(this, args.NewColor);
    }
  }
}
