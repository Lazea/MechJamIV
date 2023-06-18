using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuUI : MonoBehaviour
{
    public GameObject gameplayPanel;
    public GameObject pausePanel;
    public GameObject menuPanel;
    public GameObject pauseMenuPanel;
    public GameObject optionsMenuPanel;
    public GameObject controlsPanel;

    public void Awake()
    {
        DisplayGameplay();
    }

    public void DisplayGameplay()
    {
        gameplayPanel.SetActive(true);
        pausePanel.SetActive(false);
        menuPanel.SetActive(false);
        pauseMenuPanel.SetActive(false);
        optionsMenuPanel.SetActive(false);
        controlsPanel.SetActive(false);
    }

    public void DisplayPauseMenu()
    {
        gameplayPanel.SetActive(false);
        pausePanel.SetActive(true);
        menuPanel.SetActive(true);
        pauseMenuPanel.SetActive(true);
        optionsMenuPanel.SetActive(false);
        controlsPanel.SetActive(false);
    }

    public void DisplayOptionsMenu()
    {
        gameplayPanel.SetActive(false);
        pausePanel.SetActive(true);
        menuPanel.SetActive(true);
        pauseMenuPanel.SetActive(false);
        optionsMenuPanel.SetActive(true);
        controlsPanel.SetActive(false);
    }

    public void DisplayControls()
    {
        gameplayPanel.SetActive(false);
        pausePanel.SetActive(true);
        menuPanel.SetActive(false);
        pauseMenuPanel.SetActive(false);
        optionsMenuPanel.SetActive(false);
        controlsPanel.SetActive(true);
    }
}
