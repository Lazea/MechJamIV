using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PickupMenuUI : Singleton<PickupMenuUI>
{
    public GameObject crosshair;
    public GameObject pickupInfoParent;

    [Header("Carry Weapon UI Elements")]
    public Image carryWeaponIcon;
    public TextMeshProUGUI carryWeaponName;
    public TextMeshProUGUI carryWeaponDamageStat;
    public TextMeshProUGUI carryWeaponAccuracyStat;
    public Image carryWeaponProjectileIcon;
    public Image carryWeaponDamageTypeIcon;
    public Image carryWeaponFireModeIcon;
    public TextMeshProUGUI[] carryWeaponModifierStats;

    [Header("Ground Weapon UI Elements")]
    public Image groundWeaponIcon;
    public TextMeshProUGUI groundWeaponName;
    public TextMeshProUGUI groundWeaponDamageStat;
    public TextMeshProUGUI groundWeaponAccuracyStat;
    public Image groundWeaponProjectileIcon;
    public Image groundWeaponDamageTypeIcon;
    public Image groundWeaponFireModeIcon;
    public TextMeshProUGUI[] groundWeaponModifierStats;

    [Header("Pickup Indicator")]
    public Image pickupIndicator;

    [Header("Projectile Type Icons")]
    public Sprite ballisticIcon;
    public Sprite plasmaIcon;
    public Sprite rocketIcon;

    [Header("Fire Modes Icons")]
    public Sprite semiAutoIcon;
    public Sprite burstFireIcon;
    public Sprite fullAutoIcon;

    [Header("Damage Type Icons")]
    public Sprite normalDmgIcon;
    public Sprite fireDmgIcon;
    public Sprite shockDmgIcon;
    public Sprite energyDmgIcon;

    // Start is called before the first frame update
    void Start()
    {
        this.enabled = false;

        carryWeaponProjectileIcon.preserveAspect = true;
        carryWeaponDamageTypeIcon.preserveAspect = true;
        carryWeaponFireModeIcon.preserveAspect = true;

        groundWeaponProjectileIcon.preserveAspect = true;
        groundWeaponDamageTypeIcon.preserveAspect = true;
        groundWeaponFireModeIcon.preserveAspect = true;

        pickupIndicator.fillAmount = 0f;
    }

    private void OnEnable()
    {
        crosshair.SetActive(false);
        pickupInfoParent.SetActive(true);
    }

    private void OnDisable()
    {
        crosshair.SetActive(true);
        pickupInfoParent.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetCarryWeapon(WeaponData data)
    {
        // TODO: Set icon

        carryWeaponDamageStat.text = data.damageAmount.ToString();

        float accuracy = 1f - Mathf.Clamp01(data.recoil / 0.075f);
        accuracy *= 100f;
        carryWeaponAccuracyStat.text = Mathf.RoundToInt(accuracy).ToString();

        carryWeaponProjectileIcon.sprite = GetProjectileTypeSprite(data.projectileType);
        carryWeaponFireModeIcon.sprite = GetFireModeSprite(data.fireMode);
        carryWeaponDamageTypeIcon.sprite = GetDamageTypeSprite(data.damageType);

        foreach(var mod in carryWeaponModifierStats)
        {
            mod.text = "";
            mod.gameObject.SetActive(false);
        }

        int i = 0;
        if(SetFireModeModifier(data.fireModeModifier, carryWeaponModifierStats[i]))
            i++;

        SetProjectileModifiers(data.projectileModifiers, carryWeaponModifierStats, i);
    }

    public void SetGroundWeapon(WeaponData data)
    {
        // TODO: Set icon

        carryWeaponDamageStat.text = data.damageAmount.ToString();

        float accuracy = 1f - Mathf.Clamp01(data.recoil / 0.075f);
        accuracy *= 100f;
        groundWeaponAccuracyStat.text = Mathf.RoundToInt(accuracy).ToString();

        groundWeaponProjectileIcon.sprite = GetProjectileTypeSprite(data.projectileType);
        groundWeaponFireModeIcon.sprite = GetFireModeSprite(data.fireMode);
        groundWeaponDamageTypeIcon.sprite = GetDamageTypeSprite(data.damageType);

        foreach(var mod in groundWeaponModifierStats)
        {
            mod.text = "";
            mod.gameObject.SetActive(false);
        }

        int i = 0;
        if(SetFireModeModifier(data.fireModeModifier, groundWeaponModifierStats[i]))
            i++;

        SetProjectileModifiers(data.projectileModifiers, groundWeaponModifierStats, i);
    }

    public bool SetFireModeModifier(FireModeModifier fireModeModifier, TextMeshProUGUI modifierStat)
    {
        switch(fireModeModifier)
        {
            case FireModeModifier.DualSplit:
                modifierStat.gameObject.SetActive(true);
                modifierStat.text = "Dual Split";
                return true;
            case FireModeModifier.TrippleSplit:
                modifierStat.gameObject.SetActive(true);
                modifierStat.text = "Triple Split";
                return true;
            case FireModeModifier.ClusterFire:
                modifierStat.gameObject.SetActive(true);
                modifierStat.text = "Cluster Fire";
                return true;
            case FireModeModifier.RampUpFire:
                modifierStat.gameObject.SetActive(true);
                modifierStat.text = "Ramp Up";
                return true;
            default:
                return false;
        }
    }

    public void SetProjectileModifiers(
        List<ProjectileModifier> modifiers,
        TextMeshProUGUI[] modifierStats,
        int modifierStatOffset)
    {
        int i = modifierStatOffset;
        foreach(var projMod in modifiers)
        {
            switch(projMod)
            {
                case ProjectileModifier.ClusterOnHit:
                    modifierStats[i].gameObject.SetActive(true);
                    modifierStats[i].text = "Cluster On Hit";
                    i++;
                    break;
                case ProjectileModifier.ExplodeOnHit:
                    modifierStats[i].gameObject.SetActive(true);
                    modifierStats[i].text = "Explode On Hit";
                    i++;
                    break;
                case ProjectileModifier.Penetrate:
                    modifierStats[i].gameObject.SetActive(true);
                    modifierStats[i].text = "Penetrate";
                    i++;
                    break;
                case ProjectileModifier.Ricochet:
                    modifierStats[i].gameObject.SetActive(true);
                    modifierStats[i].text = "Ricochet";
                    i++;
                    break;
            }
        }
    }

    Sprite GetFireModeSprite(FireMode fireMode)
    {
        switch(fireMode)
        {
            case FireMode.SemiAuto:
                return semiAutoIcon;
            case FireMode.BurstFire:
                return burstFireIcon;
            case FireMode.FullAuto:
                return fullAutoIcon;
            default:
                return null;
        }
    }

    Sprite GetProjectileTypeSprite(ProjectileType projectileType)
    {
        switch(projectileType)
        {
            case ProjectileType.Ballistic:
                return ballisticIcon;
            case ProjectileType.EnergyBeam:
                return plasmaIcon;
            case ProjectileType.Rocket:
                return rocketIcon;
            default:
                return null;
        }
    }

    Sprite GetDamageTypeSprite(DamageType damageType)
    {
        switch(damageType)
        {
            case DamageType.Normal:
                return normalDmgIcon;
            case DamageType.Fire:
                return fireDmgIcon;
            case DamageType.Energy:
                return energyDmgIcon;
            case DamageType.Shock:
                return shockDmgIcon;
            default:
                return null;
        }
    }

    public void SetPickupIndicatorFillBar(float value)
    {
        pickupIndicator.fillAmount = value;
    }
}
