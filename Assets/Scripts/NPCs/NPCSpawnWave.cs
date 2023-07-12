using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
        fileName = "SO_NPCSpawnWave_Base",
        menuName = "Scriptable Objects/NPC Spawn Wave")]
public class NPCSpawnWave : ScriptableObject
{
    public int turretCount = 3;
    public GameObject[] turrets;
    public EnemyUnitSet[] enemyUnitSets;
}

[System.Serializable]
public struct EnemyUnitSet
{
    public GameObject enemyUnit;
    public int unitCount;
}
