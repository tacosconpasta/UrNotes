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


namespace UrNotes.Views.UserControls.General;

public sealed partial class LeftPanelMenu : UserControl {
  public event RoutedEventHandler? NewNoteRequested;
  public event EventHandler<Note>? NoteSelected;

  public LeftPanelMenu() {
    this.InitializeComponent();

    CreateNoteButtonComponent.CreateNoteClicked += (s, e) => {
      //We keep propagating upwards, bubbling, until we hit the MainView so we can trigger the NewTab from the tabView in MainView
      NewNoteRequested?.Invoke(this, e);
    };
  }

  private void NotesListView_SelectionChanged(object sender, SelectionChangedEventArgs e) {
    if (e.AddedItems.Count > 0 && e.AddedItems[0] is Note selectedNote) {
      //Update the ViewModel's SelectedNote
      if (this.DataContext is NotesViewModel viewModel) {
        viewModel.SelectedNote = selectedNote;
      }
      //Notify the parent that a note was selected
      NoteSelected?.Invoke(this, selectedNote);
    } else {
      Console.WriteLine("No valid note selected");
    }
  }
}
