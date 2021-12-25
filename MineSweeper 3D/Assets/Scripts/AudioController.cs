using UnityEngine;

public class AudioController : MonoBehaviour
{
    [SerializeField] private AudioSource jumpSound;
    [SerializeField] private AudioSource boomSound;
    [SerializeField] private AudioSource[] selectSound; // 1-st for mine select; 2-nd for flags
    [SerializeField] private AudioSource backgroundMusic;

    void Start()
    {
        backgroundMusic.Play();
    }

    public void PlaySoundJump()
    {
        jumpSound.Play();
    }

    public void PlaySoundBoom()
    {
        boomSound.Play();
    }

    public void PlaySoundSelect(int n)
    {
        selectSound[n].Play();
    }

}