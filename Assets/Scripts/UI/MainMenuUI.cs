using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    public GameObject mainMenuPanel;
    public GameObject optionsMenuPanel;
    public GameObject controlsPanel;
    public GameObject creditsPanel;

    public void Awake()
    {
        DisplayMainMenu();
    }

    public void DisplayMainMenu()
    {
        mainMenuPanel.SetActive(true);
        optionsMenuPanel.SetActive(false);
        controlsPanel.SetActive(false);
        creditsPanel.SetActive(false);
    }

    public void DisplayOptionsMenu()
    {
        mainMenuPanel.SetActive(false);
        optionsMenuPanel.SetActive(true);
        controlsPanel.SetActive(false);
        creditsPanel.SetActive(false);
    }

    public void DisplayControls()
    {
        mainMenuPanel.SetActive(false);
        optionsMenuPanel.SetActive(false);
        controlsPanel.SetActive(true);
        creditsPanel.SetActive(false);
    }

    public void DisplayCredits()
    {
        mainMenuPanel.SetActive(false);
        optionsMenuPanel.SetActive(false);
        controlsPanel.SetActive(false);
        creditsPanel.SetActive(true);
    }
}
