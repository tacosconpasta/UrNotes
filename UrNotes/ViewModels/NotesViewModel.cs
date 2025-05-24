using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using UrNotes.Models;
using UrNotes.Models.DTOs;
using UrNotes.Services;

namespace UrNotes.ViewModel {
  class NotesViewModel {
    private NotesDataManager dataFolder = new NotesDataManager();
    public ObservableCollection<Note> Notes { get; } = new ObservableCollection<Note>();

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
  }
}
