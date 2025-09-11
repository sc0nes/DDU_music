// See https://aka.ms/new-console-template for more information
using System.Runtime.CompilerServices;
using FFMpegCore.Arguments;
using NAudio.Wave;
using shascam.AudioProcessor;
using System.IO;
using System;

//Console.WriteLine("Hello, ");

class Programio
{
    //private static bool running = true;
    private static MemoryStream recordedStream = new MemoryStream();
    static void Main(string[] args)
    {
        Console.WriteLine("Hello, Sha-scammers!");

        // generelle kodestruktur her følger https://www.royvanrijn.com/blog/2010/06/creating-shazam-in-java/
        string filePath;
        bool flowControl = InputfileHandler(out filePath);
        if (!flowControl)
        {
            return;
        }

        var reader = new AudioFileReader(filePath);
        int totalSamples = (int)(reader.Length / (reader.WaveFormat.BitsPerSample / 8));

        WaveFormat format = GetFormat();
        Console.WriteLine($"Sample rate: {reader.WaveFormat.SampleRate}, " +
                            $"Channels: {reader.WaveFormat.Channels}, " +
                            $"BitsPerSample: {reader.WaveFormat.BitsPerSample}");

        var waveIn = new WaveInEvent { WaveFormat = format, BufferMilliseconds = 100 };
        waveIn.StartRecording();

        var outStream = new MemoryStream();
        waveIn.DataAvailable += DataAvailable;


    }

    private static bool InputfileHandler(out string filePath)
    {
        filePath = "test.wav";
        if (!File.Exists(filePath))
        {
            filePath = "test.mp3";
            if (!File.Exists(filePath)) return false;
            var ap = new AudioProcessor();
            string wavPath = filePath + ".wav";
            ap.ConvertToWav(filePath, wavPath);
            Console.WriteLine("File not found: " + filePath);
        }
        if (!File.Exists(filePath))
        {
            filePath = "test.wav";
            Console.WriteLine("File not found: " + filePath);
            return false;
        }
        Console.WriteLine($"File: {filePath}");
        
        return true;
    }

    static WaveFormat GetFormat()
    {
    int sampleRate = 44100;
    int sampleSizeInBits = 16;
    int channels = 1;//tk increase this to 2, and make it work with stereo
    return new WaveFormat(sampleRate, sampleSizeInBits, channels);
    }

    static void  DataAvailable(object sender, WaveInEventArgs e) // this function was taken from https://markheath.net/post/how-to-record-and-play-audio-at-same
    {
        recordedStream.Write(e.Buffer, 0, e.BytesRecorded);
    }

}
