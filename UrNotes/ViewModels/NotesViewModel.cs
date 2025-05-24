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

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null) {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
  }
}
