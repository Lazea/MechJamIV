using UnityEngine;

[System.Serializable]
public class Skill
{
    [Header("Info")]
    public string name;
    [HideInInspector]
    public int id;
    public Sprite icon;
    [TextAreaAttribute]
    public string description;

    [Header("Cost")]
    public int cost = 10;
    public bool unlocked;

    [Header("Hierarchy")]
    public int skillTreeLayer;
    public int[] childrenInNextLayer;
    public int[] childrenInLayer;

    [Header("Stat Boost Effect")]
    public float statBoostValue;
    public enum StatBoostType
    {
        Health,
        Shield,
        Damage,
        Speed,
        None
    }
    public StatBoostType statBoostType;

    // TODO: Implement abilities
    //[Header("Ability")]
    //...
}
