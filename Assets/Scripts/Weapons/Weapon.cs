using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Weapon : MonoBehaviour
{
    public WeaponData Data
    {
        get
        {
            return playerData.weaponData;
        }
        set
        {
            playerData.weaponData = value;
            SetProjectilePrefab();
        }
    }
    PlayerData playerData;

    [Header("Projectile")]
    GameObject projectilePrefab;
    public GameObject[] ballisticProjectilePrefabs;
    public GameObject[] plasmaProjectilePrefabs;
    public GameObject[] rocketProjectilePrefabs;
    public GameObject[] laserProjectilePrefabs;
    public Transform projectileSpawnPoint;
    public Transform[] dualProjectileSpawnPoints;
    public Transform[] tripleProjectileSpawnPoints;

    [Header("Fire Rates")]
    float nextFireTime;
    bool isFiring;
    bool canFire;
    int burstCount;

    [Header("Events")]
    public UnityEvent<WeaponData> onWeaponFire = new UnityEvent<WeaponData>();

    private void Awake()
    {
        playerData = GameManager.Instance.playerData;
        ResetWeaponData();
    }

    // Update is called once per frame
    void Update()
    {
        if (isFiring && Time.time >= nextFireTime)
        {
            switch (playerData.weaponData.fireMode)
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
            //isFiring = true;
            if(Time.time >= nextFireTime)
            {
                isFiring = true;
                canFire = true;
                nextFireTime = Time.time;
            }
        }
    }

    public void FireRelease()
    {
        if(isFiring && playerData.weaponData.fireMode != FireMode.BurstFire)
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
        nextFireTime = Time.time + Data.baseFireRate;
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
                nextFireTime = Time.time + Data.baseFireRate * 0.5f;
                burstCount = 0;
                isFiring = false;
                canFire = false;
            }
            else
            {
                // Apply delay between shots in the burst
                nextFireTime = Time.time + ((Data.baseFireRate * 0.5f) / 4f);
            }
        }
    }

    void FireFullAuto()
    {
        FireProjectile();
        nextFireTime = Time.time + Data.baseFireRate;
    }

    [ContextMenu("Fire Projectile")]
    public void FireProjectile()
    {
        Vector2 recoilOffset = Random.insideUnitCircle * playerData.weaponData.baseAccuracy;
        switch (playerData.weaponData.fireModeModifier)
        {
            case FireModeModifier.Cluster:
                for(int i = 0; i < 6; i++)
                    SpawnProjectile(
                        projectilePrefab,
                        projectileSpawnPoint,
                        projectileSpawnPoint);
                break;
            case FireModeModifier.DualSplit:
                foreach (Transform t in dualProjectileSpawnPoints)
                    SpawnProjectile(
                        projectilePrefab,
                        t,
                        projectileSpawnPoint,
                        recoilOffset);
                break;
            case FireModeModifier.TrippleSplit:
                foreach (Transform t in tripleProjectileSpawnPoints)
                    SpawnProjectile(
                        projectilePrefab,
                        t,
                        projectileSpawnPoint,
                        recoilOffset);
                break;
            default:
                SpawnProjectile(
                    projectilePrefab,
                    projectileSpawnPoint,
                    projectileSpawnPoint,
                    recoilOffset);
                break;
        }

        onWeaponFire.Invoke(playerData.weaponData);
    }

    void SpawnProjectile(
        GameObject projectilePrefab,
        Transform spawnTransform,
        Transform spawnDirTransfrom,
        Vector2 recoilOffset = default)
    {
        Vector3 point = spawnTransform.position;

        if (recoilOffset == default)
            recoilOffset = Random.insideUnitCircle * playerData.weaponData.baseAccuracy;
        Vector3 dir = spawnDirTransfrom.forward;
        dir += spawnDirTransfrom.right * recoilOffset.x;
        dir += spawnDirTransfrom.up * recoilOffset.y;
        Quaternion rot = Quaternion.LookRotation(dir.normalized);
        rot.eulerAngles += spawnTransform.localEulerAngles;

        CombatUtils.SpawnProjectile(
            point,
            rot,
            Data.baseDamage * playerData.damageMultiplier,
            playerData.shieldEnergyDamageMultiplier,
            playerData.critChance,
            playerData.critDamageMultiplier,
            playerData.fireChance,
            playerData.fireDamage,
            playerData.fireDuration,
            playerData.shockChance,
            playerData.shockDuration,
            projectilePrefab,
            gameObject);
    }
    
    void SetProjectilePrefab()
    {
        Debug.LogFormat(
            "Setting projectile prefab based on select type {0}",
            playerData.weaponData.projectileType.ToString());
        switch (playerData.weaponData.projectileType)
        {
            case ProjectileType.Ballistic:
                projectilePrefab = GetProjectileByDamageType(
                    ballisticProjectilePrefabs);
                break;
            case ProjectileType.Plasma:
                projectilePrefab = GetProjectileByDamageType(
                    plasmaProjectilePrefabs);
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
                playerData.weaponData.projectileType.ToString(),
                playerData.weaponData.damageType.ToString());
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
                p.GetComponent<IProjectile>().Damage.damageType.ToString(),
                playerData.weaponData.damageType.ToString());
            if (p.GetComponent<IProjectile>().Damage.damageType == playerData.weaponData.damageType)
            {
                Debug.LogFormat(
                    "Returning projectile {0} cause it matches type {1}",
                    p.name,
                    playerData.weaponData.damageType);
                return p;
            }
        }

        return null;
    }

    [ContextMenu("Reset Weapon Data")]
    public void ResetWeaponData()
    {
        Data = playerData.weaponData;
    }
}
