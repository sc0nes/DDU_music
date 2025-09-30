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
    public static int addSong(String name)
    {
        // Get the folder of the current script
        string scriptDir = AppDomain.CurrentDomain.BaseDirectory;

        // Go up one folder to the project root (where the DB is)
        string projectRoot = Path.GetFullPath(Path.Combine(scriptDir, @".."));

        // Path to the database
        string dbPath = Path.Combine(projectRoot, "SongDatabase.db");
        string connectionString = $"Data Source={dbPath};";
        int currentId = 0;
        using (var connection = new SqliteConnection(connectionString))
        {

            connection.Open();
            Console.WriteLine("Connected to DB!");

            using SqliteCommand command = new SqliteCommand();
            command.CommandText = @"INSERT INTO SongInfo (SongArtist, SongName) VALUES (@artist, @name);";
            command.Parameters.AddWithValue("@artist", "artist unknown");
            command.Parameters.AddWithValue("@name", name);
            command.Connection = connection;
            command.ExecuteNonQuery();
            command.Parameters.Clear();


            command.CommandText = "SELECT MAX(song_id) FROM SongInfo;";
            command.Connection = connection;
            currentId = DBNull.Value != null ? Convert.ToInt32(command.ExecuteScalar()) : 0;//lao, yk that i know the ? operator. it was easier than writing a whole ass if statement 
            command.Parameters.Clear();
        }

        return currentId;

    }
    public static void addHash(long hash, int offset, int ID)
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
            using SqliteCommand command = new SqliteCommand();
            command.CommandText = @"INSERT INTO SongInfo (song_id, time_offset, SongData) VALUES (@ID, @offset, @hash);";
            command.Parameters.AddWithValue("@ID", ID);
            command.Parameters.AddWithValue("@offset", offset);
            command.Parameters.AddWithValue("@hash", hash);
            command.Connection = connection;
            command.ExecuteNonQuery();

        }

    }

    public static void printAllDB()
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
            using SqliteCommand command = new SqliteCommand();
            command.CommandText = "SELECT * FROM SongInfo;";
            command.Connection = connection;
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                Console.WriteLine($"{reader.GetInt32(0)} {reader.GetInt32(1)} {reader.GetInt64(2)}");
            }
        }
    }

}