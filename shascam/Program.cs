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
        //DataBaseManager.Test();
        DataBaseManager.LoadFingerprintsIntoMemory(); //this is very important, do not delete

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


        //FillDB(); //should take 30 minutes to run
        //BatchFillDB(); //should take 5 minutes to run

        int i = IdentifySong("./1006.WAV");
        Console.WriteLine(i);
        string str = DataBaseManager.GetSongNameFromID(i);
        Console.WriteLine(str);
        
        

        //WindowPartitioning(samples);

    }

    private static void FillDB()
    {
        string pathy = "./White Girl Music";
        (float[][], int[]) outp = shascam.FileHandler.LoadFolderForDB(pathy);
        float[][] samples = outp.Item1;
        int[] songID = outp.Item2;
        //Array.ForEach(samples, Console.WriteLine); // https://www.reddit.com/r/csharp/comments/11vb5fq/the_kool_kidz_way_of_printing_an_array/
        List<long> hashes;
        for (int i = 0; i < samples.Length; i++)
        {
            hashes = WindowPartitioning(samples[i], out List<int> offsets);

            for (int j = 0; j < hashes.Count; j++)
            {
                DataBaseManager.addHash(hashes[j], j * 50, songID[i]); // vi har afmålt at vinduerne vare i 50ms
            }
        }
    }
    private static void BatchFillDB()
    {
        string pathy = "./White Girl Music";
        (float[][], int[]) outp = shascam.FileHandler.LoadFolderForDB(pathy);
        float[][] samples = outp.Item1;
        int[] songID = outp.Item2;
        List<(long hash, int offset, int songID)> batch = new List<(long, int, int)>();
        int batchSize = 5000; // random number, change if you want

        for (int i = 0; i < samples.Length; i++)
        {
            List<int> offsets;
            var hashes = WindowPartitioning(samples[i], out offsets);
            Console.WriteLine("WindowPartitioning: " + i);
            for (int j = 0; j < hashes.Count; j++)
            {
                batch.Add((hashes[j], offsets[j], songID[i]));

                if (batch.Count >= batchSize)
                {
                    DataBaseManager.AddHashesBatch(batch);
                    batch.Clear();
                }
            }
        }

        // Insert any remaining hashes
        if (batch.Count > 0)
        DataBaseManager.AddHashesBatch(batch);
    }

    private static List<long> WindowPartitioning(float[] samples, out List<int> offsets)
    {
        offsets = new List<int>();
        
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
        return hashes;
    }
    public static int IdentifySong(string path)
    {
        float[] samples = shascam.FileHandler.SampleWav(path);
        List<long> hashes = WindowPartitioning(samples, out List<int> offsets);

        var votes = new Dictionary<(int songID, int delta), int>(); //the hash is stored as a long

        for (int i = 0; i < hashes.Count; i++)
        {
            long currentHash = hashes[i];
            int currentOffset = i * 50;
            //Console.WriteLine(" Offset: " + currentOffset);

            var matches = DataBaseManager.lookupHash(currentHash); //you have to load the db into RAM for this function to work btw

            foreach (var (songID, dbOffset) in matches)
            {
                int delta = dbOffset - currentOffset;
                var key = (songID, delta);

                if (!votes.ContainsKey(key))
                    votes[key] = 0;

                votes[key]++;
            }
        }
        int bestSongID = -1;
        int maxVotes = 0;
        Debug_the_thing(votes);

        foreach (var vote in votes)
        {
            if (vote.Value > maxVotes)
            {
                bestSongID = vote.Key.songID;
                maxVotes = vote.Value;

            }
        }
        return bestSongID;
    }

    private static void Debug_the_thing(Dictionary<(int songID, int delta), int> votes)
    {
        foreach (var kvp in votes)
        {
            var key = kvp.Key;   // (songID, delta)
            var count = kvp.Value;
            if (count >= 2)
                Console.WriteLine($"SongID: {key.songID}, Delta: {key.delta}, Votes: {count}");
        }
    }

    private static void PrintBytes(int offset, long hash)
    {
        byte[] bytes = BitConverter.GetBytes(hash);

        foreach (byte b in bytes)
        {
            Console.WriteLine((int)b);
        }
        Console.WriteLine("/offset/" + offset / 4096);
    }


}
