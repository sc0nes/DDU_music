namespace shascam.AudioProcessor;

using FFMpegCore;
using NAudio.Wave;
using FFMpegCore.Arguments;

public class AudioProcessor
{
    public void ConvertToWav(string inputPath, string outputPath) // copied directly from https://github.com/FFmpeg/FFmpeg/wiki/FFmpeg-Options
    {
        //var filters = new AudioFilterGraph();
        //filters.AddFilter("afftdn");

        var ffmpegFolder = @"ffmpeg-2025-09-08-git-45db6945e9-essentials_build\bin";
        var oldPath = Environment.GetEnvironmentVariable("PATH") ?? "";
        if (!oldPath.Split(Path.PathSeparator).Contains(ffmpegFolder, StringComparer.OrdinalIgnoreCase))
        {
            Environment.SetEnvironmentVariable("PATH", ffmpegFolder + Path.PathSeparator + oldPath);
        }
        FFMpegArguments
            .FromFileInput(inputPath)
            .OutputToFile(outputPath, true, options => options
                .WithAudioCodec("pcm_s16le")
                .WithAudioSamplingRate(48000)
                .WithCustomArgument("-af afftdn") //denoiser
                )
            .ProcessSynchronously();
            
    }

    public float[] LoadWavSamples(string wavPath)
    {
        using var reader = new AudioFileReader(wavPath);
        var samples = new float[reader.Length / sizeof(float)];
        reader.Read(samples, 0, samples.Length);
        return samples;
    }
    
}
/*
public float[] llsv(wavPath){
var reader = new AudioFileReader(string)
}

*/

