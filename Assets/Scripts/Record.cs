using System.IO;
using UnityEngine;
using UnityEngine.Android;               

public class RecordAudio : MonoBehaviour
{
    private AudioClip recordedClip;

    private string directoryPath => Path.Combine(Application.persistentDataPath, "Recordings");
    private string fileName => "recording.wav";
    private string FullPath => Path.Combine(directoryPath, fileName);


    private float startTime;
    private float recordingLength;

    private void Awake()
    {
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
            //Debug.Log($"[RecordAudio] Oprettet mappe: {directoryPath}");
        }
        RequestMicrophonePermission();
        //https://docs.unity3d.com/6000.0/Documentation/ScriptReference/Android.Permission.RequestUserPermission.html
    }

    public void RequestMicrophonePermission()
    {
        if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
        {
            Permission.RequestUserPermission(Permission.Microphone);
        }
    }

    public void StartRecording()
    {
        string device = Microphone.devices[0];
        int sampleRate = 44100;
        int maxLengthSec = 3599; // 59 min 59sek no mas else KABOOM bluescreen

        recordedClip = Microphone.Start(device, false, maxLengthSec, sampleRate);
        startTime = Time.realtimeSinceStartup;
        //Debug.Log("[RecordAudio] Optagelse påbegyndt...");
    }

    public void StopRecording()
    {
        Microphone.End(null);
        recordingLength = Time.realtimeSinceStartup - startTime;
        recordedClip = TrimClip(recordedClip, recordingLength);

        //Debug.Log($"[RecordAudio] Optagelse stoppet. Længde: {recordingLength:F2}s");
        SaveRecording();
    }

    public void SaveRecording()
    {
        // Bruger wawUtill som er en anden fil der håndtere kreationen af lyd filen
        WavUtility.Save(FullPath, recordedClip);
        Debug.Log($"[RecordAudio] Optagelse gemt: {FullPath}");
    }

    private AudioClip TrimClip(AudioClip clip, float lengthSec)
    {
        int samples = Mathf.FloorToInt(clip.frequency * lengthSec);
        float[] data = new float[samples * clip.channels];
        clip.GetData(data, 0);

        AudioClip trimmed = AudioClip.Create(clip.name, samples, clip.channels, clip.frequency, false);
        trimmed.SetData(data, 0);
        return trimmed;
    }
}
