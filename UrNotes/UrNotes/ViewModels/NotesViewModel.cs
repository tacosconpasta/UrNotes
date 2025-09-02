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

    private Note? selectedNote;
    public Note? SelectedNote {
      get => selectedNote;
      set {
        selectedNote = value;
        OnPropertyChanged();
      }
    }

    public NotesViewModel() {
      loadNotes();
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

    public void createNote(String name) {
      Guid guid = Guid.NewGuid();
      Note newNote = new Note(guid, name);
      Notes.Add(newNote);

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

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null) {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
  }
}
