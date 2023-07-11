using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PickupMenuUI : MonoBehaviour
{
    public GameObject crosshair;

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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnEnable()
    {
        crosshair.SetActive(false);
    }

    private void OnDisable()
    {
        crosshair.SetActive(true);
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

        // TODO: Set projectile and fore mode modifiers
    }

    public void SetGroundWeapon(WeaponData data)
    {
        // TODO: Set icon

        carryWeaponDamageStat.text = data.damageAmount.ToString();

        float accuracy = 1f - Mathf.Clamp01(data.recoil / 0.075f);
        accuracy *= 100f;
        groundWeaponAccuracyStat.text = Mathf.RoundToInt(accuracy).ToString();

        // TODO: Set projectile and fore mode modifiers
    }
}
