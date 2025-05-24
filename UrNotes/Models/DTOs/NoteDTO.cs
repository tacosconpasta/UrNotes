using UrNotes.Models;

namespace UrNotes.Models.DTOs {
  public class NoteDTO {
    public Guid ID { get; set; }
    public string Name { get; set; } = "";
    public string Html { get; set; } = "";
    public bool IsPinned { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastModified { get; set; }

    public static NoteDTO toNoteDTO(Note note) => new NoteDTO {
      ID = note.ID,
      Name = note.Name,
      Html = note.Html,
      IsPinned = note.IsPinned,
      CreatedAt = note.CreatedAt,
      LastModified = note.LastModified
    };

    public static Note toNote(NoteDTO dto) {
      return new Note(dto.ID, dto.Name, dto.Html, dto.IsPinned, dto.CreatedAt, dto.LastModified);
    }
  };
}
