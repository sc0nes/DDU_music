namespace shascam;

using FFMpegCore;
using NAudio;
using NAudio.Wave;
using shascam.AudioProcessor;

public class FileHandler
{
    public static bool FindCorrectPath(out string filePath)
    {
        //filePath = "test";
        filePath = "test2.wav";
        if (!File.Exists(filePath))
        {
            filePath = "test2.mp3";
            if (!File.Exists(filePath)) return false;
            var ap = new AudioProcessor.AudioProcessor();
            string wavPath = filePath + ".wav";
            ap.ConvertToWav(filePath, wavPath);
            Console.WriteLine("File not found: " + filePath);
        }
        if (!File.Exists(filePath))
        {
            filePath = "test2.wav";
            Console.WriteLine("File not found: " + filePath);
            return false;
        }
        Console.WriteLine($"File: {filePath}");
        
        return true;
    }
    public static float[] LoadWav(string path)
    {
        using var reader = new AudioFileReader(path);
        var samples = new float[reader.Length / sizeof(float)];
        int read = reader.Read(samples, 0, samples.Length);
        Array.Resize(ref samples, read);


        WaveFormat format = GetFormat();
        Console.WriteLine($"Sample rate: {reader.WaveFormat.SampleRate}, " +
                            $"Channels: {reader.WaveFormat.Channels}, " +
                            $"BitsPerSample: {reader.WaveFormat.BitsPerSample}");

        return samples;
    }

    public static WaveFormat GetFormat()
    {
        int sampleRate = 44100;
        int sampleSizeInBits = 16;
        int channels = 1;//tk increase this to 2, and make it work with stereo
        return new WaveFormat(sampleRate, sampleSizeInBits, channels);
    }
}
