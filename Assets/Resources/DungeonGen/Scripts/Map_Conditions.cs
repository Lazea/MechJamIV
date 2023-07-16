using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.AI;
#if UNITY_EDITOR
using UnityEditor;
#endif



[ExecuteInEditMode]
public class Map_Conditions : MonoBehaviour
{
    NavMeshSurface navSurface;
    [Header("Transform to hold chunks")]
    public Transform chunkShell;

    [Header("Player object")]
    public Transform player;

    Volume post;
    [Header("ambient audio source")]
    public AudioSource ambient;
    [Header("music audio source")]
    public AudioSource music;
    AudioReverbFilter reverb;
    [Header("total number of possible tilesets")]
    public Tileset_Info[] tilesetLibrary;
    [HideInInspector]
    public Tileset_Info tileset;

    [Header("default weather if none is provided. Leave blank to leave clear")]
    public GameObject weather;

    [Header("default env if none is provided.")]
    public Tileset_ENV env;


    [Header("card that should be carried from level to level. Leave as player card!")]
    public Map_Card activeCard;


    Camera cam;

    Dictionary<Vector2Int, ChunkInfo> chunks;
    Dictionary<Vector2Int, ChunkInfo> secondaryChunks;
    Chunk_Controller lastChunk;
    Vector2Int lastPos;

    [HideInInspector]
    public Chunk_Controller startChunk;
    [HideInInspector]
    public Chunk_Controller endChunk;

    [Header("number of rooms in main chain")]
    public int seedLength = 4;
    [HideInInspector]
    public int startingLength;
    [Header("number of attempts to place secondary rooms")]
    public int secondaryLength = 1;
    [HideInInspector]
    public int startingSecondary;

    [Header("Should chunks rotate to fit together.")]
    public bool alignChunks;



    bool isBuilding;

    

    private void Awake()
    {
        chunks = new Dictionary<Vector2Int, ChunkInfo>();
        secondaryChunks = new Dictionary<Vector2Int, ChunkInfo>();
        navSurface = GetComponent<NavMeshSurface>();
        cam = FindObjectOfType<Camera>();
        reverb = cam.GetComponent<AudioReverbFilter>();
        post = GetComponent<Volume>();
        player = FindObjectOfType<Player>().transform;
        startingLength = seedLength;
        startingSecondary = secondaryLength;

        BuildMap();

        navSurface.BuildNavMesh();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    public float GetNoiseValue(Vector3 sample)
    {
        Vector3 scaleSample = sample * tileset.scale;
        float test = Mathf.PerlinNoise(scaleSample.x, scaleSample.z);

        return test;
    }

    public void newMap()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void clearMap()
    {
        activeCard.lengthMod = 0;
        activeCard.secondaryMod = 0;

        if (weather)
        {
            DestroyImmediate(weather);
            weather = null;
        }



        Debug.Log("clear");
    }

    private void OnApplicationQuit()
    {
        clearMap();
    }

    

    public void BuildMap()
    {
        isBuilding = true;

        if(activeCard.tileset)
            tileset = activeCard.tileset;
        else
        {
            int t = Random.Range(0, tilesetLibrary.Length);
            tileset = tilesetLibrary[t];
        }

        if(activeCard.env)
            env = activeCard.env;

        if (weather)
        {
            DestroyImmediate(weather);
            weather = null;
        }

        if (env.ambient)
            ambient.clip = env.ambient;

        if (env.song)
            music.clip = env.song;

        

        if(env.weather)
        {
            weather = Instantiate(env.weather, cam.transform);
        }

        

        seedLength += activeCard.lengthMod;
        secondaryLength += activeCard.secondaryMod;

        reverb.reverbPreset = env.reverb;

        post.profile = env.volume;

        RenderSettings.fogDensity = env.fogLevel;
        RenderSettings.skybox = env.sky;
        RenderSettings.fogColor = env.fogColor;

        if (chunkShell.transform.childCount > 0)
        {
            for (int i = chunkShell.transform.childCount; i > 0; i--)
            {
                DestroyImmediate(chunkShell.GetChild(i - 1).gameObject);
            }

            chunks.Clear();
            secondaryChunks.Clear();
        }



        Vector2Int seedPos = new Vector2Int(0, 0);

        int cCount = 0;

        while (cCount < seedLength)
        {
            bool posFound = false;

            float inc = 0;
            int a = Random.Range(0, 4);

            for (int i = 0; i < 4; i++)
            {
                if(i == a)
                    inc = 90 * i;
            }
            
            for (int i = 0; i < 4; i++)
            {
                inc += 90;

                int x = (int)Mathf.Sin(inc * Mathf.Deg2Rad);
                int y = (int)Mathf.Cos(inc * Mathf.Deg2Rad);

                


                if (!posFound)
                {
                    Vector2Int testPos = seedPos + new Vector2Int(x, y);

                    if (!chunks.ContainsKey(testPos))
                    {
                        posFound = true;
                        seedPos = testPos;



                        if (chunks.Count == 0 && player)
                        {
                            Vector3 pY = Vector3.up * player.position.y;
                            
                            player.position = (new Vector3(seedPos.x, 0, seedPos.y) * tileset.res * tileset.tileSize) + pY;
                        }

                        chunks.Add(seedPos, new ChunkInfo());

                        SpawnChunk(seedPos, chunks, false);
                        
                    }
                }


            }

            cCount++;
        }

        startChunk = chunkShell.GetChild(0).GetComponent<Chunk_Controller>();
        endChunk = chunkShell.GetChild(seedLength - 1).GetComponent<Chunk_Controller>();


        if(secondaryLength > 0)
        {
            


            for (int c = 0; c < secondaryLength; c++)
            {

                int r = Random.Range(0, chunks.Count);
                int rCheck = 0;
                bool rFound = false;

                foreach (Vector2Int v in chunks.Keys)
                {
                    if (r == rCheck && !rFound)
                    {
                        rFound = true;
                        bool posFound = false;

                        float inc = 0;
                        int a = Random.Range(0, 4);

                        for (int i = 0; i < 4; i++)
                        {
                            if (i == a)
                                inc = 90 * i;
                        }

                        for (int i = 0; i < 4; i++)
                        {
                            inc += 90;

                            int x = (int)Mathf.Sin(inc * Mathf.Deg2Rad);
                            int y = (int)Mathf.Cos(inc * Mathf.Deg2Rad);

                            if (!posFound)
                            {
                                Vector2Int testPos = v + new Vector2Int(x, y);

                                if (!chunks.ContainsKey(testPos) && !secondaryChunks.ContainsKey(testPos))
                                {
                                    posFound = true;


                                    secondaryChunks.Add(testPos, new ChunkInfo());

                                    lastChunk = chunks[v].chunk;

                                    SpawnChunk(testPos, secondaryChunks, false);
                                }
                            }
                        }
                    }
                    else
                        rCheck++;

                }
                
                

                if (secondaryChunks.Count > 0)
                {
                    foreach (Vector2Int v in secondaryChunks.Keys)
                    {
                        chunks.Add(v, secondaryChunks[v]);
                    }

                    secondaryChunks.Clear();
                }

                int lChunk = Random.Range(seedLength, chunkShell.childCount);
                lastChunk = chunkShell.GetChild(lChunk).GetComponent<Chunk_Controller>();
            }



        }

        GenerateLinks();
        GenerateRoadLinks();


        if(tileset.fillVoids)
            FillVoids();

        GenerateMap();

        

        isBuilding = false;
    }

    //searches surrounding area for immediately adjacent chunks, and adds a link if conditions are right.
    void GenerateLinks()
    {
        foreach(Vector2Int v in chunks.Keys)
        {
            float r = Random.Range(0f, 1f);

            

            if (r > 1 - tileset.interconnectedness)
            {
                int inc = 0;
                bool linkFound = false;

                for (int i = 0; i < 4; i++)
                {
                    inc += 90;

                    int x = (int)Mathf.Sin(inc * Mathf.Deg2Rad);
                    int y = (int)Mathf.Cos(inc * Mathf.Deg2Rad);

                    if (!linkFound)
                    {
                        Vector2Int testPos = v + new Vector2Int(x, y);

                        

                        if (chunks.ContainsKey(testPos))
                        {
                            
                            if (!CheckSibling(chunks[v].chunk, chunks[testPos].chunk))
                            {
                                
                                linkFound = true;

                                chunks[v].chunk.siblings.Add(new Chunk_Controller.Sibling(chunks[testPos].chunk, testPos - v));
                                chunks[testPos].chunk.siblings.Add(new Chunk_Controller.Sibling(chunks[v].chunk, v - testPos));
                            }
                        }
                    }


                }
            }
        }
    }

    void FillVoids()
    {
        //x is min, y is max
        Vector2 xExtents = new Vector2(Mathf.Infinity, -Mathf.Infinity);
        Vector2 yExtents = new Vector2(Mathf.Infinity, -Mathf.Infinity);

        foreach(Vector2Int v in chunks.Keys)
        {
            //get x extents
            if (v.x < xExtents.x)
                xExtents.x = v.x;

            if (v.x > xExtents.y)
                xExtents.y = v.x;

            //Get y extents
            if (v.y < yExtents.x)
                yExtents.x = v.y;

            if (v.y > yExtents.y)
                yExtents.y = v.y;
        }

        for (int y = (int)yExtents.x; y <= (int)yExtents.y; y++)
        {
            for (int x = (int)xExtents.x; x <= (int)xExtents.y; x++)
            {
                Vector2Int pos = new Vector2Int(x, y);
                
                if (!chunks.ContainsKey(pos))
                {
                    lastChunk = null;
                    chunks.Add(pos, new ChunkInfo());
                    SpawnChunk(pos, chunks, true);
                }
            }
        }
       
    }

    bool CheckSibling(Chunk_Controller testChunk, Chunk_Controller testSib)
    {
        bool test = false;

        foreach(Chunk_Controller.Sibling s in testChunk.siblings)
        {
            if (s.chunk == testSib)
                test = true;
        }

        return test;
    }

    void GenerateRoadLinks()
    {
        if (tileset.roadSeeds > 0)
        {
            for (int i = 0; i < tileset.roadSeeds; i++)
            {
                //because dictionaries cannot be referenced by index, pick a random point in chunk dictnioary,
                //and loop throuygh dictionary  until point is met
                int startIndex = Random.Range(0, chunks.Count);
                int iCount = 0;
                Chunk_Controller currentChunk = null;

                int rCode = Random.Range(0, tileset.tileMaster.transform.GetChild(2).childCount);

                foreach(Vector2Int v in chunks.Keys)
                {
                    if (iCount == startIndex && chunks[v].chunk != startChunk && chunks[v].chunk != endChunk)
                    {
                        currentChunk = chunks[v].chunk;
                        currentChunk.roadCode = rCode;
                        currentChunk.hasRoad = true;

                        //Debug.Log($"{currentChunk}, {startChunk}, {endChunk}");
                        

                        break;
                    }
                    else
                        iCount++;
                    
                }

                if (currentChunk)
                {
                    for (int rCount = 0; rCount < tileset.roadLength; rCount++)
                    {


                        //pick random sibling connected to current chunk, connect road, then set current chunk to sibling
                        int s = Random.Range(0, currentChunk.siblings.Count);

                        



                        if (currentChunk.siblings[s].chunk == lastChunk)
                        {
                            s++;
                            s = s % currentChunk.siblings.Count;
                        }

                        bool endCheck = currentChunk.siblings[s].chunk != startChunk && currentChunk.siblings[s].chunk != endChunk;

                        if (endCheck)
                        {
                           // Debug.Log($"FIRST {currentChunk.siblings[s].chunk}");

                            currentChunk.siblings[s].connectRoad = true;
                        }

                        foreach (Chunk_Controller.Sibling sib in currentChunk.siblings[s].chunk.siblings)
                        {
                            if (sib.chunk == currentChunk)
                            {
                                if (sib.chunk != startChunk && sib.chunk != endChunk)
                                {
                                    //Debug.Log($"SECOND {sib.chunk}");
                                    sib.connectRoad = true;
                                }
                            }
                        }

                        lastChunk = currentChunk;
                        currentChunk = currentChunk.siblings[s].chunk;

                        //if(currentChunk != startChunk && currentChunk != endChunk)
                        currentChunk.roadCode = rCode;
                    }
                }
                
            }
        }
    }

    void SpawnChunk(Vector2Int v, Dictionary<Vector2Int, ChunkInfo> C, bool forVoid)
    {
        Vector2 chunkPos = (Vector2)v * tileset.tileSize * tileset.res;

        GameObject b = (GameObject)Instantiate(Resources.Load("DungeonGen/Prefabs/Chunk_Basic", typeof(GameObject)));
        b.transform.parent = chunkShell;
        b.transform.position = transform.position + new Vector3(chunkPos.x, 0, chunkPos.y);
        b.gameObject.name = chunkShell.childCount.ToString();

        Chunk_Controller c = b.GetComponent<Chunk_Controller>();
        c.Init(v);


        if (lastChunk)
        {
            c.siblings.Add(new Chunk_Controller.Sibling(lastChunk, lastChunk.mapPos - c.mapPos));

            lastChunk.siblings.Add(new Chunk_Controller.Sibling(c, c.mapPos - lastChunk.mapPos));

            //Debug.Log($"Sib_{v - lastPos}");

            //Debug.Log($"{c.mapPos - lastChunk.mapPos}_{v}_{lastChunk.mapPos}");
        }

       

        lastChunk = c;
        lastPos = v;

        c.isVoid = forVoid;

        C[v].chunk = c;
    }

    public void GenerateMap()
    {
        foreach(Vector2Int v in chunks.Keys)
        {
            if(chunks[v].chunk != null)
                chunks[v].chunk.GenerateMap();
        }
    }

    class ChunkInfo
    {
        public Chunk_Controller chunk;
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(Map_Conditions))]
    public class MyScriptEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var myScript = target as Map_Conditions;

            if (GUILayout.Button("Spawn Map") && !myScript.isBuilding)
                myScript.BuildMap();

            
        }
    }
#endif
}

