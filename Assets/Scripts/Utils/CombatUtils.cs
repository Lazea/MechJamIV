using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public static class CombatUtils
{
    public static void ApplyDamage(Damage damage, IDamageable damageable)
    {
        // Check GUID before applying damage
        if (!damageable.CheckGUIDIsInBuffer(damage.guid))
            return;

        int shieldDamage = Mathf.RoundToInt(damage.amount * damage.shieldMultiplier);

        damageable.Shield -= shieldDamage;
        if (damageable.Shield <= 0)
        {
            int healthDamage = Mathf.RoundToInt(Mathf.Abs(damageable.Shield) / damage.shieldMultiplier);
            damageable.Shield = 0;

            damageable.Health -= healthDamage;
            if (damageable.Health <= 0)
                damageable.Health = 0;
        }
    }

    public static bool CheckGUIDIsInBuffer(
        Queue<System.Guid> guidBuffer,
        System.Guid guid)
    {
        Debug.Log("Buffer:");
        foreach(System.Guid g in guidBuffer)
            Debug.Log(g);
        Debug.LogFormat("Check against {0}", guid);
        if (guidBuffer.Any(o => o.Equals(guid)))
        {
            Debug.Log("GUID is in Buffer");
            return false;
        }

        Debug.Log("GUID is NOT in Buffer");
        if (guidBuffer.Count >= GameSettings.guidBufferCapacity)
            guidBuffer.Dequeue();
        guidBuffer.Enqueue(guid);
        return true;
    }

    public static bool HitScanCheck(
        Damage damage,
        Vector3 origin,
        Vector3 dir,
        float distance,
        out RaycastHit hit,
        LayerMask mask)
    {
        Ray ray = new Ray(origin, dir);
        if(Physics.Raycast(ray, out hit, distance, mask))
        {
            IDamageable damageable = hit.collider.GetComponentInParent<IDamageable>();
            if (damageable != null)
            {
                damageable.ApplyDamage(damage);
            }

            return true;
        }

        return false;
    }

    public static bool HitboxCheck(
        Damage damage,
        Vector3 origin,
        Vector3 end,
        float radius,
        out RaycastHit[] hits,
        LayerMask mask,
        LayerMask coverMask = default)
    {
        Vector3 disp = end - origin;
        Ray ray = new Ray(origin, disp.normalized);
        hits = Physics.SphereCastAll(ray, radius, disp.magnitude, mask);
        bool hitConfirm = false;
        foreach(RaycastHit hit in hits)
        {
            if (coverMask == default)
            {
                IDamageable damageable = hit.collider.GetComponentInParent<IDamageable>();
                if (damageable != null)
                    damageable.ApplyDamage(damage);

                hitConfirm = true;
            }
            else
            {
                Vector3 _disp = hit.point - origin;
                if (!Physics.Raycast(
                    origin,
                    disp.normalized,
                    disp.magnitude,
                    coverMask))
                {
                    IDamageable damageable = hit.collider.GetComponentInParent<IDamageable>();
                    if (damageable != null)
                        damageable.ApplyDamage(damage);

                    hitConfirm = true;
                }
            }
        }

        return hitConfirm;
    }

    public static void AOEHitCheck(
        Vector3 origin,
        float radius,
        LayerMask mask,
        Damage damage,
        out Collider[] colliders,
        AnimationCurve damageFalloff = default,
        LayerMask coverMask = default)
    {
        colliders = Physics.OverlapSphere(origin, radius, mask);
        foreach(Collider c in colliders)
        {
            Vector3 disp = c.transform.position - origin;

            IDamageable damageable = c.GetComponentInParent<IDamageable>();
            if (damageable != null)
            {
                if (damageFalloff != null)
                {
                    Damage _damage = damage.Clone();
                    float k = damageFalloff.Evaluate(disp.magnitude / radius);
                    _damage.amount *= k;
                }

                if (coverMask == default)
                {
                    damageable.ApplyDamage(damage);
                }
                else
                {
                    if (!Physics.Raycast(origin, disp.normalized, disp.magnitude, coverMask))
                    {
                        damageable.ApplyDamage(damage);
                    }
                }
            }
        }
    }
}
