using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace UrNotes.Views.UserControls.Buttons {
  public sealed partial class CreateNoteButton : UserControl {
    public event RoutedEventHandler? CreateNoteClicked;

    public CreateNoteButton() {
      this.InitializeComponent();
      InnerButton.CreateNoteClicked += (s, e) => {
        //When the button on AutoSpacedButton.xaml is clicked, it fires its _Click function, which invokes the AutoSpacedButton.xaml.cs 
        //"CreateNoteClicked" event.
        //^ up there, by using InnerButton.CreateNoteClicked, we subscribe to it.
        //We then say, when that event happens (when the InnerButton classes Button is Clicked)
        //We invoke THIS CLASS' "CreateNoteClicked" event, down here, so that it can be subscribed to... upwards.
        CreateNoteClicked?.Invoke(this, e);
      };
    }
  }
}
