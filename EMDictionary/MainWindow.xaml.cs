using EMDictionary.Models;
using EMDictionary.Serivces;
using System;
using System.Collections.Generic;
using System.Speech.Synthesis;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace EMDictionary
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    private readonly DatabaseService databaseService;
    private Timer timer;
    private List<Dictionary> dictionaries;

    private readonly SpeechSynthesizer speechSynthesizer = new SpeechSynthesizer();

    public MainWindow()
    {
      InitializeComponent();

      databaseService = new DatabaseService(DatabaseHelper.Instance);
    }

    private void OnTimerDone(Object source, ElapsedEventArgs e)
    {
      this.Dispatcher.Invoke((Action)(() =>
      {
        dictionaries = databaseService.SearchDictionaries(textBoxSearch.Text);
        if (textBoxSearch.Text.Length == 0 || dictionaries.Count == 0) return;
        listViewWord.ItemsSource = dictionaries;
        listViewWord.SelectedItem = dictionaries[0];

      }));
      timer.Stop();
    }
    


    private void ListViewWordSelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
      if (listViewWord.SelectedItem == null) return;

      Dictionary dictionary = listViewWord.SelectedItem as Dictionary;
      textBoxMymDefinition.Text = dictionary.MymDefinition;
      textBoxEngDefinition.Text = dictionary.EngDefinition;
    }

    private void OnWindowKeyDown(object sender, KeyEventArgs e)
    {
      if (e.Key == Key.Escape)
      {
        textBoxSearch.Clear();
        textBoxSearch.Focus();
      }
      if (e.Key == Key.Enter)
      {
        if (listViewWord.SelectedItem == null) return;
        var current = speechSynthesizer.GetCurrentlySpokenPrompt();

        if (current != null)
          speechSynthesizer.SpeakAsyncCancel(current);
        speechSynthesizer.SpeakAsync(((Dictionary)listViewWord.SelectedItem).Word);
      }
    }

    private void OnTextSearchChanged(object sender, TextChangedEventArgs e)
    {
      if (timer != null)
      {
        timer.Stop();
      }
      timer = new Timer(200);
      timer.Elapsed += new ElapsedEventHandler(OnTimerDone);
      timer.Start();
    }

    private void OnTextSearchKeyUp(object sender, KeyEventArgs e)
    {
      if (e.Key == Key.Down)
      {
        listViewWord.Focus();
        listViewWord.ItemContainerGenerator.StatusChanged += new EventHandler(ItemContainerGenerator_StatusChanged);
      }
    }
    private void ItemContainerGenerator_StatusChanged(object sender, EventArgs e)
    {
      if (listViewWord.ItemContainerGenerator.Status == System.Windows.Controls.Primitives.GeneratorStatus.ContainersGenerated)
      {
        int index = listViewWord.SelectedIndex;

        if (index >= 0)
        {
          if (listViewWord.ItemContainerGenerator.ContainerFromIndex(index) is ListViewItem item)
            item.Focus();
        }
      }
    }
  }
}
