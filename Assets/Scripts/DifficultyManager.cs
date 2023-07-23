using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DifficultyManager
{
    [Header("The card pool the game starts with as stage 0")]
    public Map_Card[] startingCardPool;
    [Header("The card pool the game starts with as early stages")]
    public Map_Card[] earlyCardPool;
    [Header("The card pool the game starts with as mid stages")]
    public Map_Card[] midCardPool;
    [Header("The card pool the game starts with as late stages")]
    public Map_Card[] lateCardPool;
    public CardHand cardHand;

    [Header("The stage counts needed to be exceeded for the next card pool to be used.")]
    public int startStageThreshold = 1;
    public int earlyStageThreshold = 3;
    public int midStageThreshold = 5;

    public Map_Card[] SelectCardPool(int stageCount)
    {
        if (stageCount >= midStageThreshold)
            return lateCardPool;
        else if (stageCount >= earlyStageThreshold)
            return midCardPool;
        else if (stageCount >= startStageThreshold)
            return earlyCardPool;
        else
            return startingCardPool;
    }

    public Map_Card GetRandomCard(Map_Card[] cardPool)
    {
        int i = Random.Range(0, cardPool.Length);
        return cardPool[i];
    }

    public void ApplyNPCSpawnWaveSettings(Map_Card card)
    {
        NPCsManager.Instance.wavesCount = card.enemyWaveCount;
        NPCsManager.Instance.spawnWavesPool = card.npcWavePool;
        Debug.LogFormat(
            "Applied spawn waves from card {0}: Now have wave count {1} and pool size of {2}",
            card.name,
            NPCsManager.Instance.wavesCount,
            NPCsManager.Instance.spawnWavesPool.Length);
    }

    public void ApplyCardLootRarityToWeaponGenerator(Map_Card card)
    {
        WeaponGenerator.Instance.maxDropRarity = card.maxRarity;
    }

    public void ApplyCardPoolToHand(Map_Card[] cardPool)
    {
        cardHand.cardPool = cardPool;
        cardHand.PickCards();
    }
}
