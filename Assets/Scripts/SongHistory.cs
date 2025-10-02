/*********************************************************************
 * SongHistory – gemmer de seneste sange + deres kunstnere
 *
 *   • 10 UI‑tekstfelter (5 song + 5 maker) sættes i Inspector.
 *   •  AddSong(string song, string maker)  → skifter listen ned,
 *       gemmer til fil og opdaterer UI.
 *   •  Data gemmes som JSON i  Application.persistentDataPath .
 *
 *   •  Du kan kalde AddSong fra enhver anden komponent
 *      (fx når du henter en ny sang fra en ekstern kilde).
 *********************************************************************/

using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

// UI‑tekst; skift til TMP_Text, hvis du bruger TextMeshPro
using UnityEngine.UI;           // <- normal Unity UI
//using TMPro;                 // <- udkommentér ovenfor, hvis du bruger TMP

public class SongHistory : MonoBehaviour
{
    // -----------------------------------------------------------------
    // 0️⃣ UI – træk 10 tekst‑felter ind i Inspector
    // -----------------------------------------------------------------
    [Header("UI – Song‑navne (1‑5)")]
    public Text[] songTexts = new Text[5];   // songTexts[0] = nr. 1, … songTexts[4] = nr. 5

    [Header("UI – Kunstner‑navne (1‑5)")]
    public Text[] makerTexts = new Text[5];   // makerTexts[0] = nr. 1, …

    // -----------------------------------------------------------------
    // 1️⃣ Intern data‑struktur
    // -----------------------------------------------------------------
    [System.Serializable]
    private class SongEntry
    {
        public string song;
        public string maker;
    }

    private const string fileName = "songHistory.json";
    private List<SongEntry> history = new List<SongEntry>();  // maks 5

    // -----------------------------------------------------------------
    // 2️⃣ Unity‑livscyklus
    // -----------------------------------------------------------------
    private void Awake()
    {
        LoadHistory();          // henter gemt resultat (hvis der findes)
        UpdateUI();             // viser hvad vi har ved start
    }

    // -----------------------------------------------------------------
    // 3️⃣ PUBLIC – kaldes fra andre scripts når en ny sang kommer
    // -----------------------------------------------------------------
    /// <summary>
    /// Tilføjer et nyt (song, maker)‑par. Gammel data rykkes ned.
    /// </summary>
    public void AddSong(string song, string maker)
    {
        // 1. Indsæt øverst
        history.Insert(0, new SongEntry { song = song, maker = maker });

        // 2. Hold kun de seneste 5
        if (history.Count > 5) history.RemoveAt(5);

        // 3. Gem til disk og opdatér UI
        SaveHistory();
        UpdateUI();
    }

    // -----------------------------------------------------------------
    // 4️⃣ UI‑opdatering
    // -----------------------------------------------------------------
    private void UpdateUI()
    {
        // Nulstil alt for at undgå “null reference” hvis du har færre end 5
        for (int i = 0; i < 5; i++)
        {
            if (songTexts[i] != null) songTexts[i].text = "";
            if (makerTexts[i] != null) makerTexts[i].text = "";
        }

        // Fyld de felter, vi har data til
        for (int i = 0; i < history.Count; i++)
        {
            if (songTexts[i] != null) songTexts[i].text = history[i].song;
            if (makerTexts[i] != null) makerTexts[i].text = history[i].maker;
        }
    }

    // -----------------------------------------------------------------
    // 5️⃣ Gem / indlæs JSON‑fil (samme mekanik som din gamle Score‑klasse)
    // -----------------------------------------------------------------
    private void SaveHistory()
    {
        string path = Path.Combine(Application.persistentDataPath, fileName);
        string json = JsonUtility.ToJson(new Wrapper { list = history }, true);
        File.WriteAllText(path, json);
        //Debug.Log($"SongHistory gemt til: {path}");
    }

    private void LoadHistory()
    {
        string path = Path.Combine(Application.persistentDataPath, fileName);
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            Wrapper wrapper = JsonUtility.FromJson<Wrapper>(json);
            history = wrapper.list ?? new List<SongEntry>();
            //Debug.Log($"SongHistory indlæst fra: {path}");
        }
        else
        {
            // Ingen fil – start med tom liste
            history = new List<SongEntry>();
        }
    }

    // JsonUtility kan ikke (direkte) serialisere List<T> – vi bruger en wrapper‑klasse.
    [System.Serializable]
    private class Wrapper
    {
        public List<SongEntry> list;
    }

    // -----------------------------------------------------------------
    // 6️⃣ Eksempel‑metode: kaldes fra en anden komponent (valgfri)
    // -----------------------------------------------------------------
    /// <summary>
    /// Eksempel på, hvordan du fra et andet script kan give en ny sang.
    /// </summary>
    public void DemoAddFromExternal()
    {
        // “Extern kilde” – du kan erstatte dette med f.eks. en web‑request.
        string nySong = "My New Song";
        string nyMaker = "Cool Artist";

        AddSong(nySong, nyMaker);
    }
}
