using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using SOGESys = SOGameEventSystem;

public class NPCsManager : Singleton<NPCsManager>
{
    public Transform npcsParent;

    [Header("Waves Pool")]
    public NPCSpawnWave[] spawnWavesPool;

    [Header("Waves")]
    public int wavesCount = 2;
    public List<NPCSpawnWave> sceneSpawnWaves;
    int waveIndex = 0;
    bool combatComplete;

    [Header("Spawn Points")]
    public float minSpawnDistance = 20f;
    public string fuzzySpawnPointsName = "NPCSpawnPoint";
    public string turretSpawnPointsName = "TurretSpawnPoint";
    public List<Transform> fuzzySpawnPoints;
    public List<Transform> turretSpawnPoints;

    [Header("Spawned NPCs")]
    public List<GameObject> spawnedNPCs;
    public List<GameObject> spawnedTurretNPCs;

    [Header("Events")]
    public SOGESys.Events.TransformGameEvent NPCSpawned;
    public SOGESys.BaseGameEvent CombatComplete;

    // Start is called before the first frame update
    void Start()
    {
        // Grab all fuzzy spawn points
        fuzzySpawnPoints = new List<Transform>();
        var spawns = GameObject.FindObjectsOfType<GameObject>()
            .Where(obj => obj.name == fuzzySpawnPointsName)
            .ToArray();
        foreach (GameObject s in spawns)
        {
            fuzzySpawnPoints.Add(s.transform);
        }

        // Grab all turret spawn points
        turretSpawnPoints = new List<Transform>();
        spawns = GameObject.FindObjectsOfType<GameObject>()
            .Where(obj => obj.name == turretSpawnPointsName)
            .ToArray();
        foreach (GameObject s in spawns)
        {
            turretSpawnPoints.Add(s.transform);
        }

        sceneSpawnWaves = SelectSpawnWavesFromPool();

        // Pick a wave that has the most turrets in count
        NPCSpawnWave selectedTurretSpawnWave = sceneSpawnWaves[0];
        int waveTurretCount = 0;
        foreach(var wave in sceneSpawnWaves)
        {
            if(wave.turretCount > waveTurretCount)
            {
                waveTurretCount = wave.turretCount;
                selectedTurretSpawnWave = wave;
            }
        }

        // Spawn all turrets as long as there is enough space
        System.Random rand = new System.Random();
        var shuffledSpawns = turretSpawnPoints.OrderBy(x => rand.Next()).ToArray();
        int maxTurretCount = Mathf.Min(
            shuffledSpawns.Length,
            selectedTurretSpawnWave.turretCount);
        for(int i = 0; i < maxTurretCount; i++)
        {
            int j = UnityEngine.Random.Range(0, selectedTurretSpawnWave.turrets.Length);
            Vector3 playerPosition = Vector3.zero;
            if(GameManager.Instance.Player != null)
                playerPosition = GameManager.Instance.Player.transform.position;
            if (Vector3.Distance(playerPosition, shuffledSpawns[i].position) >= minSpawnDistance * 0.5f)
                SpawnNPC(
                    selectedTurretSpawnWave.turrets[j],
                    shuffledSpawns[i].position,
                    spawnedTurretNPCs);
        }

        // Spawn initial wave
        foreach(var unitSet in sceneSpawnWaves[waveIndex].enemyUnitSets)
        {
            Debug.LogFormat("Spawning unit {0}", unitSet.enemyUnit.name);
            SpawnNPCsWave(unitSet.enemyUnit, unitSet.unitCount);
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if(spawnedNPCs.Count == 0)
        {
            // TODO: Add a delay
            waveIndex++;
            if(waveIndex < sceneSpawnWaves.Count)
            {
                Debug.LogFormat("Spawning Enemy Wave {0}", waveIndex + 1);
                foreach(var unitSet in sceneSpawnWaves[waveIndex].enemyUnitSets)
                {
                    SpawnNPCsWave(unitSet.enemyUnit, unitSet.unitCount);
                }
            }
            else if(!combatComplete)
            {
                Debug.Log("All waves completed. Combat is Done");
                combatComplete = true;
                CombatComplete.Raise();
            }
        }
    }

    public List<NPCSpawnWave> SelectSpawnWavesFromPool()
    {
        List<NPCSpawnWave> waves = new List<NPCSpawnWave>();

        // TODO: Control save count and selection based on player stage count
        for(int i = 0; i < wavesCount; i++)
        {
            int j = UnityEngine.Random.Range(0, spawnWavesPool.Length);
            waves.Add(spawnWavesPool[j]);
        }

        return waves;
    }

    public GameObject SpawnNPC(GameObject npc)
    {
        List<Vector3> points = SelectSpawnPoints();
        if (points.Count == 0)
            return null;

        int i = UnityEngine.Random.Range(0, points.Count);
        Vector3 point = GetRandomSpawnPoint(points[i], 8f);

        return SpawnNPC(npc, point);
    }

    public GameObject SpawnNPC(GameObject npc, Vector3 position, List<GameObject> spawnedList = default)
    {
        GameObject _npc = Instantiate(
            npc,
            position,
            Quaternion.identity,
            npcsParent);

        if(spawnedList == default)
            spawnedNPCs.Add(_npc);
        else
            spawnedList.Add(_npc);

        NPCSpawned.Raise(_npc.transform);

        return _npc;
    }

    public void SpawnNPCsWave(GameObject npc, int count, List<GameObject> spawnedList = default)
    {
        List<Vector3> points = SelectSpawnPoints();

        if (points.Count == 0)
        {
            Debug.LogWarningFormat("No spawn points found for {0}", npc.name);
            return;
        }

        for (int i = 0; i < count; i++)
        {
            int j = UnityEngine.Random.Range(0, points.Count);
            Vector3 point = GetRandomSpawnPoint(points[j], 8f);

            SpawnNPC(npc, point, spawnedList);
        }
    }

    public Vector3 GetRandomSpawnPoint(Vector3 point, float radius)
    {
        Vector2 offset = UnityEngine.Random.insideUnitCircle * radius;
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
                {
                    Debug.LogWarning("No spawn point could be found on the navMesh");
                    break;
                }
            }
        }

        return point;
    }

    public List<Vector3> SelectSpawnPoints()
    {
        List<Vector3> spawnPoints = new List<Vector3>();
        if (GameManager.Instance.Player == null)
            return spawnPoints;

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

    public bool IsCombatCompleted()
    {
        return combatComplete;
    }
}
