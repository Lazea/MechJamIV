using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CombatUtils
{
    public static void ApplyDamage()
    {

    }

    public static void ApplyDamageEffect()
    {

    }

    public static void HitScanCheck(
        Vector3 origin,
        Vector3 dir,
        float distance,
        LayerMask mask)
    {
        Ray ray = new Ray(origin, dir);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, distance, mask))
        {

        }
    }

    public static void HitboxCheck(
        Vector3 origin,
        Vector3 dir,
        float distance,
        float radius,
        LayerMask mask)
    {
        Ray ray = new Ray(origin, dir);
        RaycastHit[] hits = Physics.SphereCastAll(ray, radius, distance, mask);
        foreach(RaycastHit hit in hits)
        {

        }
    }

    public static void SphereHitCheck(
        Vector3 origin,
        float radius,
        LayerMask mask,
        LayerMask coverMask = default)
    {
        Collider[] colliders = Physics.OverlapSphere(origin, radius, mask);
        foreach(Collider c in colliders)
        {
            Vector3 disp = c.transform.position - origin;
            if(!Physics.Raycast(origin, disp.normalized, disp.magnitude, coverMask))
            {

            }
        }
    }
}
