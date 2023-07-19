using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        switch(playerData.weaponData.fireModeModifier)
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
        GameObject projectilePrefab,
        Transform spawnTransform)
    {
        Vector2 offset = Random.insideUnitCircle * playerData.weaponData.spawnOffset;
        Vector3 point = projectileSpawnPoint.position;
        point += projectileSpawnPoint.right * offset.x;
        point += projectileSpawnPoint.up * offset.y;

        Vector2 recoilOffset = Random.insideUnitCircle * playerData.weaponData.recoil;
        Vector3 dir = spawnTransform.forward;
        dir += spawnTransform.right * recoilOffset.x;
        dir += spawnTransform.up * recoilOffset.y;
        Quaternion rot = Quaternion.LookRotation(dir.normalized);

        CombatUtils.SpawnProjectile(
            point,
            rot,
            playerData.damageMultiplier,
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
