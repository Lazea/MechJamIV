using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
        fileName = "SO_Data_NPC",
        menuName = "Scriptable Objects/Data/NPC Data")]
public class NPCData : ScriptableObject
{
    public int health;
    public int shield;
}
