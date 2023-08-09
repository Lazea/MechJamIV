using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitController : MonoBehaviour
{
    public Player player;
    public Collider col;
    CardHand cards;

    UI_Aesthetics uiA;
    SceneTransitionUI sceneUI;

    // Start is called before the first frame update
    void Awake()
    {
        player = FindObjectOfType<Player>();
        cards = FindObjectOfType<CardHand>();
        col = GetComponent<Collider>();

        uiA = FindObjectOfType<UI_Aesthetics>();
        sceneUI = FindObjectOfType<SceneTransitionUI>();

    }

    private void Start()
    {
        enabled = NPCsManager.Instance.IsCombatCompleted();

    }

    // Update is called once per frame
    void Update()
    {
        if (player == null)
            return;

        if (col.bounds.Contains(player.transform.position) && !cards.isShowing)
        {
            Debug.Log("[ENTER] Exit bounds");
            cards.isShowing = true;
            cards.onCardsShowing.Invoke();
            //StartCoroutine(cards.showCards());

            uiA.SwapIstant(sceneUI.cardHand);
        }
        else if(!col.bounds.Contains(player.transform.position) && cards.isShowing)
        {
            Debug.Log("[EXIT] Exit bounds");
            cards.isShowing = false;
            cards.onCardsHiding.Invoke();

            uiA.SwapIstant(sceneUI.gameScreen);
            //StartCoroutine(cards.showCards());
        }
    }
}
