/*********************************************************************
 * AutoRecorderButton – optagning ved hold‑ned‑knap
 *
 *   • Hold knappen (UI‑Button) nede → optagning starter.
 *   • Slip knappen → optagning stopper og gemmes.
 *   • Max. kliplængde 3599 sekunder (≈ 1 t – 0 min – 1 sek).
 *   • UI‑status‑tekst (Text eller TextMeshPro) viser Lytter / Optager /
 *     Stoppet / Fil‑sti.
 *********************************************************************/

using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;                       // UI‑Text (eller TMP_Text)
#if TMP_PRESENT
using TMPro;                                 // TextMeshPro (valgfrit)
#endif
using UnityEngine.EventSystems;             // IPointerDownHandler / IPointerUpHandler

#if UNITY_ANDROID
using UnityEngine.Android;
#endif

public class SaveSongs : MonoBehaviour,
                                   IPointerDownHandler,
                                   IPointerUpHandler
{
    // -----------------------------------------------------------------
    // 0️⃣ UI – status‑tekst (drag & drop i Inspector)
    // -----------------------------------------------------------------
    [Header("UI")]
    // public TMP_Text statusText;   // <-- brug denne, hvis du har TextMeshPro
    public Text statusText;          // <-- brug denne, hvis du bruger UI‑Text

    // -----------------------------------------------------------------
    // 1️⃣ Indstillinger
    // -----------------------------------------------------------------
    [Header("Optagelses‑indstillinger")]
    [Tooltip("Sample‑rate – 44100 er standard")]
    public int sampleRate = 44100;

    [Tooltip("Maks. varighed i sekunder (≤ 3599)")]
    public int maxLengthSec = 3599;   // 3599 sek ≈ 1 t – 0 min – 1 sek

    // -----------------------------------------------------------------
    // 2️⃣ Interne felter
    // -----------------------------------------------------------------
    private AudioClip micClip;          // den aktuelle optagelse
    private bool isRecording = false;

    // -----------------------------------------------------------------
    // 3️⃣ Unity‑livscyklus
    // -----------------------------------------------------------------
    private void Start()
    {
#if UNITY_ANDROID
        // Mikrofon‑tilladelse på Android
        if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
            Permission.RequestUserPermission(Permission.Microphone);
#endif
        // Udgangspunkt – vent på knap‑ned‑tryk
        SetStatus("Vent på knap‑ned‑tryk …");
    }

    // -----------------------------------------------------------------
    // 4️⃣ Knap‑ned‑event – starter optagelse
    // -----------------------------------------------------------------
    public void OnPointerDown(PointerEventData eventData)
    {
        // Undgå dobbelt‑start, hvis brugeren holder knappen meget længe
        if (isRecording) return;

        // Start en ny optagelse (ingen loop‑buffer)
        micClip = Microphone.Start(null, false, maxLengthSec, sampleRate);
        isRecording = true;
        SetStatus("Optager …");
        Debug.Log("[AutoRecorderButton] Optagelse startet (holdes nede).");
    }

    // -----------------------------------------------------------------
    // 5️⃣ Knap‑op‑event – stopper optagelse + gemmer
    // -----------------------------------------------------------------
    public void OnPointerUp(PointerEventData eventData)
    {
        if (!isRecording) return;               // intet at stoppe

        // Hent den aktuelle position *før* vi slutter med Microphone
        int endPos = Microphone.GetPosition(null);
        Microphone.End(null);
        isRecording = false;

        SetStatus("Optagelse stoppet – gemmer …");
        Debug.Log("[AutoRecorderButton] Optagelse stoppet, gemmer…");

        SaveClip(endPos);                       // gem filen
    }

    // -----------------------------------------------------------------
    // 6️⃣ Gem filen (WAV)
    // -----------------------------------------------------------------
    private void SaveClip(int endPos)
    {
        if (endPos <= 0)
        {
            SetStatus("Ingen data – intet gemt.");
            Debug.LogWarning("[AutoRecorderButton] EndPos = 0 – intet at gemme.");
            return;
        }

        // Hent samples fra start (0) til endPos
        float[] wavData = new float[endPos * micClip.channels];
        micClip.GetData(wavData, 0);

        // Opret en præcis AudioClip – kun den reelle længde
        AudioClip finalClip = AudioClip.Create("record", endPos,
                                               micClip.channels,
                                               micClip.frequency,
                                               false);
        finalClip.SetData(wavData, 0);

        // Mappe til persistent storage
        string folder = Path.Combine(Application.persistentDataPath, "Recordings");
        if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

        string path = Path.Combine(folder, fileName);

        // -------------------------------------------------
        // ⚠️ Du skal have en WAV‑skrivnings‑helper:
        //     WavUtility (https://github.com/deadlyfingers/UnityWavUtility)
        //     eller din egen SaveWav‑funktion.
        // -------------------------------------------------
        WavUtility.Save(path, finalClip);

        SetStatus($"Gemte til:\n{path}");
        Debug.Log($"[AutoRecorderButton] Gemte til: {path}");
    }

    // -----------------------------------------------------------------
    // 7️⃣ Hjælpe‑metode – opdatér UI‑tekst
    // -----------------------------------------------------------------
    private void SetStatus(string txt)
    {
        if (statusText != null) statusText.text = txt;
    }

    // -----------------------------------------------------------------
    // 8️⃣ Filnavn (kan eksponeres i Inspector hvis ønsket)
    // -----------------------------------------------------------------
    private const string fileName = "buttonRecord.wav";
}
