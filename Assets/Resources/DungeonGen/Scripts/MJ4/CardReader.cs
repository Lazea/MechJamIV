using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardReader : BaseCardReader
{
    protected Map_Conditions map;
    public Map_Card card;
    protected Tileset_ENV env;

    public TextMeshProUGUI missionName;
    public TextMeshProUGUI loc;
    public TextMeshProUGUI length;
    public TextMeshProUGUI conditions;

    protected override void Awake()
    {
        base.Awake();

        map = FindObjectOfType<Map_Conditions>();
        ReadCard(card);
    }

    //HANDLE LEVEL ITERATION HERE--------------------------------------------------------------------------------------------------------------------------
    protected override void SelectCard()
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
}
