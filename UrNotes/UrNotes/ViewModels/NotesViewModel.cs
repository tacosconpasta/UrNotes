using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using UrNotes.Models;
using UrNotes.Models.DTOs;
using UrNotes.Services;

namespace UrNotes.ViewModel {
  public class NotesViewModel {
    private NotesDataManager dataFolder = new NotesDataManager();
    public ObservableCollection<Note> Notes { get; } = new ObservableCollection<Note>();

    //Notes that the list actually shows, only the ones matching the search query
    //(Notes can't be filtered directly because saving would only keep the filtered ones)
    public ObservableCollection<Note> FilteredNotes { get; } = new ObservableCollection<Note>();

    private string searchQuery = "";
    public string SearchQuery {
      get => searchQuery;
      set {
        searchQuery = value ?? "";
        applyFilter();
        OnPropertyChanged();
      }
    }

    private Note? selectedNote;
    public Note? SelectedNote {
      get => selectedNote;
      set {
        selectedNote = value;
        OnPropertyChanged();
      }
    }

    public NotesViewModel() {
      //Keep the filtered list updated whenever a note is added/removed
      Notes.CollectionChanged += (s, e) => applyFilter();

      loadNotes();
    }

    //Refills FilteredNotes with the notes whose name matches the search query
    private void applyFilter() {
      string query = searchQuery.Trim();

      FilteredNotes.Clear();
      foreach (Note note in Notes) {
        if (query == String.Empty || note.Name.Contains(query, StringComparison.OrdinalIgnoreCase)) {
          FilteredNotes.Add(note);
        }
      }
    }

    private void loadNotes() {
      Notes.Clear();

      //Receive the NoteDTO (Note with public properties) List from json
      List<NoteDTO> dataNotes = dataFolder.loadNotesData();

      //For each NoteDTO, convert to Note and add to Notes
      foreach (NoteDTO dataNote in dataNotes) {
        Note note = NoteDTO.toNote(dataNote);
        Notes.Add(note);
      }
    }

    public void createNote(String name, String html) {
      Guid guid = Guid.NewGuid();
      Note newNote = new Note(guid, name, html);
      Notes.Add(newNote);

      dataFolder.saveNotesData(Notes);
      loadNotes();
    }

    public void addNote(Note note) {
      Notes.Add(note);

      dataFolder.saveNotesData(Notes);
      loadNotes();
    }

    //TODO: Method saves ALL notes, it should just modify/overwrite the contents of the existing one
    public void SaveNote(Note note) {
      // Save all notes to persist changes
      dataFolder.saveNotesData(Notes);

      // Optionally trigger property changed if needed
      OnPropertyChanged(nameof(Notes));
    }

    // Also add a method to save all notes
    public void SaveAllNotes() {
      dataFolder.saveNotesData(Notes);
    }

    //Renames a Note
    public void RenameNote(Guid noteToRenameID, string newName) {
      foreach (Note note in Notes) {
        if (note.ID == noteToRenameID) {
          note.Name = newName;
        }
      } 
    }

    //If ID already exists in List, returns true
    public bool existsID(Guid id) {
      if (id == Guid.Empty) return false;
      if (Notes.Count == 0) return false;

      foreach (Note note in Notes) {
        if (note.ID == id) {
          return true;
        }
      }

      //ID wasn't found on notes
      return false;
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null) {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
  }
}
