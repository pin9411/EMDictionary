using EMDictionary.Models;
using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace EMDictionary.Serivces
{
  class DatabaseService
  {

    const string dictionaryTable = "dictionary";
    const string dictionaryWord = "word";
    const int dictionaryLimit = 50;

    private readonly SQLiteConnection connection;

    public DatabaseService(DatabaseHelper databaseHelper)
    {
      connection = databaseHelper.Connection;
    }

    public List<Dictionary> searchDictionaries(string word)
    {
      List<Dictionary> result = new List<Dictionary>();

      string command = $"SELECT * FROM {dictionaryTable} " +
          $"WHERE {dictionaryWord} " +
          $"LIKE '{word}%' " +
          $"ORDER BY LENGTH({dictionaryWord}) " +
          $"LIMIT {dictionaryLimit} ";

      var sqlCommand = new SQLiteCommand(command, connection);
      SQLiteDataReader dataReader = sqlCommand.ExecuteReader();

      while (dataReader.Read())
      {
        result.Add(new Dictionary()
        {
          Word = dataReader.GetString(1) ?? "",
          EngDefinition = dataReader.GetString(2) ?? "",
          MymDefinition = dataReader.GetString(3) ?? "",
        });
      }
      return result;
    }
  }
}

class DatabaseHelper
{
  private static DatabaseHelper instance = null;
  private static readonly object padlock = new object();

  public SQLiteConnection Connection { get; set; }

  public SQLiteConnection GetConnection()
  {
    return Connection;
  }

  private DatabaseHelper()
  {
    Connection = new SQLiteConnection(@"Data Source=.\Resources\Database\data.db");

    try
    {
      Connection.Open();
    }
    catch (Exception e)
    {
      Console.WriteLine(e);
    }
  }


  public static DatabaseHelper Instance
  {
    get
    {
      lock (padlock)
      {
        if (instance == null)
          instance = new DatabaseHelper();
      }
      return instance;
    }
  }

}
