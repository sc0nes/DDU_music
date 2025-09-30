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
        string filePath = "test.wav";
        bool flowControl = shascam.FileHandler.FindCorrectPath(out filePath, filePath);
        if (!flowControl)
        {
            return;
        }

        //sampling

        float[] samples = shascam.FileHandler.LoadWav(filePath);

        //string pathy = "./whiteGirlbangers";
        float[][] samples2 = shascam.FileHandler.LoadFilesForFolder(pathy);
        //Array.ForEach(samples, Console.WriteLine); // https://www.reddit.com/r/csharp/comments/11vb5fq/the_kool_kidz_way_of_printing_an_array/
        for(int i = 0; i < samples2.Length; i++)
        {
            WindowPartitioning(samples2[i]);
        }
        //WindowPartitioning(samples);

    }

    private static void WindowPartitioning(float[] samples)
    {
        for (int offset = 0; offset < samples.Length - windowSize; offset += windowSize) // probably buggy
        {
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
        }
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
