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

    [Header("Ground Weapon UI Elements")]
    public Image groundWeaponIcon;
    public TextMeshProUGUI groundWeaponName;
    public TextMeshProUGUI groundWeaponDamageStat;
    public TextMeshProUGUI groundWeaponAccuracyStat;
    public Image groundWeaponProjectileIcon;
    public Image groundWeaponDamageTypeIcon;
    public Image groundWeaponFireModeIcon;

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

        // TODO: Set projectile and fore mode modifiers
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

        // TODO: Set projectile and fore mode modifiers
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
}
