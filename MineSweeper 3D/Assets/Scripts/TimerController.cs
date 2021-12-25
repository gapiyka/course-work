using UnityEngine;
using UnityEngine.UI;

public class TimerController : MonoBehaviour
{
    [SerializeField] private Text timerText;
    [SerializeField] private Text minesText;
    [SerializeField] private GameObject timerBar;
    private bool IsTimerRunning = false;
    private float startGameTime;
    private int minesCounter;

    void Update()
    {
        if (IsTimerRunning)
        {
            const string timeSeparator = ":";
            const int minD = 60;
            int deltaGameTime = (int)(Time.time - startGameTime);
            string deltaMin = FormatInteger(deltaGameTime / minD);
            string deltaSec = FormatInteger(deltaGameTime % minD);
            timerText.text = deltaMin + timeSeparator + deltaSec;
            minesText.text = FormatInteger(minesCounter);
        }             
    }

    void ManageTimer(bool state)
    {
        IsTimerRunning = state;
        timerBar.SetActive(state);
    }
    string FormatInteger(int num)
    {
        const int digitBorder = 10;
        string modifier = (num < digitBorder) ? "0" : "";
        if (num < 0) num = 0;
        return modifier + num.ToString();
    }

    public void RunTimer(int nMines)
    {
        const bool on = true;
        startGameTime = Time.time;
        minesCounter = nMines;
        ManageTimer(on);
    }

    public void StopTimer()
    {
        const bool off = false;
        ManageTimer(off);
    }

    public void MinesDecrease()
    {
        minesCounter--;
    }
    public void MinesIncrease()
    {
        minesCounter++;
    }
}
