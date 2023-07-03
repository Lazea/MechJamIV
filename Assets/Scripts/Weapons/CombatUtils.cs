using System.Linq;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Provides utility methods for combat-related functionality.
/// </summary>
public static class CombatUtils
{
    /// <summary>
    /// Applies damage to a damageable object, taking into account shields and health.
    /// </summary>
    /// <param name="damage">The damage object to apply.</param>
    /// <param name="damageable">The damageable object.</param>
    public static void ApplyDamage(
        Damage damage,
        IDamageable damageable)
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

    /// <summary>
    /// Checks if the specified GUID is present in the GUID buffer.
    /// </summary>
    /// <param name="guidBuffer">The GUID buffer.</param>
    /// <param name="guid">The GUID to check.</param>
    /// <returns>True if the GUID is in the buffer, false otherwise.</returns>
    public static bool CheckGUIDIsInBuffer(
        Queue<System.Guid> guidBuffer,
        System.Guid guid)
    {
        if (guidBuffer.Any(o => o.Equals(guid)))
            return false;

        if (guidBuffer.Count >= GameSettings.guidBufferCapacity)
            guidBuffer.Dequeue();
        guidBuffer.Enqueue(guid);
        return true;
    }

    /// <summary>
    /// Performs a hit scan check to detect if a damageable object is hit by a raycast.
    /// </summary>
    /// <param name="damage">The damage to apply if a hit is detected.</param>
    /// <param name="origin">The origin of the raycast.</param>
    /// <param name="dir">The direction of the raycast.</param>
    /// <param name="distance">The maximum distance of the raycast.</param>
    /// <param name="hit">The RaycastHit information.</param>
    /// <param name="mask">The layer mask for the raycast.</param>
    /// <returns>True if a hit is detected, false otherwise.</returns>
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

    /// <summary>
    /// Performs a hitbox check to detect if damageable objects are hit within a specific area.
    /// </summary>
    /// <param name="damage">The damage to apply if hits are detected.</param>
    /// <param name="origin">The origin of the hitbox.</param>
    /// <param name="end">The end point of the hitbox.</param>
    /// <param name="radius">The radius of the hitbox.</param>
    /// <param name="hits">An array of RaycastHit information for each hit.</param>
    /// <param name="mask">The layer mask for the hitbox check.</param>
    /// <param name="coverMask">The layer mask for cover detection (optional).</param>
    /// <returns>True if at least one hit is detected, false otherwise.</returns>
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

    /// <summary>
    /// Performs an area-of-effect hit check to detect damageable objects within a specified radius.
    /// </summary>
    /// <param name="origin">The origin of the area-of-effect.</param>
    /// <param name="radius">The radius of the area-of-effect.</param>
    /// <param name="mask">The layer mask for the area-of-effect check.</param>
    /// <param name="damage">The damage to apply to each detected damageable object.</param>
    /// <param name="colliders">An array of colliders for the detected damageable objects.</param>
    /// <param name="damageFalloff">The damage falloff curve (optional).</param>
    /// <param name="coverMask">The layer mask for cover detection (optional).</param>
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
