using System;
using UrNotes.Models.DTOs;

namespace UrNotes.Models {
  public class Note {
    public Guid ID { get; }

    private string name = "";
    private string html = "";
    private bool isPinned = false;

    public DateTime CreatedAt { get; }
    public DateTime LastModified { get; private set; }

    //CreateNote Constructor
    public Note(Guid ID, string name) {
      this.ID = ID;

      if (name == null)
        throw new ArgumentNullException("The provided Note name is NULL");

      if (name == String.Empty)
        name = "Untitled";

      name = name.Trim();
      this.name = name;
      CreatedAt = DateTime.Now;
      LastModified = DateTime.Now;
    }

    //Serializing Constructor
    public Note(Guid id, string name, string html, bool isPinned, DateTime createdAt, DateTime lastModified) {
      this.ID = id;
      if (name == null)
        throw new ArgumentNullException("The provided Note name is NULL");
      if (name == String.Empty)
        name = "Untitled";
      this.name = name.Trim();
      this.html = html ?? "";
      this.isPinned = isPinned;
      this.CreatedAt = createdAt;
      this.LastModified = lastModified;
    }

    //Properties
    public string Name { 
      set { 
        name = value; 
        LastModified = DateTime.Now;
      } get { return name; } }

    public string Html {
      set {
        html = value;
        LastModified = DateTime.Now;
      }
      get { return html; }
    }

    public bool IsPinned {
      set {
        isPinned = value;
        LastModified = DateTime.Now;
      }
      get { return isPinned; } }

    public static NoteDTO toNoteDTO(Note note) => new NoteDTO {
      ID = note.ID,
      Name = note.Name,
      Html = note.Html,
      IsPinned = note.IsPinned,
      CreatedAt = note.CreatedAt,
      LastModified = note.LastModified
    };
  }
}
