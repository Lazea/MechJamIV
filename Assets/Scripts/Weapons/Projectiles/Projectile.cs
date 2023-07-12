using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Damage")]
    public Damage damage;

    [Header("Hit Check & Travel")]
    public float radius = 1f;
    public float speed = 10f;
    [Tooltip("How much gravity should affect this projectile.")]
    public float gravityModifier = 0f;
    [Tooltip("How far should the projectile travel before being destroyed.")]
    public float maxDistance = 100f;
    float distanceTraveled;
    public LayerMask mask;
    [Tooltip(
        "If the radius is > 0.0 then the hit check is of SphereCastAll. " +
        "Thus this cover mask ensures that nothing getting hit is obstructed " +
        "by some kind of cover like a wall.")]
    public LayerMask coverMask;

    [Header("Effects")]
    [Tooltip("Effects that should detach on destroy. Examples inclue smoke trails.")]
    public Transform[] effects;
    [Tooltip("Effect to spawn on hit.")]
    public Transform hitFX;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        damage = damage.Clone();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        Vector3 newPosition = transform.position + transform.forward * speed * Time.deltaTime;
        newPosition += Physics.gravity.y * Vector3.up * gravityModifier * Time.deltaTime * Time.deltaTime;
        Vector3 disp = newPosition - transform.position;
        distanceTraveled += disp.magnitude;

        if(distanceTraveled >= maxDistance)
            ProjectileHit(transform.position, -transform.forward);

        if (radius == 0)
        {
            RaycastHit hit;
            if (CombatUtils.HitScanCheck(
                transform.position,
                disp.normalized,
                disp.magnitude,
                out hit,
                mask,
                damage))
            {
                ProjectileHit(hit.point, hit.normal);
            }
        }
        else
        {
            RaycastHit[] hits;
            if (CombatUtils.HitboxCheck(
                transform.position,
                newPosition,
                radius,
                out hits,
                mask,
                coverMask,
                damage))
            {
                Vector3 averagePoint = Vector3.zero;
                Vector3 averageDir = Vector3.zero;
                foreach (RaycastHit hit in hits)
                {
                    averagePoint += hit.point;
                    averageDir += hit.normal;
                }
                averagePoint /= hits.Length;
                averageDir /= hits.Length;
                ProjectileHit(averagePoint, averageDir);
            }
        }

        transform.position = newPosition;
        Quaternion rot = Quaternion.LookRotation(disp.normalized, Vector3.up);
        transform.rotation = rot;
    }

    public virtual void ProjectileHit(Vector3 point, Vector3 direction)
    {
        foreach (Transform e in effects)
            e.parent = null;
        SpawnHitFX(point, direction);
        Destroy(gameObject);
    }

    public virtual void SpawnHitFX()
    {
        SpawnHitFX(transform.position, transform.forward);
    }

    public virtual void SpawnHitFX(Vector3 point, Vector3 dir)
    {
        if (hitFX == null)
            return;
            
        hitFX.transform.parent = null;
        hitFX.transform.position = point;
        hitFX.transform.rotation = Quaternion.LookRotation(dir);
        hitFX.gameObject.SetActive(true);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
        Gizmos.DrawLine(
            transform.position,
            transform.position + transform.forward * Mathf.Clamp(speed, 0f, 5f));
    }
}
