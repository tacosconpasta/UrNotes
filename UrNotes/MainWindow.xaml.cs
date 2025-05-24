using System.Windows;
using UrNotes.ViewModel;

namespace UrNotes {

  public partial class MainWindow : Window {
    public MainWindow() {
      InitializeComponent();
      DataContext = new NotesViewModel();
    }
  }

}