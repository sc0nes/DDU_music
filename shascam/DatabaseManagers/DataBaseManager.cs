namespace shascam.DatabaseManagers;
using System;
using System.IO;
using Microsoft.Data.Sqlite;


public class DataBaseManager
{
    public static void Test()
    {
    //    printAllDB();
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
            Console.WriteLine("Connected to DB! " + dbPath);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT name FROM sqlite_master WHERE type='table';";
           /* using var reader = cmd.ExecuteReader();
            Console.WriteLine("Tables in DB:");
            while (reader.Read())
            {
                Console.WriteLine(reader.GetString(0));
                using SqliteCommand command = new SqliteCommand();
                command.CommandText = @"INSERT INTO SongInfo (SongArtist, SongName) VALUES (@artist, @name);";
                command.Parameters.AddWithValue("@artist", "Test");
                command.Parameters.AddWithValue("@name", "Test");
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
            }*/
        } 
    }
    public static int AddSong(String name)
    {
        string scriptDir = AppContext.BaseDirectory;
        string projectRoot = Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetParent(scriptDir)!.FullName)!.FullName)!.FullName)!.FullName;
        string dbPath = Path.Combine(projectRoot, "SongDatabase.db");

        string connectionString = $"Data Source={dbPath};";
        int currentId = 0;
        
        using (var connection = new SqliteConnection(connectionString))
        {

            connection.Open();
            using SqliteCommand command = new SqliteCommand();
            command.CommandText = @"INSERT INTO SongInfo (SongArtist, SongName) VALUES (@artist, @name);";
            command.Parameters.AddWithValue("@artist", "artist unknown");
            command.Parameters.AddWithValue("@name", name);
            command.Connection = connection;
            try
            {
                int rowsAffected = command.ExecuteNonQuery();
                Console.WriteLine($"Inserted successfully! Rows affected: {rowsAffected}");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Insert failed: " + ex.Message);
                return 0; // Return 0 on failure
            }
            command.Parameters.Clear();

            command.CommandText = "SELECT MAX(SongID) FROM SongInfo;";
            command.Connection = connection;
            var result = command.ExecuteScalar();
            currentId = (result != DBNull.Value && result != null) ? Convert.ToInt32(result) : 0;
            Console.WriteLine($"Last inserted ID: {currentId}");
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
        string projectRoot = Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetParent(scriptDir)!.FullName)!.FullName)!.FullName)!.FullName;
        string dbPath = Path.Combine(projectRoot, "SongDatabase.db");
        string connectionString = $"Data Source={dbPath};";
        
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            
            // First, let's check what tables exist
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = "SELECT name FROM sqlite_master WHERE type='table';";
                using var reader = cmd.ExecuteReader();
                Console.WriteLine("Tables in database:");
                while (reader.Read())
                {
                    Console.WriteLine($"Table: {reader.GetString(0)}");
                }
            }

            // Now let's check the structure of the SongInfo table
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = "PRAGMA table_info(SongInfo);";
                using var reader = cmd.ExecuteReader();
                Console.WriteLine("\nColumns in SongInfo table:");
                while (reader.Read())
                {
                    Console.WriteLine($"Column: {reader["name"]}, Type: {reader["type"]}");
                }
            }

            // Now try to read the data with the correct column names
            using SqliteCommand command = new SqliteCommand();
            command.CommandText = "SELECT * FROM SongInfo;";
            command.Connection = connection;
            using var reader2 = command.ExecuteReader();
            
            // Get the column names first
            var columnNames = new List<string>();
            for (int i = 0; i < reader2.FieldCount; i++)
            {
                columnNames.Add(reader2.GetName(i));
            }
            Console.WriteLine($"\nActual columns in result: {string.Join(", ", columnNames)}");

            // Now read the data using ordinal positions instead of names
            reader2.Close();
            using var reader3 = command.ExecuteReader();
            while (reader3.Read())
            {
                for (int i = 0; i < reader3.FieldCount; i++)
                {
                    Console.Write($"{columnNames[i]}: {reader3[i]} ");
                }
                Console.WriteLine();
            }
        }
    }

}