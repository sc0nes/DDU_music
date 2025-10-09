namespace shascam.Algorithm;

using MathNet.Numerics.IntegralTransforms;
using System;

public class Algorithm
{
    
    public static readonly int upperLimit = 300;
    
    //40-80, 80-120, 120-180, 180-300.
    public static readonly int[] RANGE = new int[] { 40, 80, 120, 180, upperLimit };

    public static long shascam(double[] FFTresults)
    {

        //indexes the highest magnitudes at different frequencies.

        (double, int) bassNotes = FingerprintLoudestFreq(FFTresults, RANGE[0], RANGE[1]);
        (double, int) midNotes1 = FingerprintLoudestFreq(FFTresults, RANGE[1], RANGE[2]);
        (double, int) midNotes2 = FingerprintLoudestFreq(FFTresults, RANGE[2], RANGE[3]);
        (double, int) highNotes = FingerprintLoudestFreq(FFTresults, RANGE[3], RANGE[4]);



        string path = "log.txt";
        //WriteLog(bassNotes, midNotes1, midNotes2, highNotes, path);

        string line = $"{bassNotes.Item2}\t{midNotes1.Item2}\t{midNotes2.Item2}\t{highNotes.Item2}";
        return hash(line);
        //return results;
    }

    private static void WriteLog((double, int) bassNotes, (double, int) midNotes1, (double, int) midNotes2, (double, int) highNotes, string path)
    {
        File.AppendAllText(path, "Start\n");
        File.AppendAllText(path,
        $"Bass Notes: {string.Join(", ", bassNotes.Item1)} \n |//| {string.Join(", ", bassNotes.Item2)} \n");
        File.AppendAllText(path,
        $"Mid Notes 1: {string.Join(", ", midNotes1.Item1)} \n |//| {string.Join(", ", midNotes1.Item2)} \n");
        File.AppendAllText(path,
        $"Mid Notes 2: {string.Join(", ", midNotes2.Item1)} \n |//| {string.Join(", ", midNotes2.Item2)} \n");
        File.AppendAllText(path,
        $"High Notes: {string.Join(", ", highNotes.Item1)} \n |//| {string.Join(", ", highNotes.Item2)} \n");
        File.AppendAllText(path, "End\n");
    }

    private static readonly int FUZ_FACTOR = 12;
    private static long hash(String line) { // found from the java implementation
        String[] p = line.Split("\t");
        long p1 = long.Parse(p[0]);
        long p2 = long.Parse(p[1]);
        long p3 = long.Parse(p[2]);
        long p4 = long.Parse(p[3]);
        return  (p4 - (p4 % FUZ_FACTOR)) * (1L<<48)  //2^48
         + (p3 - (p3 % FUZ_FACTOR)) * (1L<<32)       //2^32
          + (p2 - (p2 % FUZ_FACTOR)) * (1L<<16)      //2^16	
           + (p1 - (p1 % FUZ_FACTOR));              //2^0
    }


    

    private static (double, int) FingerprintLoudestFreq(double[] FFTresults, int low_limit, int high_limit)
    {
        double highMag = double.NegativeInfinity;
        int highMagIndex = low_limit;


        for (int freq = low_limit; freq < high_limit; freq++)
        {
            double mag = Math.Log(Math.Abs(FFTresults[freq]) + 1);
            int index = GetIndex(freq);

            if (mag > highMag)
            {
                highMag = mag;
                highMagIndex = freq;
            }
        }


        return (highMag, highMagIndex);
    }
    private static int GetIndex(int freq)
    {
        int i = 0;
        while(RANGE[i] < freq) i++;
        return i;
    }
}
