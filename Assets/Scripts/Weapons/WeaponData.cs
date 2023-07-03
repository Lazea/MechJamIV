using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WeaponData
{
    [Header("Projectile")]
    public ProjectileType projectileType;
    public List<ProjectileModifier> projectileModifiers;
    public float spawnOffset;
    public DamageType damageType;
    public float damageAmount;
    public float shieldMultiplier;

    [Header("Fire Mode")]
    public FireMode fireMode;
    public FireModeModifier fireModeModifier;
    public float recoil;

    [Header("Weapon Score")]
    public int power;
    public Rarity rarity;
}

public enum FireMode
{
    SemiAuto,
    BurstFire,
    FullAuto
}

public enum FireModeModifier
{
    None,
    ClusterFire,
    RampUpFire,
    DualSplit,
    TrippleSplit
}

public enum ProjectileType
{
    Ballistic,
    EnergyBeam,
    Rocket,
    Laser
}

public enum ProjectileModifier
{
    None,
    Ricochet,
    Penetrate,
    ClusterOnHit,
    ExplodeOnHit
}

public enum DamageType
{
    Normal,
    Energy,
    Fire,
    Shock
}

public enum Rarity
{
    Common,
    Uncommon,
    Rare,
    Legendary
}

