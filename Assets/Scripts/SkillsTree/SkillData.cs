using UnityEngine;


[CreateAssetMenu(
        fileName = "SO_SkillData_Base",
        menuName = "Scriptable Objects/Data/Skill Data")]
public class SkillData : ScriptableObject
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

    [Header("Stat Boost Effect")]
    public SkillStatBoost[] statBoosts;
}

[System.Serializable]
public struct SkillStatBoost
{
    public enum StatBoostType
    {
        Health,
        Shield,
        Speed,
        BaseDamage,
        CritChance,
        CritDamage,
        FireChance,
        FireDamage,
        FireDuration,
        ShockChance,
        ShockDuration,
        ShieldDamage,
        RareWeaponChance
    }
    public StatBoostType statBoostType;
    public bool isPercentage;
    public float value;
}
