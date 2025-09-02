using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UrNotes.Models;
using UrNotes.ViewModel;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace UrNotes.Views.UserControls.General;

public sealed partial class LeftPanelMenu : UserControl {
  public event RoutedEventHandler? NewNoteRequested;
  public event EventHandler<Note>? NoteSelected;

  public LeftPanelMenu() {
    this.InitializeComponent();

    this.Loaded += (s, e) => {
      Console.WriteLine($"LeftPanelMenu DataContext: {this.DataContext?.GetType().Name}");

      if (this.DataContext is NotesViewModel vm) {
        Console.WriteLine($"Notes count: {vm.Notes.Count}");
        foreach (var note in vm.Notes) {
          Console.WriteLine($"Note: {note.Name}");
        }
      } else {
        Console.WriteLine("DataContext is not NotesViewModel");
      }
    };
    CreateNoteButtonComponent.CreateNoteClicked += (s, e) =>
        NewNoteRequested?.Invoke(this, e);
  }

  private void NotesListView_SelectionChanged(object sender, SelectionChangedEventArgs e) {
    Console.WriteLine($"Selection changed. Added items: {e.AddedItems.Count}");

    if (e.AddedItems.Count > 0 && e.AddedItems[0] is Note selectedNote) {
      Console.WriteLine($"Selected note: {selectedNote.Name}");

      // Update the ViewModel's SelectedNote
      if (this.DataContext is NotesViewModel viewModel) {
        viewModel.SelectedNote = selectedNote;
        Console.WriteLine("Updated ViewModel SelectedNote");
      }

      // Notify the parent that a note was selected
      Console.WriteLine("Firing NoteSelected event");
      NoteSelected?.Invoke(this, selectedNote);
    } else {
      Console.WriteLine("No valid note selected");
    }
  }
}
