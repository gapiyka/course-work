using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] private Slider sensitivityScrollbar;
    [SerializeField] private Slider musicScrollbar;
    [SerializeField] private Slider vfxScrollbar;
    const float SensMultiplier = 250f;

    void Start()
    {
        if (File.Exists(Application.dataPath + "/save.txt"))
        {
            string saveString = File.ReadAllText(Application.dataPath + "/save.txt");
            SaveJsonObject saveObject = JsonUtility.FromJson<SaveJsonObject>(saveString);
            sensitivityScrollbar.value = saveObject.mouseSensitivity / SensMultiplier;
            musicScrollbar.value = saveObject.musicVolume;
            vfxScrollbar.value = saveObject.vfxVolume;
        }
    }

    public void TaskOnClickSave()
    {
        SaveJsonObject saveObject = new SaveJsonObject
        {
            mouseSensitivity = sensitivityScrollbar.value * SensMultiplier,
            musicVolume = musicScrollbar.value,
            vfxVolume = vfxScrollbar.value
        };

        string json = JsonUtility.ToJson(saveObject);
        File.WriteAllText(Application.dataPath + "/save.txt", json);
        Debug.Log("saved");
    }

    public void Back ()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
