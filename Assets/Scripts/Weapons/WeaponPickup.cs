using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    [SerializeField]
    WeaponData data;
    public WeaponData Data
    {
        set
        {
            switch(value.rarity)
            {
                case Rarity.Common:
                    lightColumn.material.color = commonColor;
                    break;
                case Rarity.Uncommon:
                    lightColumn.material.color = uncommonColor;
                    break;
                case Rarity.Rare:
                    lightColumn.material.color = rareColor;
                    break;
                case Rarity.Legendary:
                    lightColumn.material.color = legendaryColor;
                    break;
            }
            data = value;
        }
        get
        {
            return data;
        }
    }

    public MeshRenderer lightColumn;
    public Color commonColor;
    public Color uncommonColor;
    public Color rareColor;
    public Color legendaryColor;

    private void Start()
    {
        Vector2 dir = Random.insideUnitCircle;
        GetComponent<Rigidbody>().AddForce(
            new Vector3(dir.x, 0f, dir.y) * 2f + Vector3.up * 2f,
            ForceMode.Impulse);
    }
}
