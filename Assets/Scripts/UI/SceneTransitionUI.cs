using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SOGameEventSystem;

public class SceneTransitionUI : Singleton<SceneTransitionUI>
{
    UI_Aesthetics ui;
    UI_MenuDirector mDirector;

    public UI_HeadElement messageScreen;
    public UI_HeadElement gameScreen;
    public UI_HeadElement pauseScreen;
    public UI_HeadElement cardHand;
    public CanvasGroup mGroup;
    public CanvasGroup dGroup;

    [Header("Mission Intro")]
    public UI_SubElement messageTitle;
    public UI_SubElement messageBody;

    [Header("Player Death")]
    [TextAreaAttribute]
    public string deathTextFormat;

    [Header("Events")]
    public BaseGameEvent onGameplayReady;


    private void Start()
    {
        ui = FindObjectOfType<UI_Aesthetics>();
        mDirector = GetComponent<UI_MenuDirector>();

        SetMissionIntroText(Map_Conditions.Instance.activeCard.cardName);

        messageTitle.t.color = Color.white;
        ui.StartCoroutine(ui.SwapPanel(mDirector, messageScreen));
        StartCoroutine(FadeIn());
    }

    // Update is called once per frame
    void Update()
    {
        if(GameManager.Instance.PlayerIsDead)
        {
            /*
            if(anim.GetCurrentAnimatorStateInfo(0).IsName("Gameplay"))
            {
                SetDeathText(
                GameManager.Instance.playerData.kills,
                GameManager.Instance.playerData.credits,
                GameManager.Instance.playerData.creditsSaved);
                anim.SetTrigger("PlayerDeath");
            }*/
        }
    }

    IEnumerator FadeIn()
    {
        ui.StartCoroutine(ui.DisplayText(messageTitle, ui.messageRead, 1));
        ui.StartCoroutine(ui.DisplayText(messageBody, ui.messageRead, 2));

        ui.StartCoroutine(ui.Fade(mGroup, true, 1));

        yield return new WaitForSecondsRealtime(5);

        ui.StartCoroutine(ui.Fade(mGroup, false, 1));

        yield return new WaitForSecondsRealtime(1);

        ui.StartCoroutine(ui.SwapPanel(mDirector, gameScreen));

        yield return new WaitForSecondsRealtime(1);

        EnteredGameplay();
    }
    

    public void EnteredGameplay()
    {
        onGameplayReady.Raise();
    }

    public void DeathFinished()
    {
        ScenesManager.Instance.LoadScene(1);
    }

    IEnumerator DeathEvent()
    {
        messageTitle.t.text = "";
        messageBody.t.text = "";

        ui.StartCoroutine(ui.SwapPanel(mDirector, messageScreen));

        ui.StartCoroutine(ui.Fade(dGroup, true, 5));

        messageTitle.t.color = ui.warnColor;
        messageTitle.t.text = "YOU DIED";
        messageBody.t.text = string.Format(deathTextFormat, GameManager.Instance.playerData.kills, GameManager.Instance.playerData.credits, GameManager.Instance.playerData.creditsSaved);

        ui.StartCoroutine(ui.DisplayText(messageTitle, ui.messageRead, 2));
        ui.StartCoroutine(ui.DisplayText(messageBody, ui.messageRead, 3));

        

        yield return new WaitForSecondsRealtime(10);

        DeathFinished();
    }

    public void StartDeathEvent()
    {
        StartCoroutine(DeathEvent());
    }

    public void StartPause()
    {
        
        ui.StartCoroutine(ui.SwapPanel(mDirector, pauseScreen));
    }

    public void ResumeGame()
    {
        ui.StartCoroutine(ui.SwapPanel(mDirector, gameScreen));
    }

    public void SetMissionIntroText(string missionTitle)
    {
        messageTitle.t.text = missionTitle;

        //replace later
        messageBody.t.text = "destroy all enemies to proceed";

    }
}
