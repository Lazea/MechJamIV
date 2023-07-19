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
        bool bypassShields,
        IDamageable damageable,
        out Damage damageDealt)
    {
        damageDealt = null;

        // Check GUID before applying damage
        if (!damageable.CheckGUIDIsInBuffer(damage.guid))
            return;

        int shieldDamage = 0;
        int totalDamage = 0;
        if (!bypassShields)
        {
            shieldDamage = Mathf.RoundToInt(damage.amount * damage.shieldMultiplier);
            totalDamage = (shieldDamage >= damageable.Shield) ? damageable.Shield : shieldDamage;

            damageable.Shield -= shieldDamage;
        }

        if (damageable.Shield <= 0 || bypassShields)
        {
            int healthDamage = Mathf.RoundToInt(
                (shieldDamage - totalDamage) / damage.shieldMultiplier);
            damageable.Shield = 0;
            damageable.Health -= healthDamage;
            if (damageable.Health <= 0)
                damageable.Health = 0;

            totalDamage += healthDamage;
        }

        damageDealt = new Damage(damage);
        damageDealt.amount = totalDamage;
    }

    public static void ApplyDamage(
        float amount,
        float shieldMultiplier,
        bool bypassShields,
        IDamageable damageable,
        out Damage damageDealt)
    {
        int shieldDamage = 0;
        int totalDamage = 0;
        if (!bypassShields)
        {
            shieldDamage = Mathf.RoundToInt(amount * shieldMultiplier);
            totalDamage = (shieldDamage >= damageable.Shield) ? damageable.Shield : shieldDamage;

            damageable.Shield -= shieldDamage;

            if (damageable.Shield <= 0)
            {
                int healthDamage = Mathf.RoundToInt(
                    (shieldDamage - totalDamage) / shieldMultiplier);
                damageable.Shield = 0;
                damageable.Health -= healthDamage;
                if (damageable.Health <= 0)
                    damageable.Health = 0;

                totalDamage += healthDamage;
            }
        }
        else
        {
            totalDamage = Mathf.RoundToInt(amount);
            damageable.Health -= totalDamage;
            if (damageable.Health <= 0)
                damageable.Health = 0;
        }

        damageDealt = new Damage(
            totalDamage,
            shieldMultiplier,
            false,
            false,
            null,
            DamageType.Fire,
            0f,
            0f,
            null);
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
    /// <param name="origin">The origin of the raycast.</param>
    /// <param name="dir">The direction of the raycast.</param>
    /// <param name="distance">The maximum distance of the raycast.</param>
    /// <param name="hit">The RaycastHit information.</param>
    /// <param name="mask">The layer mask for the raycast.</param>
    /// <param name="damage">The damage to apply if a hit is detected (optional).</param>
    /// <returns>True if a hit is detected, false otherwise.</returns>
    public static bool HitScanCheck(
        Vector3 origin,
        Vector3 dir,
        float distance,
        out RaycastHit hit,
        LayerMask mask,
        Damage damage = default)
    {
        Ray ray = new Ray(origin, dir);
        if(Physics.Raycast(ray, out hit, distance, mask))
        {
            if(damage != default)
            {
                IDamageable damageable = hit.collider.GetComponentInParent<IDamageable>();
                if (damageable != null)
                {
                    damageable.ApplyDamage(damage);
                }
            }

            return true;
        }

        return false;
    }

    /// <summary>
    /// Performs a hitbox check to detect if damageable objects are hit within a specific area.
    /// </summary>
    /// <param name="origin">The origin of the hitbox.</param>
    /// <param name="end">The end point of the hitbox.</param>
    /// <param name="radius">The radius of the hitbox.</param>
    /// <param name="hits">An array of RaycastHit information for each hit.</param>
    /// <param name="mask">The layer mask for the hitbox check.</param>
    /// <param name="coverMask">The layer mask for cover detection (optional).</param>
    /// <param name="damage">The damage to apply if hits are detected (optional).</param>
    /// <returns>True if at least one hit is detected, false otherwise.</returns>
    public static bool HitboxCheck(
        Vector3 origin,
        Vector3 end,
        float radius,
        out RaycastHit[] hits,
        LayerMask mask,
        LayerMask coverMask = default,
        Damage damage = default)
    {
        Vector3 disp = end - origin;
        Ray ray = new Ray(origin, disp.normalized);
        hits = Physics.SphereCastAll(ray, radius, disp.magnitude, mask);
        bool hitConfirm = false;
        foreach(RaycastHit hit in hits)
        {
            if (coverMask == default)
            {
                if(damage != default)
                {
                    IDamageable damageable = hit.collider.GetComponentInParent<IDamageable>();
                    if (damageable != null)
                        damageable.ApplyDamage(damage);
                }

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
                    if(damage != default)
                    {
                        IDamageable damageable = hit.collider.GetComponentInParent<IDamageable>();
                        if (damageable != null)
                            damageable.ApplyDamage(damage);
                    }

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
    /// <param name="colliders">An array of colliders for the detected damageable objects.</param>
    /// <param name="damage">The damage to apply to each detected damageable object (optional).</param>
    /// <param name="damageFalloff">The damage falloff curve (optional).</param>
    /// <param name="coverMask">The layer mask for cover detection (optional).</param>
    public static void AOEHitCheck(
        Vector3 origin,
        float radius,
        LayerMask mask,
        out Collider[] colliders,
        Damage damage = default,
        AnimationCurve damageFalloff = default,
        LayerMask coverMask = default)
    {
        origin += Vector3.up * 0.1f;
        colliders = Physics.OverlapSphere(origin, radius, mask);
        foreach(Collider c in colliders)
        {
            Vector3 disp = c.transform.position - origin;
            if (coverMask != default)
            {
                if (!Physics.Raycast(origin, disp.normalized, disp.magnitude, coverMask))
                {
                    if(damage != default)
                    {
                        IDamageable damageable = c.GetComponentInParent<IDamageable>();
                        if (damageable != null)
                        {
                            if (damageFalloff != null)
                            {
                                Damage _damage = damage.Clone();
                                float k = damageFalloff.Evaluate(disp.magnitude / radius);
                                _damage.amount *= k;
                            }

                            damageable.ApplyDamage(damage);
                        }
                        
                        if(c.attachedRigidbody != null)
                        {
                            c.attachedRigidbody.AddExplosionForce(
                                damage.pushForce,
                                origin,
                                radius,
                                damage.upPushForce,
                                ForceMode.Impulse
                            );
                        }
                    }
                }
            }
            else
            {
                if(damage != default)
                {
                    IDamageable damageable = c.GetComponentInParent<IDamageable>();
                    if (damageable != null)
                    {
                        if (damageFalloff != null)
                        {
                            Damage _damage = damage.Clone();
                            float k = damageFalloff.Evaluate(disp.magnitude / radius);
                            _damage.amount *= k;
                        }

                        damageable.ApplyDamage(damage);
                    }

                    if(c.attachedRigidbody != null)
                        {
                            c.attachedRigidbody.AddExplosionForce(
                                damage.pushForce,
                                origin,
                                radius,
                                damage.upPushForce,
                                ForceMode.Impulse
                            );
                        }
                }
            }
        }
    }

    public static void SpawnProjectile(
        Vector3 point,
        Quaternion orientation,
        GameObject projectile,
        GameObject source)
    {
        var _projectile = GameObject.Instantiate(
            projectile,
            point,
            orientation).GetComponent<IProjectile>();
        _projectile.Damage.source = source;
    }

    public static void SpawnProjectile(
        Vector3 point,
        Quaternion orientation,
        float damageMultiplier,
        float shieldDamageMultiplier,
        float critChance,
        float critDamageMultiplier,
        float fireChance,
        int fireDamage,
        float fireDuration,
        float shockChance,
        float shockDuration,
        GameObject projectile,
        GameObject source)
    {
        var _projectile = GameObject.Instantiate(
            projectile,
            point,
            orientation).GetComponent<IProjectile>();
        _projectile.Damage.source = source;

        _projectile.Damage.amount *= damageMultiplier;
        _projectile.Damage.shieldMultiplier = shieldDamageMultiplier;

        if (Random.Range(0f, 1f) <= critChance)
        {
            _projectile.Damage.amount *= critDamageMultiplier;
            _projectile.Damage.isCrit = true;
        }

        if(_projectile.Damage.damageType == DamageType.Fire)
        {
            if (Random.Range(0f, 1f) <= fireChance)
            {
                _projectile.Damage.damageEffect.damage = fireDamage;
                _projectile.Damage.damageEffect.lifeTime = fireDuration;
                _projectile.Damage.hasEffect = true;
            }
        }

        if (_projectile.Damage.damageType == DamageType.Shock)
        {
            if (Random.Range(0f, 1f) <= shockChance)
            {
                _projectile.Damage.damageEffect.damage = 0;
                _projectile.Damage.damageEffect.lifeTime = shockDuration;
                _projectile.Damage.hasEffect = true;
            }
        }

        _projectile.Damage = _projectile.Damage.Clone();
    }
}
