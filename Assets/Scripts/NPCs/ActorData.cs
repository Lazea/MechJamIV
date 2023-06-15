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
}
