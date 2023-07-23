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
    public Image carryWeaponUI;
    public Image carryWeaponIcon;
    public TextMeshProUGUI carryWeaponName;
    public TextMeshProUGUI carryWeaponDamageStat;
    public TextMeshProUGUI carryWeaponAccuracyStat;
    public Image carryWeaponProjectileIcon;
    public Image carryWeaponDamageTypeIcon;
    public Image carryWeaponFireModeIcon;
    public TextMeshProUGUI[] carryWeaponModifierStats;

    [Header("Ground Weapon UI Elements")]
    public Image groundWeaponUI;
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

    [Header("Rarity Colors")]
    public Color commonColor;
    public Color uncommonColor;
    public Color rareColor;
    public Color legendaryColor;

    [Header("Audio Settings")]
    public AudioSource audioSource;
    public AudioClip displayClip;
    public AudioClip interactTickClip;
    public int totalTickCount;
    int tickCount;
    public float minTickPitch;
    public float maxTickPitch;
    bool excludeFirstEnable = true;
    bool excludeFirstDisable = true;

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
        if(excludeFirstEnable)
        {
            excludeFirstEnable = false;
            return;
        }

        //crosshair.SetActive(false);
        pickupInfoParent.SetActive(true);
        audioSource.pitch = 1f;
        audioSource.PlayOneShot(displayClip);
    }

    private void OnDisable()
    {
        if (excludeFirstDisable)
        {
            excludeFirstDisable = false;
            return;
        }

        //crosshair.SetActive(true);
        pickupInfoParent.SetActive(false);
        audioSource.pitch = 0.8f;
        audioSource.PlayOneShot(displayClip);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetCarryWeapon(WeaponData data)
    {
        SetWeaponInfo(
            data,
            carryWeaponUI,
            carryWeaponName,
            carryWeaponDamageStat,
            carryWeaponAccuracyStat,
            carryWeaponModifierStats,
            carryWeaponProjectileIcon,
            carryWeaponFireModeIcon,
            carryWeaponDamageTypeIcon);
    }

    public void SetGroundWeapon(WeaponData data)
    {
        SetWeaponInfo(
            data,
            groundWeaponUI,
            groundWeaponName,
            groundWeaponDamageStat,
            groundWeaponAccuracyStat,
            groundWeaponModifierStats,
            groundWeaponProjectileIcon,
            groundWeaponFireModeIcon,
            groundWeaponDamageTypeIcon);
    }

    public void SetWeaponInfo(
        WeaponData data,
        Image weaponUI,
        TextMeshProUGUI weaponNameText,
        TextMeshProUGUI weaponDamageStat,
        TextMeshProUGUI weaponAccuracyStat,
        TextMeshProUGUI[] weaponModifierStats,
        Image weaponProjectileIcon,
        Image weaponFireModeIcon,
        Image weaponDamageTypeIcon)
    {
        switch(data.rarity)
        {
            case Rarity.Common:
                weaponNameText.text = "Common";
                weaponUI.color = commonColor;
                break;
            case Rarity.Uncommon:
                weaponNameText.text = "Unommon";
                weaponUI.color = uncommonColor;
                break;
            case Rarity.Rare:
                weaponNameText.text = "Rare";
                weaponUI.color = rareColor;
                break;
            case Rarity.Legendary:
                weaponNameText.text = "Legendary";
                weaponUI.color = legendaryColor;
                break;
        }

        int shotsPerSecond = Mathf.RoundToInt(1f / data.baseFireRate);
        if (data.fireMode == FireMode.BurstFire)
            shotsPerSecond *= 3;
        switch (data.fireModeModifier)
        {
            case FireModeModifier.Cluster:
                shotsPerSecond *= 6;
                break;
            case FireModeModifier.DualSplit:
                shotsPerSecond *= 2;
                break;
            case FireModeModifier.TrippleSplit:
                shotsPerSecond *= 3;
                break;
        }    
        weaponDamageStat.text = (shotsPerSecond * data.baseDamage).ToString();

        float accuracy = 1f - Mathf.Clamp01(data.baseAccuracy / 0.1f);
        accuracy *= 100f;
        weaponAccuracyStat.text = string.Format(
            "{0}%",
            Mathf.RoundToInt(accuracy).ToString());

        weaponProjectileIcon.sprite = GetProjectileTypeSprite(data.projectileType);
        weaponFireModeIcon.sprite = GetFireModeSprite(data.fireMode);
        weaponDamageTypeIcon.sprite = GetDamageTypeSprite(data.damageType);

        foreach (var mod in weaponModifierStats)
        {
            mod.text = "";
            mod.gameObject.SetActive(false);
        }

        int i = 0;
        if (SetFireModeModifier(data.fireModeModifier, weaponModifierStats[i]))
            i++;

        SetProjectileModifiers(data.projectileModifiers, weaponModifierStats, i);
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
            case FireModeModifier.Cluster:
                modifierStat.gameObject.SetActive(true);
                modifierStat.text = "Cluster";
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
            case ProjectileType.Plasma:
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
        if(pickupInfoParent.activeSelf)
        {
            audioSource.pitch = Mathf.Lerp(
            minTickPitch,
            maxTickPitch,
            value);

            if (value >= 1f)
            {
                audioSource.PlayOneShot(interactTickClip);
                tickCount = 0;
            }
            else
            {
                float tickInterval = 1f / totalTickCount;
                if (value >= tickInterval * tickCount)
                {
                    audioSource.PlayOneShot(interactTickClip);
                    tickCount++;
                }
            }
        }

        pickupIndicator.fillAmount = value;
    }
}
