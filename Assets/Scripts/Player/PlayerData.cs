using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[CreateAssetMenu(
        fileName = "SO_Data_Player",
        menuName = "Scriptable Objects/Data/Player Data")]
public class PlayerData : ScriptableObject
{
    [Header("Health")]
    [SerializeField]
    [Tooltip("The base max health the player starts with each run.")]
    int maxHealth = 100;
    public int MaxHealth
    {
        set
        {
            maxHealth = value;
        }
        get
        {
            // TODO: Implement a health modifiers list and add to maxHP return
            return maxHealth;
        }
    }
    public int health;

    [SerializeField]
    [Tooltip("The base max shield the player starts with each run.")]
    int maxShield = 100;
    public int MaxShield
    {
        set
        {
            maxShield = value;
        }
        get
        {
            // TODO: Implement a health modifiers list and add to maxHP return
            return maxShield;
        }
    }
    public int shield;

    [Header("Lateral Thrusters")]
    [Tooltip("The lateral dash ability cooldown.")]
    public float lateralThrustCooldown = 1.5f;

    [Header("Vertical Thrusters")]
    [Tooltip("The vertical force for jumping.")]
    public float jumpForce = 8f;
    [Tooltip("The vertical thrust force for flying.")]
    public float verticalThrust = 15f;
    [Tooltip("The max vertical thrust speed curve based on thruster fuel.")]
    public AnimationCurve maxVerticalSpeedFuelFalloff;
    [Tooltip("The amount of vertical thruster fuel.")]
    public float maxVerticalThrusterFuel = 100f;
    [Tooltip("The vertical thruster fuel burn rate.")]
    public float verticalThrusterFuleRate = 10f;
    [Tooltip("The vertical thruster fuel cooldown before fuel regenerates.")]
    public float verticalThrustCooldown = 1.5f;

    [Header("Ground Speed")]
    [Tooltip(
        "A value [1,~] which interpolates between the fastest" +
        "and slowest animation movement speeds")]
    public float speedScaler = 1f;

    [Header("Damage Scale")]
    [Tooltip("The base damage multiplier applied to all damage the player deals.")]
    public float damageMultiplier = 1f;

    [Header("Crit Damage")]
    [Range(0f, 1f)]
    [Tooltip("The critical hit chance between (0.0, 1.0]. The higher the number the higher the chance.")]
    public float critChance = 0.15f;
    [Tooltip("The amount base damage is scaled by when a crit hit happens.")]
    public float critDamageMultiplier = 2f;

    [Header("Energy Damage")]
    [Tooltip("The multiplier placed on shield damage.")]
    public float shieldEnergyDamageMultiplier = 2f;

    [Header("Fire Damage")]
    [Range(0f, 1f)]
    [Tooltip("The chance of applying fire damage set between (0.0, 1.0]. The higher the number the higher the chance.")]
    public float fireChance = 0.15f;
    [Tooltip("The amount of damage the fire effect deals.")]
    public int fireDamage = 2;
    [Tooltip("The amount of time the fire effect lasts.")]
    public float fireDuration = 2f;

    [Header("Shock Damage")]
    [Range(0f, 1f)]
    [Tooltip("The chance of applying shock damage set between (0.0, 1.0]. The higher the number the higher the chance.")]
    public float shockChance = 0.15f;
    [Tooltip("The amount of time the shock effect lasts.")]
    public float shockDuration = 2f;

    [Header("Player Stat Tracking")]
    [Tooltip("The amount of credits the player earned.")]
    public int credits;
    [Tooltip("The amount of credits the player safely deposited.")]
    public int creditsSaved;
    [Tooltip("The amount of kills the player earned.")]
    public int kills;

    [ContextMenu("Reset Base Data")]
    public void ResetData()
    {
        Debug.Log("Player Data Reset");

        IniParser parser = new IniParser();
        string filePath = Path.Combine(Application.dataPath, "settings", "GameConfig.ini");
        parser.Parse(filePath);

        string section = "PlayerData";
        maxHealth = parser.GetValue<int>(section, "maxHealth");
        health = maxHealth;
        maxShield = parser.GetValue<int>(section, "maxShield");
        shield = maxShield;

        lateralThrustCooldown = parser.GetValue<float>(
            section, "lateralThrustCooldown");

        verticalThrust = parser.GetValue<float>(
            section, "verticalThrust");
        maxVerticalThrusterFuel = parser.GetValue<float>(
            section, "maxVerticalThrusterFuel");
        verticalThrusterFuleRate = parser.GetValue<float>(
            section, "verticalThrusterFuleRate");
        verticalThrustCooldown = parser.GetValue<float>(
            section, "verticalThrustCooldown");

        speedScaler = parser.GetValue<float>(section, "speedScaler");

        damageMultiplier = parser.GetValue<float>(section, "damageMultiplier");
        critChance = parser.GetValue<float>(section, "critChance");
        critDamageMultiplier = parser.GetValue<float>(section, "critDamageMultiplier");
        shieldEnergyDamageMultiplier = parser.GetValue<float>(section, "shieldEnergyDamageMultiplier");
        
        fireChance = parser.GetValue<float>(section, "fireChance");
        fireDamage = parser.GetValue<int>(section, "fireDamage");
        fireDuration = parser.GetValue<float>(section, "fireDuration");

        shockChance = parser.GetValue<float>(section, "shockChance");
        shockDuration = parser.GetValue<float>(section, "shockDuration");
    }

    [ContextMenu("Reset In Run Modifiers")]
    public void ResetInRunModifiers()
    {
        // TODO: Reset all modifier stacks
        throw new NotImplementedException();
    }

    [ContextMenu("Reset Credits")]
    public void ResetCredits()
    {
        Debug.LogFormat("Player Credits {0} Reset", this.name);

        IniParser parser = new IniParser();
        string filePath = Path.Combine(Application.dataPath, "settings", "GameConfig.ini");
        parser.Parse(filePath);

        credits = parser.GetValue<int>("PlayerData", "credits");
    }

    [ContextMenu("Reset Kills")]
    public void ResetKillCount()
    {
        kills = 0;
    }
}
