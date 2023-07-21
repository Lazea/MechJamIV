using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitController : MonoBehaviour
{
    public Player player;
    public Collider col;
    CardHand cards;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<Player>();
        cards = FindObjectOfType<CardHand>();
        col = GetComponent<Collider>();

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
            StartCoroutine(cards.showCards());
        }
        else if(!col.bounds.Contains(player.transform.position) && cards.isShowing)
        {
            Debug.Log("[EXIT] Exit bounds");
            cards.isShowing = false;
            //StartCoroutine(cards.showCards());
        }
    }
}
