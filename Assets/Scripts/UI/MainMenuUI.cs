using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    public GameObject mainPanel;
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
        mainPanel.SetActive(true);
        mainMenuPanel.SetActive(true);
        optionsMenuPanel.SetActive(false);
        controlsPanel.SetActive(false);
        creditsPanel.SetActive(false);
    }

    public void DisplayOptionsMenu()
    {
        mainPanel.SetActive(true);
        mainMenuPanel.SetActive(false);
        optionsMenuPanel.SetActive(true);
        controlsPanel.SetActive(false);
        creditsPanel.SetActive(false);
    }

    public void DisplayControls()
    {
        mainPanel.SetActive(false);
        mainMenuPanel.SetActive(false);
        optionsMenuPanel.SetActive(false);
        controlsPanel.SetActive(true);
        creditsPanel.SetActive(false);
    }

    public void DisplayCredits()
    {
        mainPanel.SetActive(true);
        mainMenuPanel.SetActive(false);
        optionsMenuPanel.SetActive(false);
        controlsPanel.SetActive(false);
        creditsPanel.SetActive(true);
    }
}
