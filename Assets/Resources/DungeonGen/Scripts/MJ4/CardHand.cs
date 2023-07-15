using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardHand : MonoBehaviour
{
    Map_Conditions map;
    public bool isShowing;

    public GameObject hand;

    public CardReader[] cards;

    public Map_Card[] cardPool;

    // Start is called before the first frame update
    void Start()
    {
        map = FindObjectOfType<Map_Conditions>();
        hand.SetActive(false);
        StartCoroutine(pickCards());
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            isShowing = !isShowing;

            if (isShowing)
                StartCoroutine(showCards());
        }

    }

    IEnumerator pickCards()
    {
        int r = Random.Range(0, (int)(cardPool.Length / 2));
        int rKeep = Random.Range(0, (int)(cardPool.Length / 2));

        for (int i = 0; i < cards.Length; i++)
        {
            r = rKeep + r;
            r = r % cardPool.Length;

            cards[i].ReadCard(cardPool[r]);

            if (!containsCard(cardPool[r]))
            {
                rKeep = r;
                r = Random.Range(0, 3);
            }
            else
            {
                while(containsCard(cardPool[r]))
                {
                    r++;
                    r = r % cardPool.Length;

                    yield return null;
                }
            }

            
        }

        yield return null;
    }

    bool containsCard(Map_Card card)
    {
        bool test = false;

        foreach(CardReader c in cards)
        {
            if (c.card == card)
                test = true;
        }

        if (card != map.activeCard)
            test = true;

        return test;
    }

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


        foreach(CardReader c in cards)
        {
            c.isShowing = false;
        }

        yield return new WaitForSeconds(waitTime);

        hand.SetActive(false);
    }


}
