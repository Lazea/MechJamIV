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
    }

    // Update is called once per frame
    void Update()
    {
        if (col.bounds.Contains(player.transform.position) && !cards.isShowing)
        {
            Debug.Log("ENTER");
            cards.isShowing = true;
            StartCoroutine(cards.showCards());
        }
    }
}
