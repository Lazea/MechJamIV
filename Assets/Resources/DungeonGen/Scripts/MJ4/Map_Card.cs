using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Map Card", menuName = "Scriptable Objects/MapCard")]
public class Map_Card : ScriptableObject
{
    [Header("Name of map")]
    public string cardName;

    [Header("tileset for next map, leave blank to pick on instantiation")]
    public Tileset_Info tileset;
    [Header("environment for next map, leave blank to pick at instantiation")]
    public Tileset_ENV env;

    [Header("hide environmental data in map screen")]
    public bool obscureTileset;

    [Header("number to stack on top of default seed length")]
    public int lengthMod = 0;

    [Header("Number to stack on top of default seondary passes")]
    public int secondaryMod = 0;

    [Header("after how many levels will this card be available")]
    public int lvlReq;

    [Header("intensity of enemy encounters")]
    [Range(0, 100)]
    public float hostility;

    [Header("richness of loot")]
    [Range(0, 100)]
    public float richness;
}
