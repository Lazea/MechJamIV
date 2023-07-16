using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponGenerator : Singleton<WeaponGenerator>
{
    [Header("Dataset")]
    public TextAsset jsonFile;
    public WeaponsDataset weaponDataset;

    [Header("Base Weapon Prefab")]
    public GameObject baseWeaponPickupPrefab;

    [Header("Barrel Prefabs")]
    public GameObject[] longBallisticBarrelMeshes;
    public GameObject[] shortBallisticBarrelMeshes;
    public GameObject[] longPlasmaBarrelMeshes;
    public GameObject[] shortPlasmaBarrelMeshes;
    public GameObject[] longRocketBarrelMeshes;
    public GameObject[] shortRocketBarrelMeshes;

    [Header("Mag Prefabs")]
    public GameObject[] semiAutoMagMeshes;
    public GameObject[] burstFireMagMeshes;
    public GameObject[] fullAutoMagMeshes;
    
    [Header("Body Prefabs")]
    public GameObject[] ballisticBodyMeshes;
    public GameObject[] plasmaBodyMeshes;
    public GameObject[] rocketBodyMeshes;

    // Start is called before the first frame update
    void Start()
    {
        ParseJson();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void GenerateWeapon(Vector3 position, Rarity rarity)
    {
        WeaponData data = GetRandomWeapon(rarity);
        GenerateWeapon(position, data);
    }

    public void GenerateWeapon(Vector3 position)
    {
        WeaponData data = GetRandomWeapon();
        GenerateWeapon(position, data);
    }

    public void GenerateWeaponOnNPCDeath(Transform npcTransform)
    {
        // TODO: Determine if and what weapon rarity to spawn
        GenerateWeapon(npcTransform.position + Vector3.up);
    }

    public (GameObject, GameObject, GameObject) GenerateWeapon(
        Vector3 position,
        WeaponData data,
        GameObject weaponParentObject = default,
        bool addPickup = true)
    {
        GameObject weaponObject = weaponParentObject;
        if(weaponParentObject == default)
        {
            weaponObject = Instantiate(
                baseWeaponPickupPrefab,
                position,
                Quaternion.identity);
        }

        if(addPickup)
            weaponObject.GetComponent<WeaponPickup>().data = data;

        return ConstructWeaponObject(weaponObject, data);
    }

    public (GameObject, GameObject, GameObject) ConstructWeaponObject(
        GameObject weaponObject,
        WeaponData data)
    {
        GameObject body = null;
        GameObject barrel = null;
        GameObject mag = null;
        switch(data.projectileType)
        {
            case ProjectileType.Ballistic:
                body = GenerateWeaponBody(weaponObject, ballisticBodyMeshes);
                barrel = GenerateWeaponBarrel(
                    body,
                    shortBallisticBarrelMeshes,
                    longBallisticBarrelMeshes,
                    data.recoil);
                break;
            case ProjectileType.EnergyBeam:
                body = GenerateWeaponBody(weaponObject, plasmaBodyMeshes);
                barrel = GenerateWeaponBarrel(
                    body,
                    shortPlasmaBarrelMeshes,
                    longPlasmaBarrelMeshes,
                    data.recoil);
                break;
            case ProjectileType.Rocket:
                body = GenerateWeaponBody(weaponObject, rocketBodyMeshes);
                barrel = GenerateWeaponBarrel(
                    body,
                    shortRocketBarrelMeshes,
                    longRocketBarrelMeshes,
                    data.recoil);
                break;
        }

        switch(data.fireMode)
        {
            case FireMode.SemiAuto:
                mag = GenerateWeaponMag(body, semiAutoMagMeshes);
                break;
            case FireMode.BurstFire:
                mag = GenerateWeaponMag(body, burstFireMagMeshes);
                break;
            case FireMode.FullAuto:
                mag = GenerateWeaponMag(body, fullAutoMagMeshes);
                break;
        }

        body.transform.localScale = Vector3.one * 0.2f;
        return (body, barrel, mag);
    }

    public GameObject GenerateWeaponBody(
        GameObject weaponObject,
        GameObject[] meshes)
    {
        int i = Random.Range(0, meshes.Length);
        GameObject body = Instantiate(meshes[i]);
        body.transform.parent = weaponObject.transform;
        body.transform.localPosition = Vector3.zero;
        return body;
    }

    public GameObject GenerateWeaponBarrel(
        GameObject weaponBody,
        GameObject[] shortBarrelMeshes,
        GameObject[] longBarrelMeshes,
        float recoil)
    {
        float accuracy = 1f - (recoil / 0.08f);
        var barrels = (accuracy >= 0.5f) ? longBarrelMeshes : shortBarrelMeshes;
        int i = Random.Range(0, barrels.Length);

        Vector3 mountPoint = Vector3.zero;
        Transform mountPointTransform = weaponBody.transform.Find("BarrelMountPoint");
        if(mountPointTransform != null)
            mountPoint = mountPointTransform.localPosition;

        GameObject barrel = Instantiate(barrels[i]);
        barrel.transform.parent = weaponBody.transform;
        barrel.transform.localPosition = mountPoint;
        return barrel;
    }

    public GameObject GenerateWeaponMag(
        GameObject weaponBody,
        GameObject[] meshes)
    {
        int i = Random.Range(0, meshes.Length);

        Vector3 mountPoint = Vector3.zero;
        Transform mountPointTransform = weaponBody.transform.Find("MagMountPoint");
        if(mountPointTransform != null)
            mountPoint = mountPointTransform.localPosition;

        GameObject mag = Instantiate(semiAutoMagMeshes[i]);
        mag.transform.parent = weaponBody.transform;
        mag.transform.localPosition = mountPoint;
        return mag;
    }

    [ContextMenu("Test Generate Weapon")]
    public void TestGenerateWeapon()
    {
        GenerateWeapon(Vector3.up);
    }

    public WeaponData GetRandomWeapon()
    {
        WeaponData weaponData = null;
        int i = 0;
        switch (Random.Range(0, 4))
        {
            case 0:
                i = Random.Range(0, weaponDataset.Common.Count);
                weaponData = weaponDataset.Common[i];
                break;
            case 1:
                i = Random.Range(0, weaponDataset.Uncommon.Count);
                weaponData = weaponDataset.Uncommon[i];
                break;
            case 2:
                i = Random.Range(0, weaponDataset.Rare.Count);
                weaponData = weaponDataset.Rare[i];
                break;
            case 3:
                i = Random.Range(0, weaponDataset.Legendary.Count);
                weaponData = weaponDataset.Legendary[i];
                break;
        }

        return weaponData;
    }

    public WeaponData GetRandomWeapon(Rarity rarity)
    {
        WeaponData weaponData = null;
        int i = 0;
        switch (rarity)
        {
            case Rarity.Common:
                i = Random.Range(0, weaponDataset.Common.Count);
                weaponData = weaponDataset.Common[i];
                break;
            case Rarity.Uncommon:
                i = Random.Range(0, weaponDataset.Uncommon.Count);
                weaponData = weaponDataset.Uncommon[i];
                break;
            case Rarity.Rare:
                i = Random.Range(0, weaponDataset.Rare.Count);
                weaponData = weaponDataset.Rare[i];
                break;
            case Rarity.Legendary:
                i = Random.Range(0, weaponDataset.Legendary.Count);
                weaponData = weaponDataset.Legendary[i];
                break;
        }

        return weaponData;
    }

    [ContextMenu("Parse Json File")]
    public void ParseJson()
    {
        weaponDataset = WeaponsJsonParser.ParseJson(jsonFile);
    }
}
