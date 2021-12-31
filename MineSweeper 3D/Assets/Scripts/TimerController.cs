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
    private int deltaGameTime = 0;

    void Update()
    {
        if (IsTimerRunning)
        {
            deltaGameTime = (int)(Time.time - startGameTime);
            timerText.text = GetStringTimer();
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

    public void RunTimer(int nMines, bool newTimer)
    {
        deltaGameTime = newTimer ? 0 : deltaGameTime;
        const bool on = true;
        startGameTime = Time.time - deltaGameTime;
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

    public int GetCurrentMinesCounter()
    {
        return minesCounter;
    }

    public string GetStringTimer()
    {
        const string timeSeparator = ":";
        const int minD = 60;
        string deltaMin = FormatInteger(deltaGameTime / minD);
        string deltaSec = FormatInteger(deltaGameTime % minD);

        return deltaMin + timeSeparator + deltaSec; ;
    }
}
