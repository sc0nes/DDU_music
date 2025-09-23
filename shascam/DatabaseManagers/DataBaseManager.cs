namespace shascam.DatabaseManagers;
using System;
using System.IO;
using Microsoft.Data.Sqlite;


public class DataBaseManager
{
    public static void Test()
    {
        // Get the folder of the current script
        string scriptDir = AppDomain.CurrentDomain.BaseDirectory;

        // Go up one folder to the project root (where the DB is)
        string projectRoot = Path.GetFullPath(Path.Combine(scriptDir, @".."));

        // Path to the database
        string dbPath = Path.Combine(projectRoot, "SongDatabase.db");
        string connectionString = $"Data Source={dbPath};";

        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            Console.WriteLine("Connected to DB!");
        }
    }

}