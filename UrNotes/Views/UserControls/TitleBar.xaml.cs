using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace UrNotes.Views.UserControls {
  public partial class TitleBar : UserControl {

    public TitleBar() {
      InitializeComponent();
      this.DataContext = this;
    }

    private void closeBtn_Click(object sender, RoutedEventArgs e) {
      Window.GetWindow(this).Close();
    }

    private void minimizeBtn_Click(object sender, RoutedEventArgs e) {
      Window window = Window.GetWindow(this);
      window.WindowState = WindowState.Minimized;
    }

    private void maximizeBtn_Click(object sender, RoutedEventArgs e) {
      Window window = Window.GetWindow(this);

      //Togle maximize/restore on click
      toggleMaximize(window);
    }

    private void titleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
      Window window = Window.GetWindow(this);
      if (window == null) return;

      if (e.ClickCount == 2) {
        toggleMaximize(window);
        return;
      }

      if (window.WindowState == WindowState.Maximized) {
        //Get mouse position relative to the control
        Point mousePositionInWindow = e.GetPosition(this);

        //Convert to screen coordinates
        Point mousePositionOnScreen = PointToScreen(mousePositionInWindow);

        //Calculate horizontal percentage
        double percentHorizontal = mousePositionInWindow.X / ActualWidth;

        //Restore window
        window.WindowState = WindowState.Normal;

        //Move window to maintain cursor position
        window.Left = mousePositionOnScreen.X - (window.RestoreBounds.Width * percentHorizontal);
        window.Top = 0;
      }

      window.DragMove();
    }

    private void toggleMaximize(Window window) {
      if (window.WindowState == WindowState.Maximized) {
        window.WindowState = WindowState.Normal;
        maximizeSvg.Source = new Uri("pack://application:,,,/assets/maximize-20.svg", UriKind.Absolute);
      } else {
        window.WindowState = WindowState.Maximized;
        maximizeSvg.Source = new Uri("pack://application:,,,/assets/maximized-20.svg", UriKind.Absolute);
      }
    }

    //Events
    public event PropertyChangedEventHandler ?PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string propertyName = "") {
      if (!string.IsNullOrEmpty(propertyName)) {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
      }
    }
  }
}
