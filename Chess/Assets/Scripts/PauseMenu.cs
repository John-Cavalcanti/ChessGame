using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NewBehaviourScript : MonoBehaviour
{
    public static bool GameIsPaused = false;
    private AudioMangement audioManager;
    public ChessBoard chessboard;

    [SerializeField] private GameObject pauseMenuUI;


    private void Awake()
    {
        if (audioManager == null)
        {
            audioManager = FindObjectOfType<AudioMangement>();
            if (audioManager == null)
            {
                Debug.Log("audioManager nulo");
            }
        }

        if (chessboard == null)
        {
            chessboard = FindObjectOfType<ChessBoard>();
            if (chessboard == null)
            {
                Debug.Log("audioManager nulo");
            }
        }
    }

    // Update is called once per frame
    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
            {
                Resume();

            } else
            {
                Pause();
            }
        }

    }

    public void onAumentarButton()
    {
        audioManager.increaseBackgroundMusicVolume();
    }

    public void onDiminuirButton()
    {
        audioManager.decreaseBackgroundMusicVolume();
    }

    public void onMenuButton()
    {
        SceneManager.LoadScene("Menu");
    }

    public void onExitButton()
    {
        Application.Quit();
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        chessboard.enabled = true;
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        chessboard.enabled = false;
        Time.timeScale = 0f;
        GameIsPaused = true;
    }


}
