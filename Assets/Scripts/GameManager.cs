using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : Singleton<GameManager>
{
    PlayerData playerData;

    public bool IsPaused
    {
        get { return (Time.timeScale == 0f); }
    }

    [Header("Events")]
    public UnityEvent onPause;
    public UnityEvent onResume;

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TogglePause()
    {
        if(IsPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
        onPause.Invoke();
    }

    public void ResumeGame() 
    {
        Time.timeScale = 1f;
        onResume.Invoke();
    }
}
