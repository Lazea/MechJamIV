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

    // TODO: This is a dumb fix for now. Find a better way to halt pausing if cards are visible
    [Header("REMOVE THIS WHEN POSSIBLE")]
    [SerializeField]
    CardHand cardHand;

    [Header("Events")]
    public UnityEvent onPause;
    public UnityEvent onResume;

    private void Start()
    {
        player = GameObject.Find("Player");
        IncrementStageCount();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TogglePause()
    {
        if (cardHand.isShowing)
            return;

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
        if (cardHand.isShowing)
            return;

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

    public void IncrementStageCount()
    {
        playerData.stageCount++;
    }

    public void PlayerDied()
    {
        playerIsDead = true;
    }
}
