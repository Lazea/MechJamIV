using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Tileset", menuName = "Scriptable Objects/Tileset")]
public class Tileset_Info : ScriptableObject
{
    [Header("Collection of meshes to use for map tiles")]
    public GameObject tileMaster;

    [Header("viable environments for this tileset")]
    public Tileset_ENV[] env;

    [Header("x/y size of tiles")]
    public float tileSize;
    [Header("number of tiles on x/y axis")]
    public int res = 1;

    [Header("how likely it is that a secondary room will spawn")]
    [Range(0, 1)]
    public float secondaryChance = .5f;
    [Header("how likely it is that adjacent rooms will connect, that were not initially paired")]
    [Range(0, 1)]
    public float interconnectedness = .5f;

    [Header("USED FOR PERLIN NOISE. WIP")]
    [Range(0, 1)]
    public float cutoff = .5f;
    [Range(0, 1)]
    public float scale = .5f;

    [Header("how long chains of roads can be")]
    public int roadLength;
    [Header("how many new roads will be planted")]
    public int roadSeeds;



    [Header("Should the map be perfectly square by filling empty spaces")]
    public bool fillVoids;
    [Header("should interior walls use a different tileset")]
    public bool useSiblingTiles;

}
