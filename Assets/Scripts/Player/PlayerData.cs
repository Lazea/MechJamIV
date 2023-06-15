using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
        fileName = "SO_Data_Player",
        menuName = "Scriptable Objects/Data/Player Data")]
public class PlayerData : ScriptableObject
{
    [SerializeField]
    int maxHealth = 100;
    public int MaxHealth
    {
        get { return maxHealth; }
        set { maxHealth = value; }
    }
    [SerializeField]
    int health;
    public int Health
    {
        get { return health; }
        set { health = value; }
    }
    [SerializeField]
    int shield;
    public int Shield
    {
        get { return shield; }
        set { shield = value; }
    }

    public int credits;
}
