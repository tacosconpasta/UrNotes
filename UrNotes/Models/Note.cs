namespace UrNotes.Model {
  public class Note {
    public Guid ID { get; }

    private string name = "";
    private string html = "";
    private bool isPinned = false;

    public DateTime CreatedAt { get; }
    public DateTime LastModified { get; private set; }

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



  }
}
