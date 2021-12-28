using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
    public void StartSolo()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void StartOnline ()
    {
        //SceneManager.LoadScene("OnlineScene");
    }

    public void Back ()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
