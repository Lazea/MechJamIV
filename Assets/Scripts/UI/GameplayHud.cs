using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameplayHud : Singleton<GameplayHud>
{
    [Header("Health & Shield & Vertical Thrusters")]
    public Image healthBar;
    public TextMeshProUGUI healthText;
    public Image shieldBar;
    public TextMeshProUGUI shieldText;
    public Image verticalThrustersFuel;

    [Header("Damage Indicator")]
    public Transform damageIndicatorParent;
    public GameObject damageIndicatorPrefab;

    [Header("Weapon")]
    public Image weaponIcon;

    [Header("Abilities")]
    public Image abilityAIcon;
    public Image abilityACooldownFill;
    public Image abilityBIcon;
    public Image abilityBCooldownFill;
    public Image abilityCIcon;
    public Image abilityCCooldownFill;

    [Header("Credits & Stats Info")]
    public TextMeshProUGUI creditsText;
    public TextMeshProUGUI stageCountText;
    public TextMeshProUGUI killCountText;
    public TextMeshProUGUI timerText;
    float tm;

    GameManager Manager { get { return GameManager.Instance; } }
    PlayerData PlayerData { get { return Manager.playerData; } }

    // Start is called before the first frame update
    void Start()
    {
        UpdateStageCount();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateCreditsCount();
        UpdateKillCount();
        tm += Time.deltaTime;
        UpdateStageTimeCount();
    }

    #region [Health & Shield & Vert Thrusters Setters]
    public void SetHealth(int health)
    {
        float amount = health / (float)PlayerData.MaxHealth;
        healthBar.fillAmount = amount;
        healthText.text = health.ToString("D4");
    }

    public void SetShield(int shield)
    {
        float amount = shield / (float)PlayerData.MaxShield;
        shieldBar.fillAmount = amount;
        shieldText.text = shield.ToString("D4");
    }

    public void SetVerticalThrustersFuel(float fuel)
    {
        float amount = fuel / PlayerData.maxVerticalThrusterFuel;
        verticalThrustersFuel.fillAmount = amount;
    }
    #endregion

    #region [Icon Setters]
    public void SetWeaponIcon(Sprite icon)
    {
        weaponIcon.sprite = icon;
    }

    public void SetAbilityAIcon(Sprite icon)
    {
        abilityAIcon.sprite = icon;
    }

    public void SetAbilityBIcon(Sprite icon)
    {
        abilityBIcon.sprite = icon;
    }

    public void SetAbilityCIcon(Sprite icon)
    {
        abilityCIcon.sprite = icon;
    }
    #endregion

    #region [Ability Cooldown Setters]
    public void SetAbilityACooldownFill(float value)
    {
        abilityACooldownFill.fillAmount = value;
    }

    public void SetAbilityBCooldownFill(float value)
    {
        abilityBCooldownFill.fillAmount = value;
    }

    public void SetAbilityCCooldownFill(float value)
    {
        abilityCCooldownFill.fillAmount = value;
    }
    #endregion

    #region [Credits, Kills, and Stats Setters]
    public void UpdateCreditsCount()
    {
        SetCreditsCount(PlayerData.credits);
    }

    public void SetCreditsCount(int value)
    {
        creditsText.text = string.Format(
            "Credits: {0}",
            value.ToString("D6"));
    }

    public void UpdateStageCount()
    {
        SetStageCount(PlayerData.stageCount);
    }

    public void SetStageCount(int value)
    {
        stageCountText.text = string.Format(
            "Stage: {0}",
            value.ToString("D3"));
    }

    public void UpdateKillCount()
    {
        SetKillCount(GameManager.Instance.playerData.kills);
    }

    public void SetKillCount(int value)
    {
        killCountText.text = string.Format(
            "Kills: {0}",
            value.ToString("D4"));
    }

    public void UpdateStageTimeCount()
    {
        float minutes = Mathf.FloorToInt(tm / 60);
        float seconds = Mathf.FloorToInt(tm % 60);
        timerText.text = string.Format(
            "Timer: {0}",
            string.Format("{0:00}:{1:00}", minutes, seconds));
    }
    #endregion

    public void SpawnPlayerDamageIndicator(Damage damage)
    {
        var dmgIndicator = Instantiate(
            damageIndicatorPrefab,
            damageIndicatorParent).GetComponent<PlayerDamageIndicator>();
        if (damage.source != null)
            dmgIndicator.target = damage.source.transform;
    }
}
