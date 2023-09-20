using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NewBehaviourScript : MonoBehaviour
{
    public static bool GameIsPaused = false;
    private AudioMangement audioManager;

    [SerializeField] private GameObject pauseMenuUI;


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
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }

}
