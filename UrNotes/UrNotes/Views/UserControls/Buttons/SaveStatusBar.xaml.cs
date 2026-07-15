using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

namespace UrNotes.Views.UserControls.Buttons {
  public sealed partial class SaveStatusBar : UserControl {
    //Fired when the user clicks the manual save button
    public event RoutedEventHandler? SaveRequested;

    public SaveStatusBar() {
      this.InitializeComponent();
    }

    private void SaveButton_Click(object sender, RoutedEventArgs e) {
      //The owner of this control decides how the note actually gets saved
      SaveRequested?.Invoke(this, e);
    }

    //There are pending changes on their way to being saved
    public void ShowSaving() {
      StatusText.Text = "Auto-Saving...";
      SavingSpinner.Visibility = Visibility.Visible;
      SavingSpinner.IsActive = true;
      SaveButton.IsEnabled = true;
    }

    //Everything is persisted, so the manual save button gets disabled
    public void ShowSaved() {
      StatusText.Text = "Saved";
      SavingSpinner.IsActive = false;
      SavingSpinner.Visibility = Visibility.Collapsed;
      SaveButton.IsEnabled = false;
    }

    //The note has changes that won't be auto-saved (new notes), user has to save manually
    public void ShowNotSaved() {
      StatusText.Text = "Not saved";
      SavingSpinner.IsActive = false;
      SavingSpinner.Visibility = Visibility.Collapsed;
      SaveButton.IsEnabled = true;
    }
  }
}
