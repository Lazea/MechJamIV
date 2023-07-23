using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WeaponsDataset
{
    [Header("Weapon Pools")]
    public List<WeaponData> Common;
    public List<WeaponData> Uncommon;
    public List<WeaponData> Rare;
    public List<WeaponData> Legendary;

    [Header("Chances")]
    public float baseUncommonChance = 0.45f;
    public float baseRareChance = 0.2f;
}
