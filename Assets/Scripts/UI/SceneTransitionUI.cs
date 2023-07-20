using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SOGameEventSystem;

public class SceneTransitionUI : Singleton<SceneTransitionUI>
{
    [Header("Mission Intro")]
    public TMPro.TextMeshProUGUI missionIntroText;
    [TextAreaAttribute]
    public string missionIntroTextFormat;

    [Header("Player Death")]
    public TMPro.TextMeshProUGUI deathText;
    [TextAreaAttribute]
    public string deathTextFormat;

    [Header("Events")]
    public BaseGameEvent onGameplayReady;

    Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();

        SetMissionIntroText(Map_Conditions.Instance.activeCard.cardName);
    }

    // Update is called once per frame
    void Update()
    {
        if(GameManager.Instance.PlayerIsDead)
        {
            if(anim.GetCurrentAnimatorStateInfo(0).IsName("Gameplay"))
            {
                SetDeathText(
                GameManager.Instance.playerData.kills,
                GameManager.Instance.playerData.credits,
                GameManager.Instance.playerData.creditsSaved);
                anim.SetTrigger("PlayerDeath");
            }
        }
    }

    public void EnteredGameplay()
    {
        onGameplayReady.Raise();
    }

    public void DeathFinished()
    {
        ScenesManager.Instance.LoadScene(1);
    }

    public void SetDeathText(
        int killCount,
        int credits,
        int creditsDeposited)
    {
        deathText.text = string.Format(
            deathTextFormat,
            killCount,
            credits,
            creditsDeposited);
    }

    public void SetMissionIntroText(
        string missionTitle)
    {
        missionIntroText.text = string.Format(
            missionIntroTextFormat,
            missionTitle);
    }
}
