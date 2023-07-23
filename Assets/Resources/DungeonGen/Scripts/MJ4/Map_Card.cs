using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Map Card", menuName = "Scriptable Objects/MapCard")]
public class Map_Card : ScriptableObject
{
    [Header("Name of map")]
    public string cardName;

    [Header("Env Settings")]
    [Header("Tileset for next map, leave blank to pick on instantiation")]
    public Tileset_Info tileset;
    [Header("Environment for next map, leave blank to pick at instantiation")]
    public Tileset_ENV env;

    [Header("Tile Settings")]
    [Header("Hide environmental data in map screen")]
    public bool obscureTileset;
    [Header("Number to stack on top of default seed length")]
    public int lengthMod = 0;
    [Header("Number to stack on top of default seondary passes")]
    public int secondaryMod = 0;

    [Header("Difficulty Settings")]
    [Header("Enemy wave count")]
    [Min(1)]
    public int enemyWaveCount;
    [Header("Pool of NPC waves this stage can use")]
    public NPCSpawnWave[] npcWavePool; 
    [Header("Max loot Rarity")]
    public Rarity maxRarity;
    [Header("Loot drop chance. Chance of 1.0 is 100% chance")]
    [Range(0.0f, 1.0f)]
    public float lootDropChance = 1f;
}

[System.Serializable]
public enum Difficulty
{
    Easy,
    Medium,
    Hard
}
