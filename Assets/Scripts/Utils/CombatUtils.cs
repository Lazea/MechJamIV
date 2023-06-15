using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CombatUtils
{
    public static void ApplyDamage(Damage damage, IDamageable damageable)
    {
        // Check GUID before applying damage
        if (damageable.CheckGUIDIsInBuffer(damage.guid))
            return;

        // Apply shield damage
        int damageAmount = Mathf.RoundToInt(damage.amount * damage.shieldMultiplier);
        if (damageable.Shield > damageAmount)
        {
            damageable.Shield -= damageAmount;
            damageAmount = 0;
        }
        else
        {
            damageAmount -= damageable.Shield;
            damageable.Shield = 0;

            damageAmount = Mathf.RoundToInt(damageAmount / damage.shieldMultiplier);
        }

        // Apply remaining damage to health
        int damageDealt = (damageable.Health < damageAmount) ? 
            damageable.Health : damageAmount;
        damageable.Health -= damageDealt;
    }

    public static bool CheckGUIDIsInBuffer(
        Queue<System.Guid> guidBuffer,
        System.Guid guid)
    {
        if (guidBuffer.Contains(guid))
            return false;

        if (guidBuffer.Count >= GameSettings.guidBufferCapacity)
            guidBuffer.Dequeue();
        guidBuffer.Enqueue(guid);
        return true;
    }

    public static void HitScanCheck(
        Damage damage,
        Vector3 origin,
        Vector3 dir,
        float distance,
        LayerMask mask)
    {
        Ray ray = new Ray(origin, dir);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, distance, mask))
        {
            IDamageable damageable = hit.collider.GetComponentInParent<IDamageable>();
            if (damageable != null)
            {
                damageable.ApplyDamage(damage);
            }
        }
    }

    public static void HitboxCheck(
        Damage damage,
        Vector3 origin,
        Vector3 dir,
        float distance,
        float radius,
        LayerMask mask,
        LayerMask coverMask = default)
    {
        Ray ray = new Ray(origin, dir);
        RaycastHit[] hits = Physics.SphereCastAll(ray, radius, distance, mask);
        foreach(RaycastHit hit in hits)
        {
            IDamageable damageable = hit.collider.GetComponentInParent<IDamageable>();
            if (damageable != null)
            {
                if(coverMask == default)
                {
                    damageable.ApplyDamage(damage);
                }
                else
                {
                    Vector3 disp = hit.point - origin;
                    if (!Physics.Raycast(
                        origin,
                        disp.normalized,
                        disp.magnitude,
                        coverMask))
                    {
                        damageable.ApplyDamage(damage);
                    }
                }
            }
        }
    }

    public static void SphereHitCheck(
        Vector3 origin,
        float radius,
        LayerMask mask,
        Damage damage,
        AnimationCurve damageFalloff = default,
        LayerMask coverMask = default)
    {
        Collider[] colliders = Physics.OverlapSphere(origin, radius, mask);
        foreach(Collider c in colliders)
        {
            IDamageable damageable = c.GetComponentInParent<IDamageable>();
            if (damageable != null)
            {
                if (coverMask == default)
                {
                    damageable.ApplyDamage(damage);
                }
                else
                {
                    Vector3 disp = c.transform.position - origin;
                    if (!Physics.Raycast(origin, disp.normalized, disp.magnitude, coverMask))
                    {
                        damageable.ApplyDamage(damage);
                    }
                }
            }
        }
    }
}
