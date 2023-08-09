using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : Singleton<GameManager>
{
    public PlayerData playerData;
    SceneTransitionUI sceneUI;

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

    [Header("Difficulty Manage")]
    public DifficultyManager difficultyManager;
    public Map_Card activeCard;

    [Header("Events")]
    public UnityEvent onPause;
    public UnityEvent onResume;

    private void Awake()
    {
        base.Awake();

        sceneUI = FindObjectOfType<SceneTransitionUI>();

        player = GameObject.Find("Player");

        // Setup Scene with starting stage
        if (playerData.stageCount == 0)
        {
            var card = difficultyManager.GetRandomCard(difficultyManager.startingCardPool);
            BaseCardReader.SetActiveCard(
                Map_Conditions.Instance,
                card,
                card.env);
        }
        Debug.LogFormat("Set active card {0}", activeCard.cardName);

        // Init NPC Spawn
        difficultyManager.ApplyNPCSpawnWaveSettings(activeCard);
        difficultyManager.ApplyCardLootRarityToWeaponGenerator(activeCard);

        IncrementStageCount();  // Stage count++

        // Setup NPC spawn and card hand
        var cardPool = difficultyManager.SelectCardPool(playerData.stageCount);
        difficultyManager.ApplyCardPoolToHand(cardPool);

        // Setup Map
        Map_Conditions.Instance.SetSeedLengths();
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
        sceneUI.StartPause();
        
        if (cardHand.isShowing)
            return;

        if (playerData.health <= 0)
            return;

        AudioSettingsManager.Instance.SetVolume("MusicVolumeParam", 0.45f);
        AudioSettingsManager.Instance.SetVolume("EffectsVolumeParam", 0f);

        Time.timeScale = 0f;
        onPause.Invoke();

        
    }

    public void ResumeGame() 
    {
        sceneUI.ResumeGame();

        Time.timeScale = 1f;

        AudioSettingsManager.Instance.SetVolume("MusicVolumeParam", 1f);
        AudioSettingsManager.Instance.SetVolume("EffectsVolumeParam", 1f);

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
