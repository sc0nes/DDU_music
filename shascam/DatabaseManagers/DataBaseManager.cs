namespace shascam.DatabaseManagers;
using System;
using System.IO;
using Microsoft.Data.Sqlite;


public class DataBaseManager
{
    public static void Test()
    {
        // Get the folder where the compiled program is running
        string scriptDir = AppContext.BaseDirectory;

// Go up three levels to reach the project folder
        string projectRoot = Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetParent(scriptDir)!.FullName)!.FullName)!.FullName)!.FullName;

// Path to the database in the project folder
        string dbPath = Path.Combine(projectRoot, "SongDatabase.db");

        Console.WriteLine("DB Path: " + dbPath + " Test"); 
// Should now print: C:\Users\carl-\RiderProjects\DDU_music\shascam\SongDatabase.db

// Connection string
        string connectionString = $"Data Source={dbPath};";

        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            Console.WriteLine("Connected to DB!");
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT name FROM sqlite_master WHERE type='table';";
            using var reader = cmd.ExecuteReader();
            Console.WriteLine("Tables in DB:");
            while (reader.Read())
            {
                Console.WriteLine(reader.GetString(0));
            }
        }
    }
    public static int addSong(String name)
    {
        // Get the folder where the compiled program is running
        string scriptDir = AppContext.BaseDirectory;

// Go up three levels to reach the project folder
        string projectRoot = Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetParent(scriptDir)!.FullName)!.FullName)!.FullName)!.FullName;

// Path to the database in the project folder
        string dbPath = Path.Combine(projectRoot, "SongDatabase.db");

// Should now print: C:\Users\carl-\RiderProjects\DDU_music\shascam\SongDatabase.db

// Connection string
        string connectionString = $"Data Source={dbPath};";
        int currentId = 0;
        
        using (var connection = new SqliteConnection(connectionString))
        {

            connection.Open();
            using var fkCmd = connection.CreateCommand();
            fkCmd.CommandText = "PRAGMA foreign_keys = ON;";
            fkCmd.ExecuteNonQuery();
            
            using SqliteCommand command = new SqliteCommand();
            command.CommandText = @"INSERT INTO SongInfo (SongArtist, SongName) VALUES (@artist, @name);";
            command.Parameters.AddWithValue("@artist", "artist unknown");
            command.Parameters.AddWithValue("@name", name);
            command.Connection = connection;

            try
            {
                command.ExecuteNonQuery();
                Console.WriteLine("Inserted succesfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Insert failed: " + ex.Message);
            }
            command.Parameters.Clear();


            command.CommandText = "SELECT MAX(song_id) FROM SongInfo;";
            command.Connection = connection;
            var result = command.ExecuteScalar();
            currentId = (result != DBNull.Value && result != null) ? Convert.ToInt32(result) : 0;//lao, yk that i know the ? operator. it was easier than writing a whole ass if statement 
            command.Parameters.Clear();
        }

        return currentId;

    }
    
    
    public static void addHash(long hash, int offset, int ID)
    {
        // Get the folder where the compiled program is running
        string scriptDir = AppContext.BaseDirectory;

// Go up three levels to reach the project folder
        string projectRoot = Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetParent(scriptDir)!.FullName)!.FullName)!.FullName)!.FullName;

// Path to the database in the project folder
        string dbPath = Path.Combine(projectRoot, "SongDatabase.db");
        
// Should now print: C:\Users\carl-\RiderProjects\DDU_music\shascam\SongDatabase.db

// Connection string
        string connectionString = $"Data Source={dbPath};";
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            using var fkCmd = connection.CreateCommand();
            fkCmd.CommandText = "PRAGMA foreign_keys = ON;";
            fkCmd.ExecuteNonQuery();

            
            using SqliteCommand command = new SqliteCommand();
            command.CommandText = @"INSERT INTO fingerprints (song_id, time_offset, songData) VALUES (@ID, @offset, @hash);";
            command.Parameters.AddWithValue("@ID", ID);
            command.Parameters.AddWithValue("@offset", offset);
            command.Parameters.AddWithValue("@hash", hash);
            command.Connection = connection;
            try
            {
                command.ExecuteNonQuery();
                Console.WriteLine("Inserted succesfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Insert failed: " + ex.Message);
            }
            command.Parameters.Clear();

        }

    }

    public static void printAllDB()
    {
        // Get the folder where the compiled program is running
        string scriptDir = AppContext.BaseDirectory;

// Go up three levels to reach the project folder
        string projectRoot = Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetParent(scriptDir)!.FullName)!.FullName)!.FullName)!.FullName;

// Path to the database in the project folder
        string dbPath = Path.Combine(projectRoot, "SongDatabase.db");

// Should now print: C:\Users\carl-\RiderProjects\DDU_music\shascam\SongDatabase.db

// Connection string
        string connectionString = $"Data Source={dbPath};";
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            using SqliteCommand command = new SqliteCommand();
            command.CommandText = "SELECT * FROM SongInfo;";
            command.Connection = connection;
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                Console.WriteLine($"{reader["song_id"]} {reader["SongArtist"]} {reader["SongName"]}");
            }
        }
    }

}