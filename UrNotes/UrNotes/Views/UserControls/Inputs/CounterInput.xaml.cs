using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

namespace UrNotes.Views.UserControls.Inputs {
  public sealed partial class CounterInput : UserControl {
    //Fired every time the counter value changes, carries the new value
    public event EventHandler<int>? ValueChanged;

    //Range the counter value is allowed to move in
    private const int MinValue = 1;
    private const int MaxValue = 128;

    private int value = 12;

    public int Value {
      get => value;
      set {
        //Keep the value inside the allowed range
        this.value = Math.Clamp(value, MinValue, MaxValue);
        ValueText.Text = this.value.ToString();
      }
    }

    public CounterInput() {
      this.InitializeComponent();
      ValueText.Text = value.ToString();
    }

    private void DecreaseButton_Click(object sender, RoutedEventArgs e) {
      Value = Value - 1;
      ValueChanged?.Invoke(this, Value);
    }

    private void IncreaseButton_Click(object sender, RoutedEventArgs e) {
      Value = Value + 1;
      ValueChanged?.Invoke(this, Value);
    }
  }
}
