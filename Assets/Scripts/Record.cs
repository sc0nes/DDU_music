using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.IO;

#if UNITY_ANDROID
using UnityEngine.Android;
#endif

public class SaveSongs : MonoBehaviour
{
    [Header("UI")] public Text statusText;   // <- træk dit tekst‑objekt ind

    public int sampleRate = 44100;
    public int maxLengthSec = 3599;

    private AudioClip micClip;
    private bool isRecording = false;

    // -----------------------------------------------------------------
    // Metoder som kan kaldes fra Unity‑Event (OnClick, EventTrigger osv.)
    // -----------------------------------------------------------------
    public void StartRecording()   // kan tilknyttes til PointerDown
    {
        if (isRecording) return;
        micClip = Microphone.Start(null, false, maxLengthSec, sampleRate);
        isRecording = true;
        SetStatus("Optager …");
        Debug.Log("[SaveSongs] Optagelse startet.");
    }

    public void StopRecording()    // kan tilknyttes til PointerUp
    {
        if (!isRecording) return;
        int endPos = Microphone.GetPosition(null);
        Microphone.End(null);
        isRecording = false;

        SetStatus("Optagelse stoppet – gemmer …");
        Debug.Log("[SaveSongs] Optagelse stoppet, gemmer.");
        SaveClip(endPos);
    }

    // -----------------------------------------------------------------
    // De originale IPointer…‑metoder – kan blot kalde ovenstående
    // -----------------------------------------------------------------
    public void OnPointerDown(PointerEventData _) => StartRecording();
    public void OnPointerUp(PointerEventData _) => StopRecording();

    // -----------------------------------------------------------------
    // Resten af scriptet er identisk med den version, du allerede har
    // -----------------------------------------------------------------
    private void SetStatus(string txt)
    {
        if (statusText != null) statusText.text = txt;
    }

    // ---------- Gemning (WavUtility skal være i projektet) ----------
    private const string fileName = "buttonRecord.wav";

    private void SaveClip(int endPos)
    {
        if (endPos <= 0) { SetStatus("Ingen data – intet gemt."); return; }

        float[] wav = new float[endPos * micClip.channels];
        micClip.GetData(wav, 0);

        AudioClip finalClip = AudioClip.Create("record", endPos,
                                              micClip.channels,
                                              micClip.frequency,
                                              false);
        finalClip.SetData(wav, 0);

        string folder = Path.Combine(Application.persistentDataPath, "Recordings");
        if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
        string path = Path.Combine(folder, fileName);

        // ⚠️ WavUtility.cs skal ligge i Assets!
        WavUtility.Save(path, finalClip);

        SetStatus($"Gemte til:\n{path}");
        Debug.Log($"[SaveSongs] Gemte til: {path}");
    }
}
