using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : Singleton<GameManager>
{
    public PlayerData playerData;

    GameObject player;
    public GameObject Player { get { return player; } }
    public Vector3 PlayerCenter { get { return player.transform.position + Vector3.up * 1f; } }

    bool playerIsDead;
    public bool PlayerIsDead { get { return playerIsDead; } }

    public bool IsPaused
    {
        get { return (Time.timeScale == 0f); }
    }

    [Header("Events")]
    public UnityEvent onPause;
    public UnityEvent onResume;

    private void Start()
    {
        player = GameObject.Find("Player");
    }

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

    public void AddCredits(int count)
    {
        playerData.credits += count;
    }

    public void IncrementNPCKillCount()
    {
        playerData.kills++;
    }

    public void PlayerDied()
    {
        playerIsDead = true;
    }
}
