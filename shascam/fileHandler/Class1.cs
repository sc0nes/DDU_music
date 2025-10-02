namespace shascam;

using System.Diagnostics;
using FFMpegCore;
using NAudio;
using NAudio.Wave;
using shascam.AudioProcessor;

public class FileHandler
{
    //public static int sampleRate = int.MinValue;
    public static (float[][], int[]) LoadFolderForDB(String Path)
    {
        List<int> songID = new List<int>();
        var files = Directory.EnumerateFiles(Path, "*.mp3").ToList();
        
        Console.WriteLine($"Found {files.Count} MP3 files in: {Path}");
        
        // Debug: List all found files
        foreach (var file in files)
        {
            Console.WriteLine($"Found MP3: {file}");
        }
        
        var samples = new List<float[]>();
        
        foreach (var file in files)
        {
            string wavPath = file + ".wav";
            Console.WriteLine($"Processing: {file}");
            Console.WriteLine($"WAV path would be: {wavPath}");
            Console.WriteLine($"WAV file exists: {File.Exists(wavPath)}");
            
            if (!File.Exists(wavPath))
            {
                Console.WriteLine("=== WAV DOES NOT EXIST - CONVERTING ===");
                Console.WriteLine("Tried to add song: " + wavPath);
                
                try
                {
                    var ap = new AudioProcessor.AudioProcessor();
                    ap.ConvertToWav(file, wavPath);
                    Console.WriteLine($"Conversion completed for: {wavPath}");
                    
                    // Check if WAV was actually created
                    if (File.Exists(wavPath))
                    {
                        Console.WriteLine("WAV file successfully created!");
                        samples.Add(LoadWav(wavPath));
                        songID.Add(DatabaseManagers.DataBaseManager.AddSong(wavPath)); 
                    }
                    else
                    {
                        Console.WriteLine("ERROR: WAV file was not created!");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"ERROR during conversion: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("=== WAV ALREADY EXISTS - SKIPPING CONVERSION ===");
                // TODO: You still need to add the song to database here!
                samples.Add(LoadWav(wavPath));
                
                // Add this line to handle existing WAV files:
                songID.Add(DatabaseManagers.DataBaseManager.AddSong(wavPath));
            }
            
            Console.WriteLine("---"); // Separator for readability
        }
        
        Console.WriteLine($"Final result: {samples.Count} samples, {songID.Count} song IDs");
        return (samples.ToArray(), songID.ToArray());
    }
    
    public static bool FindCorrectPath(out string filePath, string path)
    {
        //filePath = "test";
        //filePath = filePath + ".wav";
        filePath = path + ".wav";
        if (!File.Exists(filePath))
        {
            filePath = path + ".mp3";
            if (!File.Exists(filePath)) return false;
            var ap = new AudioProcessor.AudioProcessor();
            string wavPath = filePath + ".wav";
            ap.ConvertToWav(filePath, wavPath);
            Console.WriteLine("File not found: " + filePath);
        }
        if (!File.Exists(filePath))
        {
            filePath = path + ".wav";
            Console.WriteLine("File not found: " + filePath);
            return false;
        }
        Console.WriteLine($"File: {filePath}");

        return true;
    }
    
    public static float[] LoadWav(string path)
    {
        var reader = new AudioFileReader(path);
        var samples = new float[reader.Length / sizeof(float)];
        int read = reader.Read(samples, 0, samples.Length);
        Array.Resize(ref samples, read);


        WaveFormat format = GetFormat();
        Console.WriteLine($"Sample rate: {reader.WaveFormat.SampleRate}, " +
                            $"Channels: {reader.WaveFormat.Channels}, " +
                            $"BitsPerSample: {reader.WaveFormat.BitsPerSample}, " +
                            $"Duration: {reader.TotalTime}, " +
                            $"Volume: {reader.Volume}" );
        return samples;
    }

    public static WaveFormat GetFormat()
    {
        int sampleRate = 44100;
        int sampleSizeInBits = 16;
        int channels = 1;//maybe increase this to 2, and make it work with stereo
        return new WaveFormat(sampleRate, sampleSizeInBits, channels);
    }
}
