using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class CardReader : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler
{
    Map_Conditions map;
    RectTransform r;
    public Map_Card card;
    Tileset_ENV env;
    Collider exitField;

    Image panel;
    public TextMeshProUGUI missionName;
    public TextMeshProUGUI loc;
    public TextMeshProUGUI length;
    public TextMeshProUGUI conditions;
    CanvasGroup c;

    bool isHovering;
    public bool isShowing;

    float hoverScale = 1.25f;
    float hoverTime = .08f;
    float startScale = .75f;
    float fadeTime = .125f;

    Color startColor;
    public Color flashColor = new Color(1, 1, 1, .125f);

    private void Awake()
    {
        map = FindObjectOfType<Map_Conditions>();
        panel = GetComponent<Image>();
        startColor = panel.color;
        c = GetComponent<CanvasGroup>();
        r = GetComponent<RectTransform>();
        

        startScale = r.localScale.x;
        c.alpha = 0;

        ReadCard(card);
    }


    private void Update()
    {
        //GRIGGY I don't know how you want to handle button events, so I just did getbutton down. Feel free to replace if you want to use your event system instead
        if(isHovering && Input.GetButtonDown("Fire1"))
            SelectCard();
    }


    //HANDLE LEVEL ITERATION HERE--------------------------------------------------------------------------------------------------------------------------
    void SelectCard()
    {
        //swap tilesets from selected card, to held card
        map.activeCard.tileset = card.tileset;
        map.activeCard.env = env;

        //add starting length values to held card. Prevent from taking length below default value;
        if (map.startingLength + (map.activeCard.lengthMod + card.lengthMod) > map.startingLength)
            map.activeCard.lengthMod += card.lengthMod;
        else
            map.activeCard.lengthMod = 0;

        if (map.startingSecondary + (map.activeCard.secondaryMod + card.secondaryMod) > 0)
            map.activeCard.secondaryMod += card.secondaryMod;
        else
            map.activeCard.secondaryMod = 0;

        //FURTHER LEVEL ITERATION SHOULD GO RIGhT HERE!
        //VV


        //^^

        //call on map to regenerate here
        map.newMap();
    }

    //read cards replaces the values in the HUD card readers, with a new card from the pool
    public void ReadCard(Map_Card newCard)
    {
        card = newCard;

        missionName.text = card.cardName;

        string locText = card.obscureTileset ? "unknown" : card.tileset.name;
        loc.text = $"Location: {locText}";

        string lSign = card.lengthMod + card.secondaryMod >= 0 ? "+" : "";
        length.text = $"Length: {lSign}{card.lengthMod + card.secondaryMod}";

        string weather = "clear";

        //if specific env is not specified, then grab env from tileset info
        if (card.env)
            env = card.env;
        else
        {
            int e = Random.Range(0, card.tileset.env.Length);
            env = card.tileset.env[e];
        }

        if (env)
        {
            if (env.weather)
                weather = $"{env.weather.gameObject.name}";
        }

        string wRead = card.obscureTileset ? "unknown" : weather;

        conditions.text = $"Weather: {wRead}";
        
    }

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        if(!isHovering)
            StartCoroutine(HoverCard());
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
    }


    //logic for hovering over card
    public IEnumerator HoverCard()
    {
        isHovering = true;
        float hT = 0;
        float hPeriod = 0;

        Color fCurrent = flashColor;

        while((hPeriod < hoverTime && isHovering) || isHovering)
        {
            if (hPeriod < hoverTime)
            {
                hPeriod += Time.deltaTime;
                hT = hPeriod / hoverTime;

                float s = Mathf.Lerp(startScale, startScale * hoverScale, hT);
                r.localScale = new Vector3(1,1,1) * s;

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
            r.localScale = new Vector3(1,1,1) * s;

            panel.color = Color.Lerp(fCurrent, flashColor, Mathf.Sin((hT * 180) * Mathf.Deg2Rad));


            yield return null;
        }


        panel.color = startColor;
        r.localScale = new Vector3(1, 1, 1) * startScale;
    }

    public IEnumerator FadeIn()
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
