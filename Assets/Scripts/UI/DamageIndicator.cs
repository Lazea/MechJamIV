using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageIndicator : MonoBehaviour
{
    public GameObject damageIndicatorTextPrefab;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnDamageIndicator(Damage damage)
    {
        var indicator = Instantiate(
            damageIndicatorTextPrefab,
            transform.position + UnityEngine.Random.insideUnitSphere * 0.5f,
            transform.localRotation
        ).GetComponent<DamageIndicatorText>();
        indicator.SetdamageValue(damage);
    }
}
