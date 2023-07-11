using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponGenerator : Singleton<WeaponGenerator>
{
    [Header("Dataset")]
    public TextAsset jsonFile;
    public WeaponsDataset weaponDataset;

    [Header("Weapon Mesh Prefabs")]
    public GameObject[] weaponPrefabs;

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
        WeaponData weapon = GetRandomWeapon(rarity);

        int i = Random.Range(0, weaponPrefabs.Length);
        Instantiate(
            weaponPrefabs[i],
            position,
            Quaternion.identity).GetComponent<WeaponPickup>().data = weapon;
    }

    public void GenerateWeapon(Vector3 position)
    {
        WeaponData weapon = GetRandomWeapon();

        int i = Random.Range(0, weaponPrefabs.Length);
        Instantiate(
            weaponPrefabs[i],
            position,
            Quaternion.identity).GetComponent<WeaponPickup>().data = weapon;
    }

    public void GenerateWeaponOnNPCDeath(Transform npcTransform)
    {
        // TODO: Determine if and what weapon rarity to spawn
        GenerateWeapon(npcTransform.position + Vector3.up);
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
