using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Damage damage;
    public float radius = 1f;
    public float speed = 10f;
    public float maxDistance = 100f;
    float distanceTraveled;
    public LayerMask mask;
    public LayerMask coverMask;

    public GameObject hitFX;

    // Start is called before the first frame update
    void Start()
    {
        damage = damage.Clone();
        Debug.LogFormat("[{0}] {1}", this.name, damage.guid);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 newPosition = transform.position + transform.forward * speed * Time.deltaTime;
        Vector3 disp = newPosition - transform.position;
        distanceTraveled += disp.magnitude;

        if(distanceTraveled >= maxDistance)
            ProjectileHit(transform.position, -transform.forward);

        if (radius == 0)
        {
            RaycastHit hit;
            if (CombatUtils.HitScanCheck(
                damage,
                transform.position,
                disp.normalized,
                disp.magnitude,
                out hit,
                mask))
            {
                ProjectileHit(transform.position, -transform.forward);
            }
        }
        else
        {
            RaycastHit[] hits;
            if (CombatUtils.HitboxCheck(
                damage,
                transform.position,
                newPosition,
                radius,
                out hits,
                mask,
                coverMask))
            {
                Vector3 average = Vector3.zero;
                foreach (RaycastHit hit in hits)
                    average += hit.point;
                average /= hits.Length;
                ProjectileHit(average, -transform.forward);
            }
        }

        transform.position = newPosition;
    }

    public void ProjectileHit(Vector3 point, Vector3 direction)
    {
        SpawnHitFX(point, direction);
        Destroy(gameObject);
    }

    public void SpawnHitFX()
    {
        SpawnHitFX(transform.position, transform.forward);
    }

    public void SpawnHitFX(Vector3 point, Vector3 dir)
    {
        if (hitFX == null)
            return;

        GameObject.Instantiate(hitFX, point, Quaternion.LookRotation(dir));
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
