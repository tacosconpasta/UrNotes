using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using UrNotes.Models;
using UrNotes.ViewModel;
using UrNotes.Views.UserControls.General;

namespace UrNotes.Views {
  public sealed partial class MainView : UserControl {
    private NotesViewModel ViewModel = new NotesViewModel();
    private Dictionary<Guid, Note> UnsavedTabs = new Dictionary<Guid, Note>();
    private int tabCounter = 1;

    // Dictionary to store save timers for each note
    private Dictionary<Guid, DispatcherTimer> saveTimers = new Dictionary<Guid, DispatcherTimer>();

    public MainView() {
      this.InitializeComponent();
      this.DataContext = ViewModel;

      // Subscribe to note selection events
      LeftPanelMenuComponent.NoteSelected += OnNoteSelected;

      //Join the event triggered by the "NewNoteButton" with a function on this class, so that the function triggers
      //when the event is fired by the click
      LeftPanelMenuComponent.NewNoteRequested += (s,e) => LeftPanelMenuComponent_OnNewNoteRequested();
    }

    private void OnNoteSelected(object? sender, Note selectedNote) {
      // Check if note is already open in a tab
      var existingTab = FindTabForNote(selectedNote);
      if (existingTab != null) {
        NotesTabView.SelectedItem = existingTab;
        return;
      }

      // Create new tab for the note
      OpenExistingNoteTab(selectedNote);
    }

    private TabViewItem FindTabForNote(Note note) {
      return NotesTabView.TabItems.Cast<TabViewItem>()
          .FirstOrDefault(tab => tab.Tag?.Equals(note.ID) == true);
    }

    //Opens an existing note
    private void OpenExistingNoteTab(Note note) {
      Console.WriteLine($"Creating tab for note: {note.Name}");

      //Create RichEditBox for rich text editing
      var rtb = new RichEditBox {
        AcceptsReturn = true,
        IsSpellCheckEnabled = true,
        Margin = new Thickness(10),
        VerticalAlignment = VerticalAlignment.Stretch,
        HorizontalAlignment = HorizontalAlignment.Stretch,
        BorderThickness = new Thickness(0)
      };

      // Load the note's content
      if (!string.IsNullOrEmpty(note.Html)) {
        try {
          // Try to load as RTF first
          rtb.Document.SetText(Microsoft.UI.Text.TextSetOptions.FormatRtf, note.Html);
        } catch {
          try {
            // If RTF fails, try as plain text
            rtb.Document.SetText(Microsoft.UI.Text.TextSetOptions.None, note.Html);
          } catch (Exception ex) {
            System.Diagnostics.Debug.WriteLine($"Error loading content: {ex.Message}");
            rtb.Document.SetText(Microsoft.UI.Text.TextSetOptions.None, "Start typing your note here...");
          }
        }
      } else {
        rtb.Document.SetText(Microsoft.UI.Text.TextSetOptions.None, "Start typing your note here...");
      }

      // Handle text changes with debounced saving
      rtb.TextChanged += (s, e) => {
        DebouncedSave(note, rtb);
      };

      var parentGrid = new Grid {
        VerticalAlignment = VerticalAlignment.Stretch,
        HorizontalAlignment = HorizontalAlignment.Stretch,
        Background = Application.Current.Resources["TabViewItemHeaderBackgroundSelected"] as Brush
      };

      var randomGrid = new Grid {
        Padding = new Thickness(10),
        Background = Application.Current.Resources["TabViewItemHeaderBackgroundSelected"] as Brush,
        TabIndex = 0
      };

      parentGrid.Children.Add(randomGrid);
      parentGrid.Children.Add(rtb);

      var newTab = new TabViewItem {
        Header = note.Name,
        Content = parentGrid,
        Tag = note.ID // Store the note ID for identification
      };

      // Subscribe to note name changes
      note.PropertyChanged += (s, e) => {
        if (e.PropertyName == nameof(Note.Name)) {
          // Update tab header automatically
          newTab.Header = note.Name;
        }
      };

      NotesTabView.TabItems.Add(newTab);
      NotesTabView.SelectedItem = newTab;
    }

    //ADD a NEW NOTE
    private void AddNewNoteTab() {
      Guid noteID = Guid.NewGuid();
      string noteName = $"Tab {tabCounter++}";
      string defaultText = "Start typing here...";

      //Variable for saving the new Note
      Note newNote;

      //Make sure ID is not on Notes List
      while (!ViewModel.existsID(noteID)) {
        Console.WriteLine("id exists in view model");
        noteID = Guid.NewGuid();
        Console.WriteLine("ID exists, generating another one: " + noteID); 
      }

      //Then, create Note, but don't save yet
      newNote = new Note(noteID, noteName, defaultText);
      Console.WriteLine("New Note created, but not added to anything yet");

      //Add it to the unsavedTabsList
      if (newNote != null) {
        Console.WriteLine("new note is being added to unsavedtabs dictionary");
        UnsavedTabs.Add(newNote.ID, newNote);
      } else {
        //return so app doesn't crash
        Console.WriteLine("newNote was null, so let's return nothing");
        return;
      }

      //TODO: Add verification that, if closed tab Tag is in UnsavedNotes List, trigger popup to save in ViewModel
      var rtb = new RichEditBox {
        AcceptsReturn = true,
        IsSpellCheckEnabled = true,
        Margin = new Thickness(10),
        VerticalAlignment = VerticalAlignment.Stretch,
        HorizontalAlignment = HorizontalAlignment.Stretch,
        BorderThickness = new Thickness(0),
      };

      var parentGrid = new Grid {
        VerticalAlignment = VerticalAlignment.Stretch,
        HorizontalAlignment = HorizontalAlignment.Stretch,
        Background = Application.Current.Resources["TabViewItemHeaderBackgroundSelected"] as Brush
      };

      var randomGrid = new Grid {
        Padding = new Thickness(10),
        Background = Application.Current.Resources["TabViewItemHeaderBackgroundSelected"] as Brush,
        TabIndex = 0
      };

      parentGrid.Children.Add(randomGrid);
      parentGrid.Children.Add(rtb);

      var newTab = new TabViewItem {
        Header = newNote.Name,
        Content = parentGrid,
        Tag = newNote.ID,
      };

      NotesTabView.TabItems.Add(newTab);
      NotesTabView.SelectedItem = newTab;
    }

    private void DebouncedSave(Note note, RichEditBox rtb) {
      // Cancel existing timer if it exists
      if (saveTimers.ContainsKey(note.ID)) {
        saveTimers[note.ID].Stop();
        saveTimers.Remove(note.ID);
      }

      // Create new timer that will save after 5 seconds of no changes
      var timer = new DispatcherTimer {
        Interval = TimeSpan.FromSeconds(5)
      };

      timer.Tick += (s, e) => {
        timer.Stop();
        saveTimers.Remove(note.ID);

        // Save the content
        try {
          string content;
          rtb.Document.GetText(Microsoft.UI.Text.TextGetOptions.FormatRtf, out content);
          note.Html = content;
          Console.WriteLine($"Auto-saving note: {note.Name}");
          SaveNote(note);
        } catch (Exception ex) {
          Console.WriteLine($"Error during auto-save: {ex.Message}");
        }
      };

      saveTimers[note.ID] = timer;
      timer.Start();
    }

    private void SaveNote(Note note) {
      Console.WriteLine($"Saving note: {note.Name}");
      ViewModel.SaveNote(note);
    }

    // Method to save all open notes immediately (useful when app closes)
    public void SaveAllOpenNotes() {
      // Stop all timers and save immediately
      foreach (var timer in saveTimers.Values) {
        timer.Stop();
      }
      saveTimers.Clear();

      // Save all notes
      foreach (var tabObj in NotesTabView.TabItems) {
        if (tabObj is TabViewItem tab && tab.Tag is Guid noteId) {
          var note = ViewModel.Notes.FirstOrDefault(n => n.ID == noteId);
          if (note != null && tab.Content is Grid grid && grid.Children.LastOrDefault() is RichEditBox rtb) {
            try {
              string content;
              rtb.Document.GetText(Microsoft.UI.Text.TextGetOptions.FormatRtf, out content);
              note.Html = content;
              SaveNote(note);
            } catch (Exception ex) {
              System.Diagnostics.Debug.WriteLine($"Error saving note on exit: {ex.Message}");
            }
          }
        }
      }
    }

    //Allows for the "+" on the TabView to add a new note
    private void NotesTabView_AddTabButtonClick(TabView sender, object args) {
      AddNewNoteTab();
    }

    //Allows for the "Create New Note" Button to add a new note
    private void LeftPanelMenuComponent_OnNewNoteRequested() {
      AddNewNoteTab();
    }

    private async void NotesTabView_TabCloseRequested(TabView sender, TabViewTabCloseRequestedEventArgs args) {
      // Check that the closed item is a tab with a valid note ID
      if (args.Item is not TabViewItem tab || tab.Tag is not Guid noteId)
        return;

      // If the note has unsaved changes
      if (UnsavedTabs.TryGetValue(noteId, out var note)) {
        // Create a TextBox to allow renaming the note before saving
        var nameTextBox = new TextBox {
          Text = note.Name,
          Margin = new Thickness(0, 10, 0, 0)
        };

        // Create the dialog
        var dialog = new ContentDialog {
          Title = "Save changes?",
          Content = new StackPanel {
            Children =
                {
                    new TextBlock { Text = "Do you want to save this note before closing?" },
                    new TextBlock { Text = "Edit note name:" },
                    nameTextBox
                }
          },
          PrimaryButtonText = "Save",
          SecondaryButtonText = "Don't Save",
          CloseButtonText = "Cancel",
          XamlRoot = this.XamlRoot
        };

        // Show the dialog and get the result
        var result = await dialog.ShowAsync();

        switch (result) {
          case ContentDialogResult.Primary:
            // Save the note and update its name
            note.Name = nameTextBox.Text;

            if (tab.Content is Grid grid && grid.Children.LastOrDefault() is RichEditBox rtb) {
              rtb.Document.GetText(Microsoft.UI.Text.TextGetOptions.FormatRtf, out string content);
              note.Html = content;
              ViewModel.addNote(note);
            }

            UnsavedTabs.Remove(noteId);
            break;

          case ContentDialogResult.Secondary:
            // Don't save, just remove from unsaved tabs
            UnsavedTabs.Remove(noteId);
            break;

          case ContentDialogResult.None:
            // Cancel, leave the tab open
            return;
        }
      } else {
        // Note already saved, stop any pending save timer
        if (saveTimers.TryGetValue(noteId, out var timer)) {
          timer.Stop();
          saveTimers.Remove(noteId);
        }
      }

      // Close the tab
      NotesTabView.TabItems.Remove(tab);
    }

  }
}