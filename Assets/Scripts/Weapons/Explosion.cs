using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    public Damage damage;
    public float radius;
    public AnimationCurve damageFalloff;
    public LayerMask mask;
    public LayerMask coverMask;

    public Transform[] childEffects;

    // Start is called before the first frame update
    void Start()
    {
        Collider[] colls;
        CombatUtils.AOEHitCheck(
            transform.position,
            radius,
            mask,
            out colls,
            damage,
            damageFalloff,
            coverMask);

        foreach(Transform fx in childEffects)
            fx.parent = null;

        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
