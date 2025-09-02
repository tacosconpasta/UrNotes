using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using UrNotes.Models;
using UrNotes.Models.DTOs;

namespace UrNotes.Services {
  public class NotesDataManager {
    private readonly string dataDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", "notes");
    private DirectoryInfo NotesDirectory;
    private FileInfo? jsonData;

    public NotesDataManager() {
      NotesDirectory = new DirectoryInfo(dataDirectory);

      if (!NotesDirectory.Exists) {
        NotesDirectory.Create();
      }
      readDirectoryJSON();
    }

    //Searches for data.json and assigns it
    public void readDirectoryJSON() {
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

    //Used to load the JSON data, returning a List of NoteDTOs
    public List<NoteDTO> loadNotesData() {
      if (jsonData == null || !jsonData.Exists) {
        string jsonPath = Path.Combine(dataDirectory, "data.json");
        jsonData = new FileInfo(jsonPath);

        //If directory doesn't exist; create
        if (!NotesDirectory.Exists) {
          NotesDirectory.Create();
        }

        // Create an empty JSON file with empty array
        File.WriteAllText(jsonPath, "[]");
        jsonData = new FileInfo(jsonPath);
      }


      //Read all text from jsonData file
      string jsonString = File.ReadAllText(jsonData.FullName);


      if (!string.IsNullOrEmpty(jsonString)) {
        //Deserialize into a Note List.
        var noteList = JsonSerializer.Deserialize<List<NoteDTO>>(jsonString);

        //If not null, return; else, return an empty new list
        return noteList ?? new List<NoteDTO>();

      } else {
        return new List<NoteDTO>();

      }
    }

    //Used to serialize the JSON data
    public void saveNotesData(ObservableCollection<Note> notes) {

      string filePath = (jsonData != null && jsonData.Exists)
          ? jsonData.FullName
          : Path.Combine(dataDirectory, "data.json");

      //Ensure directory exists before writing
      string? directoryPath = Path.GetDirectoryName(filePath);

      //If it doesn't exist, then create the directory
      if (!string.IsNullOrEmpty(directoryPath) && !Directory.Exists(directoryPath)) {
        Directory.CreateDirectory(directoryPath);
      }

      //Convert the notes list to a list of DTOs
      List<NoteDTO> noteDTOs = notes.Select(note => Note.toNoteDTO(note)).ToList();

      //Serialize notes to JSON (remove redundant .ToList() call)
      string jsonString = JsonSerializer.Serialize(noteDTOs, new JsonSerializerOptions { WriteIndented = true });

      File.WriteAllText(filePath, jsonString);

      //Update jsonData
      jsonData = new FileInfo(filePath);
    }
  }
}
