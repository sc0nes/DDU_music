namespace shascam.AudioProcessor;

using FFMpegCore;
using NAudio.Wave;

public class AudioProcessor
{
    public void ConvertToWav(string inputPath, string outputPath) // copied directly from https://github.com/FFmpeg/FFmpeg/wiki/FFmpeg-Options
    {
        var ffmpegFolder = @"ffmpeg-2025-09-08-git-45db6945e9-essentials_build\bin";
        var oldPath = Environment.GetEnvironmentVariable("PATH") ?? "";
        if (!oldPath.Split(Path.PathSeparator).Contains(ffmpegFolder, StringComparer.OrdinalIgnoreCase))
        {
            Environment.SetEnvironmentVariable("PATH", ffmpegFolder + Path.PathSeparator + oldPath);
        }
        // Use FFMpeg to convert mp3 to wav
        FFMpegArguments
            .FromFileInput(inputPath)
            .OutputToFile(outputPath, true, options => options
                .WithAudioCodec("pcm_s16le")
                .WithAudioSamplingRate(44100))
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

