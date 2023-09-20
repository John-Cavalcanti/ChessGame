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

    // MainScreen botões
    public void Jogar()
    {
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

    // Modos botões

    public void onVoltarButton()
    {
        ModosScreen.SetActive(false);
        MainScreen.SetActive(true);
    }
}
