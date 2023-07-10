using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : Projectile
{
    public float blastRadius;
    public AnimationCurve damageFalloff;
    
    // Start is called before the first frame update
    // void Start()
    // {
        
    // }

    // // Update is called once per frame
    // void Update()
    // {
        
    // }

    public override void ProjectileHit(Vector3 point, Vector3 direction)
    {
        foreach (Transform e in effects)
            e.parent = null;
        SpawnHitFX(point, direction);

        Collider[] colls;
        CombatUtils.AOEHitCheck(
            transform.position,
            blastRadius,
            mask,
            out colls,
            damage,
            damageFalloff,
            coverMask);

        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
        Gizmos.DrawLine(
            transform.position,
            transform.position + transform.forward * Mathf.Clamp(speed, 0f, 5f));

        Gizmos.DrawWireSphere(transform.position, blastRadius);
    }
}
