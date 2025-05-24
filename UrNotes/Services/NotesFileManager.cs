using System.IO;
using System.Text.Json;
using UrNotes.Model;

namespace UrNotes.Services {
  public class NotesFileManager {
    private readonly string dataDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", "notes");
    private DirectoryInfo NotesDirectory;
    private FileInfo? jsonData;

    public NotesFileManager() {
      NotesDirectory = new DirectoryInfo(dataDirectory);

      if (!NotesDirectory.Exists) {
        NotesDirectory.Create();
      }
      readDirectory();
    }

    //Searches for data.json and assigns it
    public void readDirectory() {
      //Get files from NotesDirectory
      FileInfo[] files = NotesDirectory.GetFiles();

      //Search for data.json and assign to jsonData
      foreach (FileInfo file in files) {
        if (file.Name == "data.json") {
          jsonData = file;
          break;
        }
      }
    }

    //Used to load the JSON data, returning a List of Notes
    public List<Note> loadNotesData() {
      //If jsonData is null; or file doesn't exist; throw exception.
      if (jsonData == null || !jsonData.Exists) {
        throw new FileNotFoundException("The data.json file was not found in " + dataDirectory);
      }

      //Read all text from jsonData file
      string jsonString = File.ReadAllText(jsonData.FullName);

      //Deserialize into a Note List.
      var noteList = JsonSerializer.Deserialize<List<Note>>(jsonString);

      //If noteList is NOT null, return it; else, return a new empty Note List.
      return noteList ?? new List<Note>();

    }

    //Used to serialize the JSON data
    public void saveNotesData(IEnumerable<Note> notes) {
      //Create the data.json file path
      string filePath = Path.Combine(dataDirectory, "data.json");

      //Serialize notes to JSON
      string jsonString = JsonSerializer.Serialize(notes.ToList(), new JsonSerializerOptions { WriteIndented = true });

      //Write to file
      File.WriteAllText(filePath, jsonString);

      //Update jsonData
      jsonData = new FileInfo(filePath);
    }
  }
}
