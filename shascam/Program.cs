// See https://aka.ms/new-console-template for more information
using System.Runtime.CompilerServices;
using FFMpegCore.Arguments;
using NAudio.Wave;
using shascam.AudioProcessor;
using System.IO;
using shascam.Algorithm;
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
        bool flowControl = FindCorrectPath(out filePath);
        if (!flowControl)
        {
            return;
        }

        //sampling
        float[] samples = LoadWav(filePath);
        //Array.ForEach(samples, Console.WriteLine);

        int windowSize = 2048;

        for (int offset = 0; offset < samples.Length; offset += windowSize) // probably buggy
        {
            double[] window = new double[windowSize];
            for (int i = 0; i < windowSize; i++)
            {
                window[i] = samples[offset + i];
            }
            Array.ForEach(window, Console.WriteLine);

            
            float[] magnitudes; // = some fft stuff



            //Algorithm.shascam(magnitudes);



        }

    }

    private static bool FindCorrectPath(out string filePath)
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
    static float[] LoadWav(string path)
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
