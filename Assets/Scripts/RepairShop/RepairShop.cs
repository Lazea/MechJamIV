using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RepairShop : MonoBehaviour
{
    public PlayerData playerData;
    UI_Aesthetics ui;
    UI_MenuDirector menu;

    public int repairAmount = 35;
    public int repairCost = 200;
    public float depositInterest = 0.2f;

    string creditsTextFormat;
    public TextMeshProUGUI creditsText;
    string creditsSavedTextFormat;
    public TextMeshProUGUI creditsSavedText;

    public bool repairLocked;
    string mechHPTextFormat;
    public TextMeshProUGUI mechHPText;
    string mechRepairTextFormat;
    public TextMeshProUGUI mechRepairText;

    public bool depositLocked;
    string creditsDepositTextFormat;
    public TextMeshProUGUI creditsDepositText;

    [Header("Audio Clips")]
    public AudioClip repairConfirmClip;
    public AudioClip depositConfirmClip;

    // Start is called before the first frame update
    void Start()
    {
        creditsTextFormat = creditsText.text;
        creditsSavedTextFormat = creditsSavedText.text;
        mechHPTextFormat = mechHPText.text;
        mechRepairTextFormat = mechRepairText.text;
        creditsDepositTextFormat = creditsDepositText.text;
        ui = FindObjectOfType<UI_Aesthetics>();
        menu = FindObjectOfType<UI_MenuDirector>();


        UpdateCreditsText();
        UpdateMechHPText();
        UpdateCreditsDepositText();

        StartCoroutine(ui.SwapPanel(menu, menu.UIMajor[0]));
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateCreditsText()
    {
        creditsText.text = string.Format(creditsTextFormat, playerData.credits.ToString("D6"));
        creditsSavedText.text = string.Format(creditsSavedTextFormat, playerData.creditsSaved.ToString("D6"));
    }

    public void UpdateMechHPText()
    {
        mechHPText.text = string.Format(mechHPTextFormat, playerData.health.ToString("D3"), playerData.MaxHealth.ToString("D3"));


        if (playerData.credits > repairCost)
        {
            if (repairLocked)
                mechRepairText.text = "MECH REPAIRED. SERVICES UNAVAILABLE";
            else
                mechRepairText.text = string.Format(mechRepairTextFormat, repairAmount.ToString(), repairCost.ToString());
        }
        else
            mechRepairText.text = "NOT ENOUGH CREDITS";
    }

    public void UpdateCreditsDepositText()
    {
        if (playerData.credits > 0)
        {
            if (depositLocked)
            {
                creditsDepositText.text = "CREDITS DEPOSITED. TERMINAL LOCKED";
            }
            else
            {
                int pay = Mathf.RoundToInt(playerData.credits * 0.2f);
                int remaining = playerData.credits - pay;
                creditsDepositText.text = string.Format(creditsDepositTextFormat, pay.ToString("D6"), remaining.ToString("D6"), Mathf.RoundToInt(depositInterest * 100f).ToString());
            }
        }
        else
            creditsDepositText.text = "NO CREDITS";
    }

    public void RepairMech()
    {
        
        if (repairLocked)
            return;

        if((playerData.credits - repairCost) < 0)
            return;

        if (playerData.health >= playerData.MaxHealth)
            return;

        playerData.health = Mathf.Min(
            playerData.health + repairAmount,
            playerData.MaxHealth);
        playerData.credits = playerData.credits - repairCost;

        repairLocked = true;

        UpdateCreditsDepositText();
        UpdateCreditsText();
        UpdateMechHPText();
    }

    public void DepositCredits()
    {
        
        if(depositLocked)
            return;

        int pay = Mathf.RoundToInt(playerData.credits * 0.2f);

        if (pay <= 0)
            return;
        
        
        int remaining = playerData.credits - pay;
        playerData.creditsSaved += remaining;
        playerData.credits = 0;

        depositLocked = true;

        UpdateCreditsText();
        UpdateCreditsDepositText();
        UpdateMechHPText();
    }

    public void SaveRemainingCredits()
    {
        playerData.creditsSaved += playerData.credits;
        playerData.credits = 0;
    }

    public void PlayClip(AudioClip clip)
    {
        UIAudioManager.Instance.AudioSource.PlayOneShot(clip);
    }
}
