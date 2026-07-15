using Microsoft.UI.Xaml.Controls;
using System;
using Windows.UI;

namespace UrNotes.Views.UserControls.General {
  public sealed partial class ToolBox : UserControl {
    //Fired when the font size counter changes, carries the new size
    public event EventHandler<int>? FontSizeChanged;

    //Fired when a color is picked on the color square, carries the picked color
    public event EventHandler<Color>? ColorSelected;

    public ToolBox() {
      this.InitializeComponent();

      //Bubble the inner controls' events upwards so the owner can apply them to its editor
      FontSizeCounter.ValueChanged += (s, newSize) => FontSizeChanged?.Invoke(this, newSize);
      TextColorSquare.ColorChanged += (s, color) => ColorSelected?.Invoke(this, color);
    }
  }
}
