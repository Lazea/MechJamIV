using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using SOGESys = SOGameEventSystem;

public class NPCsManager : Singleton<NPCsManager>
{
    public Transform npcsParent;

    [Header("Prefabs")]
    public GameObject[] npcPrefabs;
    public GameObject turretNPCPrefab;

    [Header("Spawn Points")]
    public float minSpawnDistance = 20f;
    public List<Transform> fuzzySpawnPoints;
    public List<Transform> turretSpawnPoints;

    [Header("Spawned NPCs")]
    public List<GameObject> spawnedNPCs;

    [Header("Events")]
    public SOGESys.Events.TransformGameEvent NPCSpawned;

    [Header("Testing")]
    [SerializeField]
    int npcIndex;
    [SerializeField]
    int npcSpawnCount;

    // Start is called before the first frame update
    void Start()
    {
        foreach(Transform s in turretSpawnPoints)
        {
            SpawnNPC(turretNPCPrefab, s.position);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public GameObject SpawnNPC(GameObject npc)
    {
        List<Vector3> points = SelectSpawnPoints();
        int i = Random.Range(0, points.Count);
        Vector3 point = GetRandomSpawnPoint(points[i], 8f);

        return SpawnNPC(npc, point);
    }

    public GameObject SpawnNPC(GameObject npc, Vector3 position)
    {
        GameObject _npc = Instantiate(
            npc,
            position,
            Quaternion.identity,
            npcsParent);
        spawnedNPCs.Add(_npc);

        NPCSpawned.Raise(_npc.transform);

        return _npc;
    }

    public void SpawnNPCsWave(GameObject npc, int count)
    {
        List<Vector3> points = SelectSpawnPoints();
        for (int i = 0; i < count; i++)
        {
            int j = Random.Range(0, points.Count);
            Vector3 point = GetRandomSpawnPoint(points[j], 8f);

            SpawnNPC(npc, point);
        }
    }

    public Vector3 GetRandomSpawnPoint(Vector3 point, float radius)
    {
        Vector2 offset = Random.insideUnitCircle * radius;
        Vector3 _point = point + new Vector3(offset.x, point.y, offset.y);

        int tries = 3;
        NavMeshHit hit;
        while (true)
        {
            if(NavMesh.SamplePosition(
                _point,
                out hit,
                radius + 1f,
                NavMesh.AllAreas))
            {
                point = hit.position;
                break;
            }
            else
            {
                tries--;
                if (tries < 0)
                    break;
            }
        }

        return point;
    }

    public List<Vector3> SelectSpawnPoints()
    {
        List<Vector3> spawnPoints = new List<Vector3>();
        Vector3 playerPosition = GameManager.Instance.Player.transform.position;
        foreach(Transform s in fuzzySpawnPoints)
        {
            if (Vector3.Distance(playerPosition, s.position) >= minSpawnDistance)
                spawnPoints.Add(s.position);
        }

        return spawnPoints;
    }

    public void NPCKilled(Transform npc)
    {
        spawnedNPCs.Remove(npc.gameObject);
    }

    #region [Tests]
    [ContextMenu("Test NPC Spawn")]
    public void TestSpawnNPC()
    {
        SpawnNPC(npcPrefabs[npcIndex]);
    }

    [ContextMenu("Test NPC Wave Spawn")]
    public void TestSpawnNPCWave()
    {
        SpawnNPCsWave(npcPrefabs[npcIndex], npcSpawnCount);
    }
    #endregion
}
