using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class MenusController : MonoBehaviour
{
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject startMenu;
    [SerializeField] private GameObject settings;
    [SerializeField] private GameObject restartMenu;
    [SerializeField] private Image background;
    [SerializeField] private GridManager gridManager;
    [SerializeField] private PlayerConfiguration.PlayerController playerController;
    [SerializeField] private Slider sensitivitySlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider vfxSlider;
    [SerializeField] private Slider difficultySlider;

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
        playerController.UnlockScreenCursor();
    }

    void Update()
    {
        if(playerController.IsPressedESC) PressEsc();
        if (gridManager.gameState == GameState.Lose || gridManager.gameState == GameState.Win) RestartMenu();
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
        int dif = (int)difficultySlider.value;
        CloseMenu();
        playerController.IsGameRunning = true;
        playerController.IsGamePaused = false;
        gridManager.ReloadMap(dif);
        playerController.LockScreenCursor();
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

    public void PressEsc()
    {
        if (playerController.IsGameRunning && !playerController.IsGamePaused)
        {
            TurnOnMM();
            playerController.IsGamePaused = true;
            playerController.UnlockScreenCursor();
            gridManager.StopTimer();
        }
        else
        {
            if (mainMenu.activeSelf && playerController.IsGameRunning)
            {
                CloseMenu();
                playerController.IsGamePaused = false;
                playerController.LockScreenCursor();
                gridManager.RunTimer(false);
            }
            else
            {
                Back();
            }
        }
    }

    public void RestartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

    void LoadConfig()
    {
        string saveString = File.ReadAllText(Application.dataPath + congifSavePath);
        SaveJsonObject saveObject = JsonUtility.FromJson<SaveJsonObject>(saveString);
        sensitivitySlider.value = saveObject.mouseSensitivity / SensMultiplier;
        musicSlider.value = saveObject.musicVolume;
        vfxSlider.value = saveObject.vfxVolume;
        playerController.UpdateSettings(saveObject);
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
        float currAlpha = playerController.IsGameRunning ? transparentAlpha : opaqueAlpha;
        Color color = new Color(dByte, dByte, dByte, currAlpha);
        background.color = color;
    }

    void PrintResultOfGame(bool win)
    {
        string[] results = gridManager.GetResults();
        string timeText = "Your time is: " + results[0];
        string minesText = "Mines at map: " + results[1];
        string res = win ?
            "Hooray, you won!!!" : "Opps, you lose!"; 
        Text resText = restartMenu.transform.GetChild(1).GetComponent<Text>();
        resText.text = res + "\n" + timeText + "\n" + minesText;
    }

    void RestartMenu()
    {
        bool win = (gridManager.gameState == GameState.Win) ? true : false;
        PrintResultOfGame(win);
        restartMenu.SetActive(true);
        playerController.UnlockScreenCursor();
        playerController.IsGamePaused = true;
        if (win) playerController.WinAnimate();
    }
}
