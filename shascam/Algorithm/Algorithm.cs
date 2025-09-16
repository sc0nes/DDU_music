namespace shascam.Algorithm;

using MathNet.Numerics.IntegralTransforms;
using System;

public class Algorithm
{
    //40-80, 80-120, 120-180, 180-300.
    public static readonly int upperLimit = 300;
    public static readonly int[]  RANGE = new int[]{ 40, 80, 120, 180, upperLimit+1 };
    
    

    
    public static void shascam(float[] FFTresults)
    {
        string path = "log.txt";
        File.AppendAllText(path, "Start\n");
        //indexes the highest magnitudes at different frequencies.
        (double[], int[]) bassNotes = FingerprintLoudestFreq(FFTresults, RANGE[0], RANGE[1]);
        (double[], int[]) midNotes1 = FingerprintLoudestFreq(FFTresults, RANGE[1], RANGE[2]);
        (double[], int[]) midNotes2 = FingerprintLoudestFreq(FFTresults, RANGE[2], RANGE[3]);
        (double[], int[]) highNotes = FingerprintLoudestFreq(FFTresults, RANGE[3], RANGE[4]);
        File.AppendAllText(path,
        $"Bass Notes: {string.Join(", ", bassNotes.Item1)} | {string.Join(", ", bassNotes.Item2)}");
        File.AppendAllText(path,
        $"Mid Notes 1: {string.Join(", ", midNotes1.Item1)} | {string.Join(", ", midNotes1.Item2)}");
        File.AppendAllText(path,
        $"Mid Notes 2: {string.Join(", ", midNotes2.Item1)} | {string.Join(", ", midNotes2.Item2)}");
        File.AppendAllText(path,
        $"High Notes: {string.Join(", ", highNotes.Item1)} | {string.Join(", ", highNotes.Item2)}");
        File.AppendAllText(path, "End\n");


        //return results;
    }

    private static (double[], int[]) FingerprintLoudestFreq(float[] FFTresults, int low_limit, int high_limit)
    {
        double[] highscores = new double[high_limit - low_limit];
        int[] recordPoints = new int[high_limit - low_limit];

        for (int freq = low_limit; freq < high_limit - 1; freq++)
        {
            //Get the magnitude:
            double mag = Math.Log(Math.Abs(FFTresults[freq]) + 1);

            //Find out which range we are in:
            int index = GetIndex(freq);

            //Save the highest magnitude and corresponding frequency:
            if (mag > highscores[index])
            {
                highscores[index] = mag;
                recordPoints[index] = freq;
            }
        }
        return (highscores, recordPoints);
    }
    private static int GetIndex(int freq)
    {
        int i = 0;
        while(RANGE[i] < freq) i++;
        return i;
    }
}
