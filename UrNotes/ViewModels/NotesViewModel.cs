using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Timers;
using UrNotes.Model;

namespace UrNotes.ViewModel {
  class NotesViewModel {
    private readonly string NotesDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", "notes");

    public ObservableCollection<Note> Notes { get; } = new ObservableCollection<Note>();

  }
}
