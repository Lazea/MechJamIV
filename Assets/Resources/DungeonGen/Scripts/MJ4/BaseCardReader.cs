using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System;

public class BaseCardReader : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler
{
    protected RectTransform r;

    protected Image panel;
    protected CanvasGroup c;

    protected bool isHovering;
    public bool isShowing;

    protected float hoverScale = 1.25f;
    protected float hoverTime = .08f;
    protected float startScale = .75f;
    protected float fadeTime = .125f;

    protected Color startColor;
    public Color flashColor = new Color(1, 1, 1, .125f);

    protected virtual void Awake()
    {
        panel = GetComponent<Image>();
        startColor = panel.color;
        c = GetComponent<CanvasGroup>();
        r = GetComponent<RectTransform>();

        startScale = r.localScale.x;
        c.alpha = 0;
    }


    protected virtual void Update()
    {
        if (isHovering && Input.GetButtonDown("Fire1"))
            SelectCard();
    }


    //HANDLE LEVEL ITERATION HERE--------------------------------------------------------------------------------------------------------------------------
    protected virtual void SelectCard()
    {
        throw new NotImplementedException();
    }

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        if (!isHovering)
            StartCoroutine(HoverCard());
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
    }


    //logic for hovering over card
    public virtual IEnumerator HoverCard()
    {
        isHovering = true;
        float hT = 0;
        float hPeriod = 0;

        Color fCurrent = flashColor;

        while ((hPeriod < hoverTime && isHovering) || isHovering)
        {
            if (hPeriod < hoverTime)
            {
                hPeriod += Time.deltaTime;
                hT = hPeriod / hoverTime;

                float s = Mathf.Lerp(startScale, startScale * hoverScale, hT);
                r.localScale = new Vector3(1, 1, 1) * s;

                panel.color = Color.Lerp(startColor, flashColor, Mathf.Sin((hT * 180) * Mathf.Deg2Rad));

                fCurrent = panel.color;
            }

            yield return null;
        }


        while (hT > 0)
        {
            hPeriod -= Time.deltaTime;
            hT = hPeriod / hoverTime;

            float s = Mathf.Lerp(startScale, startScale * hoverScale, hT);
            r.localScale = new Vector3(1, 1, 1) * s;

            panel.color = Color.Lerp(fCurrent, flashColor, Mathf.Sin((hT * 180) * Mathf.Deg2Rad));


            yield return null;
        }


        panel.color = startColor;
        r.localScale = new Vector3(1, 1, 1) * startScale;
    }

    public virtual IEnumerator FadeIn()
    {
        float fT = 0;
        float fPeriod = 0;

        isShowing = true;

        while ((fPeriod < fadeTime && isShowing) || isShowing)
        {
            fPeriod += Time.deltaTime;

            fT = fPeriod / fadeTime;

            c.alpha = fT;

            yield return null;
        }

        while (fPeriod > 0)
        {
            fPeriod -= Time.deltaTime;

            fT = fPeriod / fadeTime;

            c.alpha = fT;

            yield return null;
        }

        c.alpha = 0;
    }
}
