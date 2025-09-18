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
        bool flowControl = shascam.FileHandler.FindCorrectPath(out filePath);
        if (!flowControl)
        {
            return;
        }

        //sampling
        float[] samples = shascam.FileHandler.LoadWav(filePath);
        //Array.ForEach(samples, Console.WriteLine); // https://www.reddit.com/r/csharp/comments/11vb5fq/the_kool_kidz_way_of_printing_an_array/

        int windowSize = 2048;

        for (int offset = 0; offset < samples.Length - windowSize; offset += windowSize) // probably buggy
        {
            double[] window = new double[windowSize];
            var complex = new System.Numerics.Complex[windowSize];
            for (int i = 0; i < windowSize; i++)
            {
                window[i] = samples[offset + i];git add SongDatabase.db
                complex[i] = new System.Numerics.Complex(window[i], 0);
            }
            Fourier.Forward(complex, FourierOptions.Matlab);
        
            float[] ampl = new float[windowSize / 2]; // half become imaginary, we only want the reals
            for (int i = 0; i < windowSize / 2; i++)
            {
                ampl[i] = (float)complex[i].Magnitude;
            }
            Array.ForEach(ampl, Console.WriteLine);

            Algorithm.shascam(ampl);

            //Algorithm.shascam(magnitudes);



            }

    }

    

}
