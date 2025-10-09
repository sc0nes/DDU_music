using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;
//songManager.SaveSong("Min Nyeste Sang", "Min Kunstner");
public class SongManager : MonoBehaviour
{
    public GameObject welcomeObject;
    public GameObject B1, B2, B3, B4, B5;
    public Text SONG1, SONG2, SONG3, SONG4, SONG5;
    public Text MAKER1, MAKER2, MAKER3, MAKER4, MAKER5;

    private const string saveFile = "saved_songs.json";

    public class SongData
    {//vores ting vi kan gemme
        public string songName;
        public string makerName;
        public System.DateTime saveDate;
    }

    private class SavedSongsData
    {
        public List<SongData> songs = new List<SongData>();
    }

    private SavedSongsData savedSongsData;
    private List<GameObject> songObjects;
    private List<Text> songNameFields;
    private List<Text> makerFields;
//start at programmer kalder
    private void Start()
    {
        referrence();
        LoadSavedSongs();
        UpdateSongDisplay();
    }

    private void referrence()
    {
        songObjects = new List<GameObject> { B1, B2, B3, B4, B5 };
        songNameFields = new List<Text> { SONG1, SONG2, SONG3, SONG4, SONG5 };
        makerFields = new List<Text> { MAKER1, MAKER2, MAKER3, MAKER4, MAKER5 };

        foreach (var songObj in songObjects)
        {
            if (songObj != null)
                songObj.SetActive(false);
        }
    }

    public void SaveSong(string songName, string makerName)
    {
        SongData newSong = new SongData
        {
            songName = songName,
            makerName = makerName,
        };

        savedSongsData.songs.Insert(0, newSong);

        if (savedSongsData.songs.Count > 5)
        {
            savedSongsData.songs.RemoveAt(5);
        }

        SaveSongsToFile();
        UpdateSongDisplay();
    }

    private void UpdateSongDisplay()
    {
        int songCount = savedSongsData.songs.Count;

        if (welcomeObject != null)
        {
            welcomeObject.SetActive(songCount == 0);
        }

        for (int i = 0; i < songObjects.Count; i++)
        {
            if (songObjects[i] != null)
            {
                bool shouldBeActive = i < songCount;
                songObjects[i].SetActive(shouldBeActive);

                if (shouldBeActive && i < savedSongsData.songs.Count)
                {
                    if (songNameFields[i] != null)
                        songNameFields[i].text = savedSongsData.songs[i].songName;
                    if (makerFields[i] != null)
                        makerFields[i].text = savedSongsData.songs[i].makerName;
                }
                else
                {
                    if (songNameFields[i] != null)
                        songNameFields[i].text = "";
                    if (makerFields[i] != null)
                        makerFields[i].text = "";
                }
            }
        }
    }

    public List<SongData> GetSavedSongs()
    {
        return new List<SongData>(savedSongsData.songs);
    }

    private void SaveSongsToFile()
    {
        string fullPath = Path.Combine(Application.persistentDataPath, saveFile);
        string json = JsonUtility.ToJson(savedSongsData, true);
        File.WriteAllText(fullPath, json);
    }

    private void LoadSavedSongs()
    {
        string fullPath = Path.Combine(Application.persistentDataPath, saveFile);

        if (File.Exists(fullPath))
        {
            string json = File.ReadAllText(fullPath);
            savedSongsData = JsonUtility.FromJson<SavedSongsData>(json);
        }
        else
        {
            savedSongsData = new SavedSongsData();
        }
    }

    public void SaveCurrentSong(string songName, string makerName)
    {
        if (!string.IsNullOrEmpty(songName) && !string.IsNullOrEmpty(makerName))
        {
            SaveSong(songName, makerName);
        }
    }

    public void TestSaveSong()
    {
        SaveSong("Test Sang " + System.DateTime.Now.ToString("HH:mm:ss"), "Test Kunstner");
    }

}
