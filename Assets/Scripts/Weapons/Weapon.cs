using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("Weapon Data")]
    [SerializeField]
    WeaponData data;
    public WeaponData Data
    {
        get { return data; }
        set
        {
            Debug.Log("Weapon Data set");
            data = value;
            SetProjectilePrefab();
        }
    }

    [Header("Projectile")]
    GameObject projectilePrefab;
    public GameObject[] ballisticProjectilePrefabs;
    public GameObject[] energyBeamProjectilePrefabs;
    public GameObject[] rocketProjectilePrefabs;
    public GameObject[] laserProjectilePrefabs;
    public Transform projectileSpawnPoint;
    public Transform[] dualProjectileSpawnPoints;
    public Transform[] tripleProjectileSpawnPoints;

    [Header("Fire Rates")]
    public float fireRate;
    public float rampUpRate;
    float nextFireTime;
    bool isFiring;
    bool canFire;
    int burstCount;

    // Start is called before the first frame update
    void Start()
    {
        ResetWeaponData();
    }

    // Update is called once per frame
    void Update()
    {
        if (isFiring && Time.time >= nextFireTime)
        {
            switch (data.fireMode)
            {
                case FireMode.SemiAuto:
                    FireSemiAuto();
                    break;
                case FireMode.BurstFire:
                    FireBurst();
                    break;
                case FireMode.FullAuto:
                    FireFullAuto();
                    break;
            }
        }
    }

    public void Fire()
    {
        if(!isFiring)
        {
            isFiring = true;
            if(Time.time >= nextFireTime)
            {
                canFire = true;
                nextFireTime = Time.time;
            }
        }
    }

    public void FireRelease()
    {
        if(isFiring && data.fireMode != FireMode.BurstFire)
        {
            isFiring = false;
            canFire = false;
        }
    }

    void FireSemiAuto()
    {
        if (!canFire)
            return;

        FireProjectile();
        canFire = false;
        nextFireTime = Time.time + fireRate;
    }

    void FireBurst()
    {
        if (burstCount < 3)
        {
            FireProjectile();
            burstCount++;

            if (burstCount == 3)
            {
                // Apply delay before next burst
                nextFireTime = Time.time + fireRate;
                burstCount = 0;
                isFiring = false;
                canFire = false;
            }
            else
            {
                // Apply delay between shots in the burst
                nextFireTime = Time.time + (fireRate / 4f);
            }
        }
    }

    void FireFullAuto()
    {
        FireProjectile();
        nextFireTime = Time.time + fireRate;
    }

    [ContextMenu("Fire Projectile")]
    public void FireProjectile()
    {
        switch(data.fireModeModifier)
        {
            case FireModeModifier.ClusterFire:
                for(int i = 0; i < 6; i++)
                    SpawnProjectile(projectilePrefab, projectileSpawnPoint);
                break;
            case FireModeModifier.DualSplit:
                foreach (Transform t in dualProjectileSpawnPoints)
                    SpawnProjectile(projectilePrefab, t);
                break;
            case FireModeModifier.TrippleSplit:
                foreach (Transform t in tripleProjectileSpawnPoints)
                    SpawnProjectile(projectilePrefab, t);
                break;
            default:
                SpawnProjectile(projectilePrefab, projectileSpawnPoint);
                break;
        }
    }

    void SpawnProjectile(
        GameObject projectile,
        Transform spawnTransform)
    {
        Vector2 offset = Random.insideUnitCircle * data.spawnOffset;
        Vector3 point = projectileSpawnPoint.position;
        point += projectileSpawnPoint.right * offset.x;
        point += projectileSpawnPoint.up * offset.y;

        Vector2 recoilOffset = Random.insideUnitCircle * data.recoil;
        Vector3 dir = spawnTransform.forward;
        dir += spawnTransform.right * recoilOffset.x;
        dir += spawnTransform.up * recoilOffset.y;
        Quaternion rot = Quaternion.LookRotation(dir.normalized);

        Instantiate(
            projectile,
            point,
            rot);
    }
    
    void SetProjectilePrefab()
    {
        Debug.LogFormat(
            "Setting projectile prefab based on select type {0}",
            data.projectileType.ToString());
        switch (data.projectileType)
        {
            case ProjectileType.Ballistic:
                projectilePrefab = GetProjectileByDamageType(
                    ballisticProjectilePrefabs);
                break;
            case ProjectileType.EnergyBeam:
                projectilePrefab = GetProjectileByDamageType(
                    energyBeamProjectilePrefabs);
                break;
            case ProjectileType.Rocket:
                projectilePrefab = GetProjectileByDamageType(
                    rocketProjectilePrefabs);
                break;
            case ProjectileType.Laser:
                projectilePrefab = GetProjectileByDamageType(
                    laserProjectilePrefabs);
                break;
        }

        if(projectilePrefab == null)
        {
            Debug.LogWarningFormat(
                "Could not find projectile of type {0} and damage type {1}",
                data.projectileType.ToString(),
                data.damageType.ToString());
        }
        else
        {
            Debug.LogFormat("Projectile prefab set to {0}", projectilePrefab.name);
        }
    }

    GameObject GetProjectileByDamageType(GameObject[] projectilePrefabs)
    {
        Debug.Log("Checking projectile prefabs for type");
        foreach(GameObject p in projectilePrefabs)
        {
            Debug.LogFormat(
                "Checking projectile {0} with damageType {1} if it matches type {2}",
                p.name,
                p.GetComponent<Projectile>().damage.damageType.ToString(),
                data.damageType.ToString());
            if (p.GetComponent<Projectile>().damage.damageType == data.damageType)
            {
                Debug.LogFormat(
                    "Returning projectile {0} cause it matches type {1}",
                    p.name,
                    data.damageType);
                return p;
            }
        }

        return null;
    }

    [ContextMenu("Reset Weapon Data")]
    public void ResetWeaponData()
    {
        Data = data;
    }
}
