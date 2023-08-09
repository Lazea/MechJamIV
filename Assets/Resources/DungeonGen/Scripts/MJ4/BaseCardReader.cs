using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System;

public class BaseCardReader : MonoBehaviour
{
    protected RectTransform r;

    protected Image panel;
    protected CanvasGroup c;

    public bool isShowing;


    protected virtual void Awake()
    {
        panel = GetComponentInChildren<Image>();
        c = GetComponent<CanvasGroup>();
        r = GetComponent<RectTransform>();
        c.alpha = 0;
    }


    protected virtual void Update()
    {
       // if (isHovering && Input.GetButtonDown("Fire1"))
           // SelectCard();
    }


    //HANDLE LEVEL ITERATION HERE--------------------------------------------------------------------------------------------------------------------------
    protected virtual void SelectCard()
    {
        throw new NotImplementedException();
    }

    public static void SetActiveCard(Map_Conditions map, Map_Card card, Tileset_ENV env)
    {
        Debug.LogFormat(
            "Setting Active Card {0} with seed length {1} and seed secondary length {2}",
            card.cardName,
            card.lengthMod,
            card.secondaryMod);

        //swap tilesets from selected card, to held card
        map.activeCard.cardName = card.cardName;
        map.activeCard.tileset = card.tileset;
        map.activeCard.env = env;

        map.activeCard.lengthMod = Mathf.Max(card.lengthMod, 1);
        map.activeCard.secondaryMod = card.secondaryMod;
        //add starting length values to held card. Prevent from taking length below default value
        //if (map.startingLength + (map.activeCard.lengthMod + card.lengthMod) > map.startingLength)
        //    map.activeCard.lengthMod += card.lengthMod;
        //else
        //    map.activeCard.lengthMod = 0;

        //if (map.startingSecondary + (map.activeCard.secondaryMod + card.secondaryMod) > 0)
        //    map.activeCard.secondaryMod += card.secondaryMod;
        //else
        //    map.activeCard.secondaryMod = 0;

        //add difficulty settings
        map.activeCard.enemyWaveCount = card.enemyWaveCount;
        map.activeCard.npcWavePool = card.npcWavePool;
        map.activeCard.maxRarity = card.maxRarity;
        map.activeCard.lootDropChance = card.lootDropChance;

        //FURTHER LEVEL ITERATION SHOULD GO RIGhT HERE!
        //VV


        //^^
    }

}
