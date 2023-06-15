using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
        fileName = "SO_Data_Player",
        menuName = "Scriptable Objects/Data/Player Data")]
public class PlayerData : ScriptableObject
{
    int maxHealth = 100;
    public int MaxHealth
    {
        get { return maxHealth; }
        set { maxHealth = value; }
    }
    int health;
    public int Health
    {
        get { return health; }
        set { health = value; }
    }

    public int credits;
}
