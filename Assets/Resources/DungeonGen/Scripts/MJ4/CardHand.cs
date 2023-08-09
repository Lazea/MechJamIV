using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CardHand : MonoBehaviour
{
    Map_Conditions map;
    public bool isShowing;
    bool _isShowing;

    public GameObject hand;

    public CardReader[] cards;

    public Map_Card[] cardPool;

    [Header("Card Positions")]
    public float height = 10f;
    public float width = 600f;

    [Header("Events")]
    public UnityEvent onCardsShowing;
    public UnityEvent onCardsHiding;

    // Start is called before the first frame update
    void Start()
    {
        map = FindObjectOfType<Map_Conditions>();
        //hand.SetActive(false);
        //StartCoroutine(pickCards());
    }

    // Update is called once per frame
    void LateUpdate()
    {
        /*
        if(isShowing != _isShowing)
        {
            _isShowing = isShowing;
            if(_isShowing)
            {
                onCardsShowing.Invoke();
            }
            else
            {
                onCardsHiding.Invoke();
            }
        }*/
    }

    public void PickCards()
    {
        StartCoroutine(pickCards());
    }

    IEnumerator pickCards()
    {
        for (int i = 0; i < cards.Length; i++)
        {
            int r = Random.Range(0, cardPool.Length);

            CardReader card = cards[i].GetComponent<CardReader>();

            card.ReadCard(cardPool[r]);
        }

        yield return null;
    }
    /*
    public IEnumerator showCards()
    {
        float waitTime = .15f;

        hand.SetActive(true);

        while (isShowing)
        {
            for (int i = 0; i < cards.Length; i++)
            {
                if(!cards[i].isShowing)
                    StartCoroutine(cards[i].FadeIn());

                yield return new WaitForSeconds(waitTime);
            }

            yield return null;
        }

        foreach (BaseCardReader c in cards)
        {
            c.isShowing = false;
        }

        yield return new WaitForSeconds(waitTime);

        hand.SetActive(false);
    }

    */
}
