using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using UrNotes.Model;
using UrNotes.Services;

namespace UrNotes.ViewModel {
  class NotesViewModel {
    private NotesDataManager dataFolder = new NotesDataManager();
    public ObservableCollection<Note> Notes { get; } = new ObservableCollection<Note>();

    public NotesViewModel() {
      
    }
  }
}
