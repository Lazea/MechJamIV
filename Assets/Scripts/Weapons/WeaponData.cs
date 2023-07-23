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
    public float baseDamage;
    public float shieldMultiplier = 1f;

    [Header("Fire Mode")]
    public FireMode fireMode;
    public FireModeModifier fireModeModifier;
    public float baseAccuracy;
    public float baseFireRate;

    [Header("Weapon Rarity")]
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
    Cluster,
    RampUpFire,
    DualSplit,
    TrippleSplit
}

public enum ProjectileType
{
    Ballistic,
    Plasma,
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

