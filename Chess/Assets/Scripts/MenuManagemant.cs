using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class MenuManagemant : MonoBehaviour
{
    [Header("Menu Stuff")]
    [SerializeField] private String NomeDoLevelDeJogo;
    [SerializeField] private GameObject MainScreen;
    [SerializeField] private GameObject ModosScreen;

    private AudioMangement menuAudioManager;

    private void Awake()
    {
        if(menuAudioManager == null)
        {
            menuAudioManager = FindObjectOfType<AudioMangement>();
            if(menuAudioManager == null)
            {
                Debug.Log("audioManager nulo");
            }
        }

        menuAudioManager.playBackgroundMusic("Audios/slowmotion");
        menuAudioManager.setBackgroundMusicVolume(0.2f);
    }

    // MainScreen bot�es
    public void Jogar()
    {
        if(menuAudioManager != null)
        {
            menuAudioManager.stopBackgroundMusic();
        }
        
        SceneManager.LoadScene(NomeDoLevelDeJogo);
    }

    public void onModosButton()
    {
        ModosScreen.SetActive(true);
        MainScreen.SetActive(false);

    }

    public void onExitButton()
    {
        Application.Quit();
    }

    // Modos bot�es

    public void onVoltarButton()
    {
        ModosScreen.SetActive(false);
        MainScreen.SetActive(true);
    }
}
