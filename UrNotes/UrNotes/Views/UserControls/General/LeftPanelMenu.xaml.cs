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
using System.Diagnostics;
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

  private void RenameNoteFlyoutButton_Click(object sender, RoutedEventArgs e) {
    var menuFlyoutItem = sender as MenuFlyoutItem;
    if (menuFlyoutItem?.Tag is not Note note) {
      Console.WriteLine("Note was null");
      return;
    }

    // Get the ListViewItem container for this Note
    var listViewItem = NotesListView.ContainerFromItem(note) as ListViewItem;
    if (listViewItem == null) {
      Console.WriteLine("ListViewItem was null");
      return;
    }

    // Get the root of the DataTemplate (the Grid)
    var grid = listViewItem.ContentTemplateRoot as Grid;
    if (grid == null) {
      Console.WriteLine("Grid was null");
      return;
    }

    // Find the TextBox and TextBlock by name
    var textBox = grid.FindName("NoteTextBox") as TextBox;
    var textBlockInGrid = grid.FindName("NoteTextBlock") as TextBlock;
    if (textBox == null || textBlockInGrid == null) {
      Console.WriteLine("TextBox or TextBlock was null");
      return;
    }

    // Switch from display to edit mode
    textBlockInGrid.Visibility = Visibility.Collapsed;
    textBox.Visibility = Visibility.Visible;

    // Focus and select all text for editing
    textBox.Focus(FocusState.Programmatic);
    textBox.SelectAll();
  }

  //Cancel rename when focus is lost
  private void NoteTextBox_LostFocus(object sender, RoutedEventArgs e) {
    CancelRename(sender as TextBox);
  }

  //Commit rename when Enter is pressed
  private void NoteTextBox_KeyDown(object sender, KeyRoutedEventArgs e) {
    if (e.Key == Windows.System.VirtualKey.Enter) {
      CommitRename(sender as TextBox);
    } else if (e.Key == Windows.System.VirtualKey.Escape) {
      CancelRename(sender as TextBox);
    }
  }


  private void CommitRename(TextBox? textBox) {
    if (textBox == null || textBox.DataContext is not Note note) return;

    //Update note
    var vm = DataContext as NotesViewModel;

    if (vm != null) {
      vm.RenameNote(note.ID, textBox.Text);
      vm.SaveAllNotes();
    }

    // Switch back to TextBlock
    var grid = textBox.Parent as Grid;
    var textBlock = grid?.Children.OfType<TextBlock>().FirstOrDefault();
    if (textBlock != null) {
      textBlock.Visibility = Visibility.Visible;
      textBox.Visibility = Visibility.Collapsed;
    }
  }

  private void CancelRename(TextBox? textBox) {
    if (textBox == null) return;

    // Revert value to original
    if (textBox.DataContext is Note note) {
      textBox.Text = note.Name;
    }

    // Switch back to TextBlock
    var grid = textBox.Parent as Grid;
    var textBlock = grid?.Children.OfType<TextBlock>().FirstOrDefault();
    if (textBlock != null) {
      textBlock.Visibility = Visibility.Visible;
      textBox.Visibility = Visibility.Collapsed;
    }
  }
}
