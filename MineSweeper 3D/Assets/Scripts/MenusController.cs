using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class MenusController : MonoBehaviour
{
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject startMenu;
    [SerializeField] private GameObject settings;
    [SerializeField] private Image background;
    [SerializeField] private GridManager gridManager;
    [SerializeField] private Slider sensitivitySlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider vfxSlider;
    private bool IsGameStarted = false;
    private const float SensMultiplier = 250f;
    private const float transparentAlpha = 0.5f;
    private const float fullTransparentAlpha = 0f;
    private const float opaqueAlpha = 1f;
    private const byte dByte = 255;
    private const string congifSavePath = "/config.txt";
    void Start()
    {
        TurnOnMM();
        LoadConfig();
    }
    public void StartMenu()
    {
        TurnOffMM();
        startMenu.SetActive(true);
    }

    public void Settings()
    {
        TurnOffMM();
        settings.SetActive(true);
        LoadConfig();
    }

    public void Save()
    {
        SaveJsonObject saveObject = new SaveJsonObject
        {
            mouseSensitivity = sensitivitySlider.value * SensMultiplier,
            musicVolume = musicSlider.value,
            vfxVolume = vfxSlider.value
        };

        string json = JsonUtility.ToJson(saveObject);
        File.WriteAllText(Application.dataPath + congifSavePath, json);
        LoadConfig();
    }

    public void StartSolo()
    {
        CloseMenu();
        IsGameStarted = true;
        LoadConfig();
        gridManager.ReloadMap(1);
    }

    public void StartOnline()
    {

    }

    public void JoinServer()
    {

    }

    public void Back()
    {
        TurnOffAllMenus();
        TurnOnMM();
    }

    public void Quit()
    {
        Application.Quit();
    }

    public bool PressEsc(bool IsGameRunning)
    {
        if (IsGameRunning)
        {
            TurnOnMM();
            return false;
        }
        else
        {
            if (mainMenu.activeSelf && IsGameStarted)
            {
                CloseMenu();
                return true;
            }
            else
            {
                Back();
                return false;
            }
        }
    }

    public bool IsGameLaunched() {
        return IsGameStarted;
    }

    public SaveJsonObject LoadConfig()
    {
        string saveString = File.ReadAllText(Application.dataPath + congifSavePath);
        SaveJsonObject saveObject = JsonUtility.FromJson<SaveJsonObject>(saveString);
        sensitivitySlider.value = saveObject.mouseSensitivity / SensMultiplier;
        musicSlider.value = saveObject.musicVolume;
        vfxSlider.value = saveObject.vfxVolume;
        return saveObject;
    }

    void TurnOffAllMenus()
    {
        Transform menusT = this.transform;
        int childCount = menusT.childCount;
        if (childCount > 0)
        {
            for (int i = 0; i < childCount; i++)
            {
                menusT.GetChild(i).gameObject.SetActive(false);
            }
        }
    }

    void CloseMenu()
    {
        TurnOffAllMenus();
        Color color = new Color(dByte, dByte, dByte, fullTransparentAlpha);
        background.color = color;
    }

    void TurnOffMM()
    {
        mainMenu.SetActive(false);
    }

    void TurnOnMM()
    {
        mainMenu.SetActive(true);
        float currAlpha = IsGameStarted ? transparentAlpha : opaqueAlpha;
        Color color = new Color(dByte, dByte, dByte, currAlpha);
        background.color = color;
    }
}
