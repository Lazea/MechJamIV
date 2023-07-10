using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
        fileName = "SO_Data_Actor",
        menuName = "Scriptable Objects/Data/Actor Data")]
public class ActorData : ScriptableObject
{
    public int health;
    public int shield;
    public float aimSpeed = 5f;

    public float attackInterval = 1f;
    public float minAttackEventRate = 3f;
    public float maxAttackEventRate = 5f;

    public float aimAngle = 10f;
    public float combatRange = 100f;
    public float chaseRange = 100f;
}
