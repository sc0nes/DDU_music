// See https://aka.ms/new-console-template for more information
using System.Runtime.CompilerServices;
using FFMpegCore.Arguments;
using NAudio.Wave;
using shascam.AudioProcessor;
using System.IO;
using System;
using MathNet.Numerics.IntegralTransforms;
using NAudio.Dsp;
using shascam.Algorithm;

using System.Data.SqlTypes;
using shascam.DatabaseManagers;
using System.Diagnostics;
using System.Data.SQLite;

//Console.WriteLine("Hello, ");

class Programio
{
    //public static readonly int windowSize = 1028;
    public static readonly int windowSize = 4096;

    //private static bool running = true;
    static void Main(string[] args)
    {
        Console.WriteLine("Hello, Sha-scammers!");
        DataBaseManager.Test();
        // generelle kodestruktur her følger https://www.royvanrijn.com/blog/2010/06/creating-shazam-in-java/
        /*string filePath = "test.wav";
        
        bool flowControl = shascam.FileHandler.FindCorrectPath(out filePath, filePath);
        if (!flowControl)
        {
            return;
        }*/
        //sampling
        //Console.WriteLine("22");
        //float[] samples = shascam.FileHandler.LoadWav(filePath);
        string pathy = "./White Girl Music";
        (float[][], int[]) outp = shascam.FileHandler.LoadFolderForDB(pathy);
        float[][] samples = outp.Item1;
        int[] songID = outp.Item2;
        //Array.ForEach(samples, Console.WriteLine); // https://www.reddit.com/r/csharp/comments/11vb5fq/the_kool_kidz_way_of_printing_an_array/
        long[] hashes;
        for (int i = 0; i < samples.Length; i++)
        {
            hashes = WindowPartitioning(samples[i], out List<int> offsets);
            
            for(int j = 0; j < hashes.Length; j++)
            {
                DataBaseManager.addHash(hashes[j], j*50, songID[i]); // vi har afmålt at vinduerne vare i 50ms
            }
        }

        DataBaseManager.printAllDB();
        
        //WindowPartitioning(samples);

    }

    private static long[] WindowPartitioning(float[] samples, out List<int> offsets)
    {
        offsets = new List<int>();
        Console.Write("WindowPartitioning");
        List<long> hashes = new List<long>();
        for (int offset = 0; offset < samples.Length - windowSize; offset += windowSize) // probably buggy
        {
            offsets.Add(offset);
            double[] window = new double[windowSize];
            var complex = new System.Numerics.Complex[windowSize];
            for (int i = 0; i < windowSize; i++)
            {
                window[i] = samples[offset + i];
                complex[i] = new System.Numerics.Complex(window[i], 0);
            }
            Fourier.Forward(complex, FourierOptions.Matlab);

            double[] ampl = new double[windowSize / 2]; // half become imaginary, we only want the reals
            for (int i = 0; i < windowSize / 2; i++)
            {
                ampl[i] = complex[i].Magnitude;
            }

            long hash = Algorithm.shascam(ampl);

            hashes.Add(hash);
        }
        return hashes.ToArray();
    }

    private static void PrintBytes(int offset, long hash)
    {
        byte[] bytes = BitConverter.GetBytes(hash);

        foreach (byte b in bytes)
        {
            Console.WriteLine((int)b);
        }
        Console.WriteLine("/offset/" + offset/4096);
    }


}
