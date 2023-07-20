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

    public BaseCardReader[] cards;

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
        hand.SetActive(false);
        StartCoroutine(pickCards());
    }

    // Update is called once per frame
    void LateUpdate()
    {
        //if (Input.GetKeyDown(KeyCode.I))
        //{
        //    isShowing = !isShowing;

        //    if (isShowing)
        //        StartCoroutine(showCards());
        //}

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
        }

        int cardsToShow = 0;
        foreach (var c in cards)
            cardsToShow += (c.isShowing) ? 1 : 0;

        if(cardsToShow > 1)
        {
            int count = 0;
            for (int i = 0; i < cards.Length; i++)
            {
                if (cards[i].isShowing)
                {
                    float x = 1f - ((count / (cardsToShow - 1f)) * 2f);
                    float y = -1f * Mathf.Pow(x, 2f) + 1f;

                    RectTransform cardRect = cards[i].GetComponent<RectTransform>();
                    cardRect.localPosition = new Vector3(
                        x * width,
                        y * height,
                        0f);
                    count++;
                }
            }
        }
    }

    IEnumerator pickCards()
    {
        int r = Random.Range(0, (int)(cardPool.Length / 2));
        int rKeep = Random.Range(0, (int)(cardPool.Length / 2));

        for (int i = 0; i < cards.Length; i++)
        {
            CardReader card = cards[i].GetComponent<CardReader>();
            if (card == null)
                continue;

            r = rKeep + r;
            r = r % cardPool.Length;

            card.ReadCard(cardPool[r]);

            if (!containsCard(cardPool[r]))
            {
                rKeep = r;
                r = Random.Range(0, 3);
            }
            else
            {
                while (containsCard(cardPool[r]))
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

        foreach(BaseCardReader c in cards)
        {
            CardReader _c = c.GetComponent<CardReader>();
            if (_c != null)
            {
                if (_c.card == card)
                    test = true;
            }
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

        foreach (BaseCardReader c in cards)
        {
            c.isShowing = false;
        }

        yield return new WaitForSeconds(waitTime);

        hand.SetActive(false);
    }


}
