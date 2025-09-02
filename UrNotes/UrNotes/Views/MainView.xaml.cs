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
    private int tabCounter = 1;

    // Dictionary to store save timers for each note
    private Dictionary<Guid, DispatcherTimer> saveTimers = new Dictionary<Guid, DispatcherTimer>();

    public MainView() {
      this.InitializeComponent();
      this.DataContext = ViewModel;

      // Subscribe to note selection events
      LeftPanelMenuComponent.NoteSelected += OnNoteSelected;
      LeftPanelMenuComponent.NewNoteRequested += (s,e) => LeftPanelMenuComponent_OnNewNoteRequested();
    }

    private void OnNoteSelected(object? sender, Note selectedNote) {
      System.Diagnostics.Debug.WriteLine($"OnNoteSelected called with note: {selectedNote.Name}");

      // Check if note is already open in a tab
      var existingTab = FindTabForNote(selectedNote);
      if (existingTab != null) {
        System.Diagnostics.Debug.WriteLine("Found existing tab, switching to it");
        NotesTabView.SelectedItem = existingTab;
        return;
      }

      System.Diagnostics.Debug.WriteLine("Creating new tab for note");
      // Create new tab for the note
      CreateNoteTab(selectedNote);
    }

    private TabViewItem FindTabForNote(Note note) {
      return NotesTabView.TabItems.Cast<TabViewItem>()
          .FirstOrDefault(tab => tab.Tag?.Equals(note.ID) == true);
    }

    private void CreateNoteTab(Note note) {
      System.Diagnostics.Debug.WriteLine($"Creating tab for note: {note.Name}");
      System.Diagnostics.Debug.WriteLine($"Note HTML content: '{note.Html}'");

      // Create RichEditBox for rich text editing
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

      System.Diagnostics.Debug.WriteLine($"Adding tab to TabView. Current tab count: {NotesTabView.TabItems.Count}");
      NotesTabView.TabItems.Add(newTab);
      NotesTabView.SelectedItem = newTab;
      System.Diagnostics.Debug.WriteLine($"Tab added. New tab count: {NotesTabView.TabItems.Count}");
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
          System.Diagnostics.Debug.WriteLine($"Auto-saving note: {note.Name}");
          SaveNote(note);
        } catch (Exception ex) {
          System.Diagnostics.Debug.WriteLine($"Error during auto-save: {ex.Message}");
        }
      };

      saveTimers[note.ID] = timer;
      timer.Start();
    }

    private void SaveNote(Note note) {
      System.Diagnostics.Debug.WriteLine($"Saving note: {note.Name}");
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

    //ADD NOTES
    private void AddNewNoteTab() {
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
        Header = $"Tab {tabCounter++}",
        Content = parentGrid
      };

      NotesTabView.TabItems.Add(newTab);
      NotesTabView.SelectedItem = newTab;
    }

    //Allows for the "+" on the TabView to add a new note
    private void NotesTabView_AddTabButtonClick(TabView sender, object args) {
      AddNewNoteTab();
    }

    //Allows for the "Create New Note" Button to add a new note
    private void LeftPanelMenuComponent_OnNewNoteRequested() {
      AddNewNoteTab();
    }


    private void NotesTabView_TabCloseRequested(TabView sender, TabViewTabCloseRequestedEventArgs args) {
      // Save the note before closing the tab
      if (args.Item is TabViewItem tab && tab.Tag is Guid noteId) {
        var note = ViewModel.Notes.FirstOrDefault(n => n.ID == noteId);
        if (note != null && tab.Content is Grid grid && grid.Children.LastOrDefault() is RichEditBox rtb) {
          try {
            string content;
            rtb.Document.GetText(Microsoft.UI.Text.TextGetOptions.FormatRtf, out content);
            note.Html = content;
            SaveNote(note);
          } catch (Exception ex) {
            System.Diagnostics.Debug.WriteLine($"Error saving note on tab close: {ex.Message}");
          }
        }

        // Cancel any pending save timer
        if (saveTimers.ContainsKey(noteId)) {
          saveTimers[noteId].Stop();
          saveTimers.Remove(noteId);
        }
      }

      NotesTabView.TabItems.Remove(args.Item);
    }
  }
}