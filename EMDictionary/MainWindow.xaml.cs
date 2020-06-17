using EMDictionary.Models;
using EMDictionary.Serivces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
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

    public MainWindow()
    {
      InitializeComponent();

      databaseService = new DatabaseService(DatabaseHelper.Instance);
    }

    private void OnTimerDone(Object source, ElapsedEventArgs e)
    {

      this.Dispatcher.Invoke((Action)(() =>
      {
        dictionaries = databaseService.searchDictionaries(textBoxSearch.Text);
        if (textBoxSearch.Text.Length == 0) return;
        listViewWord.ItemsSource = dictionaries; //.Select(m => m.Word).ToList();
        //if (dictionaries.Count > 0 && listViewWord.IsLoaded)
        //{
        //  listViewWord.SelectedItem = 1;
        //  ((ListBoxItem)listViewWord.SelectedItem)?.Focus();
        //}
      }));
      timer.Stop();
    }

    private void listViewWordSelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
      if (listViewWord.SelectedItem == null) return;

      Dictionary dictionary = listViewWord.SelectedItem as Dictionary;
      textBoxMymDefinition.Text = dictionary.MymDefinition;
      textBoxEngDefinition.Text = dictionary.EngDefinition;
    }

    private void onWindowKeyDown(object sender, KeyEventArgs e)
    {
      if (e.Key == Key.Escape)
      {
        textBoxSearch.Clear();
        textBoxSearch.Focus();
      }
      if (e.Key == Key.F1)
      {
        Console.WriteLine(listViewWord.SelectedItem);
      }
    }

    private void OnTextSearchKeyDown(object sender, KeyEventArgs e)
    {
      if (e.Key == Key.Enter)
      {
        listViewWord.Focus();
      }
    }

    private void OnTextSearchChanged(object sender, TextChangedEventArgs e)
    {
      if (timer != null)
      {
        timer.Stop();
      }
      timer = new Timer(500);
      timer.Elapsed += new ElapsedEventHandler(OnTimerDone);
      timer.Start();
    }
  }
}
