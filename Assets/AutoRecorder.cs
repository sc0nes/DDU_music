/*********************************************************************
 * AutoRecorderSimple – med UI‑status‑tekst
 *
 *  - Lytter, starter optagelse ved tale, stopper ved stil.
 *  - Skriver status (Lytter / Optager / Stoppet / Fil‑sti) i et UI‑Text.
 *********************************************************************/

using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;               // <‑ hvis du bruger UI‑Text
// using TMPro;                     // <‑ hvis du bruger TextMeshPro (fjern linjen ovenfor)

#if UNITY_ANDROID
using UnityEngine.Android;
#endif

public class AutoRecorderSimple : MonoBehaviour
{
    // -------------------------------------------------------------
    // 0️⃣ UI – status‑tekst (drag & drop i Inspector)
    // -------------------------------------------------------------
    [Header("UI")]
    // public TMP_Text statusText;      // <-- brug denne, hvis du bruger TextMeshPro
    public Text statusText;            // <-- brug denne, hvis du bruger UI‑Text

    // -------------------------------------------------------------
    // 1️⃣ Indstillinger
    // -------------------------------------------------------------
    [Header("Tale‑detektion")]
    [Range(0f, 1f)] public float speechThreshold  = 0.02f;   // RMS‑tærskel for start
    [Header("Stil‑detektion")]
    [Range(0f, 1f)] public float silenceThreshold = 0.015f;  // RMS‑tærskel for stop
    public float silenceTimeout   = 1.5f;   // sek. stil før stop
    public int   sampleRate       = 44100;
    public int   maxLengthSec     = 1800;  // max. varighed (30 min)

    // -------------------------------------------------------------
    // 2️⃣ Interne felter
    // -------------------------------------------------------------
    private AudioClip micClip;
    private bool      isRecording   = false;
    private float     silenceTimer  = 0f;

    // -------------------------------------------------------------
    // Unity‑livscyklus
    // -------------------------------------------------------------
    private void Start()
    {
#if UNITY_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
            Permission.RequestUserPermission(Permission.Microphone);
#endif
        // Vis at vi **lytter**, selvom vi endnu ikke optager.
        SetStatus("Lytter efter tale …");

        // Start en 1‑s loop‑buffer så vi kan lytte.
        micClip = Microphone.Start(null, true, 1, sampleRate);
        StartCoroutine(DetectAndRecord());
    }

    // -------------------------------------------------------------
    // 3️⃣ Detekter tale → start reel optagelse
    // -------------------------------------------------------------
    private IEnumerator DetectAndRecord()
    {
        const float step = 0.1f;
        int window = Mathf.CeilToInt(sampleRate * step);
        float[] buffer = new float[window];

        while (true)
        {
            int pos = Microphone.GetPosition(null);
            if (pos <= 0) { yield return null; continue; }

            // Hent de nyeste samples fra loop‑bufferen
            int start = pos - window;
            if (start < 0) start += micClip.samples;
            micClip.GetData(buffer, start);

            float rms = CalcRMS(buffer);

            // -------------------------------------------------
            // 3.1 – Tale fundet → start reel optagelse
            // -------------------------------------------------
            if (!isRecording && rms > speechThreshold)
            {
                isRecording = true;
                SetStatus("Optager …");
                // Stop den lille loop‑buffer og start en ny optagelse uden loop
                Microphone.End(null);
                micClip = Microphone.Start(null, false, maxLengthSec, sampleRate);
                Debug.Log("[AutoRecorderSimple] Talte – optagelse starter");
                continue; // fortsæt næste iteration med den nye clip
            }

            // -------------------------------------------------
            // 3.2 – Når vi er i optagelses‑mode → tjek stil
            // -------------------------------------------------
            if (isRecording)
            {
                if (rms < silenceThreshold)
                {
                    silenceTimer += step;
                    if (silenceTimer >= silenceTimeout)
                    {
                        // *** STOP & GEM ***
                        int endPos = Microphone.GetPosition(null); // før End!
                        Microphone.End(null);
                        SetStatus("Optagelse stoppet.");
                        SaveClip(endPos);
                        yield break;
                    }
                }
                else
                {
                    silenceTimer = 0f;
                }
            }

            yield return new WaitForSeconds(step);
        }
    }

    // -------------------------------------------------------------
    // 4️⃣ Gem filen (brug den position, vi har gemt før End)
    // -------------------------------------------------------------
    private void SaveClip(int endPos)
    {
        if (endPos <= 0)
        {
            SetStatus("Ingen data – intet gemt.");
            Debug.LogWarning("EndPos = 0 – intet at gemme.");
            return;
        }

        // Hent de optagede samples fra start (0) til endPos
        float[] wavData = new float[endPos * micClip.channels];
        micClip.GetData(wavData, 0);

        // Opret en ny AudioClip med præcis længde
        AudioClip finalClip = AudioClip.Create("record", endPos,
                                               micClip.channels,
                                               micClip.frequency,
                                               false);
        finalClip.SetData(wavData, 0);

        // Gem til fil (WavUtility er din egen klasse)
        string folder = Path.Combine(Application.persistentDataPath, "Recordings");
        if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
        string path = Path.Combine(folder, fileName);
        WavUtility.Save(path, finalClip);

        SetStatus($"Gemte til:\n{path}");
        Debug.Log($"[AutoRecorderSimple] Gemte til: {path}");
    }

    // -------------------------------------------------------------
    // 5️⃣ RMS‑beregning (Root‑Mean‑Square)
    // -------------------------------------------------------------
    private float CalcRMS(float[] samples)
    {
        float sum = 0f;
        for (int i = 0; i < samples.Length; i++)
            sum += samples[i] * samples[i];
        return Mathf.Sqrt(sum / samples.Length);
    }

    // -------------------------------------------------------------
    // 6️⃣ Hjælpe‑metode – opdatér tekst‑feltet
    // -------------------------------------------------------------
    private void SetStatus(string txt)
    {
        if (statusText != null) statusText.text = txt;
    }

    // -------------------------------------------------------------
    // 7️⃣ Filnavn (kan evt. eksponeres i Inspector)
    // -------------------------------------------------------------
    private const string fileName = "autoRecord.wav";
}
