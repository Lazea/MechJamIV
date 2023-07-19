using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hurtbox : MonoBehaviour, IProjectile
{
    [Header("Damage")]
    public Damage damage;
    public Damage Damage
    {
        get { return damage; }
        set { damage = value; }
    }

    [Header("Hit Check & Travel")]
    public float radius;
    public float range;
    public LayerMask mask;
    public LayerMask coverMask;

    [Header("Effects")]
    [Tooltip("Effect to spawn on hit.")]
    public Transform hitFX;

    // Start is called before the first frame update
    void Start()
    {
        if (radius == 0)
        {
            RaycastHit hit;
            if (CombatUtils.HitScanCheck(
                transform.position,
                transform.forward,
                range,
                out hit,
                mask,
                damage))
            {
                Hit(hit.point);
                return;
            }
        }
        else
        {
            RaycastHit[] hits;
            if (CombatUtils.HitboxCheck(
                transform.position,
                transform.position + transform.forward * range,
                radius,
                out hits,
                mask,
                coverMask,
                damage))
            {
                Vector3 averagePoint = Vector3.zero;
                foreach (RaycastHit hit in hits)
                {
                    averagePoint += hit.point;
                }
                averagePoint /= hits.Length;
                Hit(averagePoint);
                return;
            }
        }

        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Hit(Vector3 point)
    {
        if (hitFX != null)
        {
            hitFX.transform.parent = null;
            hitFX.transform.position = point;
            hitFX.transform.rotation = Quaternion.LookRotation(-transform.forward);
            hitFX.gameObject.SetActive(true);
        }

        DrawPoint(point);
        Destroy(gameObject);
    }

    void DrawPoint(Vector3 point)
    {
        Debug.DrawLine(point, point + Vector3.forward);
        Debug.DrawLine(point, point - Vector3.forward);
        Debug.DrawLine(point, point + Vector3.right);
        Debug.DrawLine(point, point - Vector3.right);
        Debug.DrawLine(point, point + Vector3.up);
        Debug.DrawLine(point, point - Vector3.up);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position + transform.forward * range, radius);
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * range);
    }
}
