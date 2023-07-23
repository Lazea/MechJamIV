using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretUnit : BaseNPC
{
    [Header("Aim Transforms")]
    public Transform aim;
    public Transform aimBase;
    public float minLookAngle;
    public float maxLookAngle;

    int maxShotCount = 6;
    int shotCount;
    float nextAttackTime;
    float nextAttackEventTime;

    Quaternion aimRot;
    Quaternion aimBaseRot;

    // Update is called once per frame
    protected override void Update()
    {
        CalculateTargetPosition(aim.position, aim.forward);

        hasTarget = (GameManager.Instance.Player != null);
        targetObscured = IsTargetObscured(aim.position, coverMask);
        if (tookDamage)
            targetInReach = true;
        else
            targetInReach = IsTargetInReach(data.chaseRange);
        targetInCombatRange = IsTargetInReach(data.combatRange);
        targetInAim = IsTargetInAim();

        if (HasTarget && !isStunned && !isDead)
        {
            if(TargetInCombatRange && TargetInAim)
            {
                float t = Time.time;
                if(t >= nextAttackEventTime)
                {
                    if(t >= nextAttackTime)
                    {
                        SpawnProjectiles();

                        shotCount++;
                        if(shotCount >= maxShotCount)
                        {
                            shotCount = 0;
                            
                            float dt = UnityEngine.Random.Range(data.minAttackEventRate, data.maxAttackEventRate);
                            nextAttackEventTime = Time.time + dt;
                            nextAttackTime = nextAttackEventTime;
                        }
                        else
                        {
                            nextAttackTime = Time.time + data.attackInterval;
                        }
                    }
                }
            }
        }
        else
        {
            shotCount = 0;

            float dt = UnityEngine.Random.Range(data.minAttackEventRate, data.maxAttackEventRate);
            nextAttackEventTime = Time.time + dt;
            nextAttackTime = nextAttackEventTime;
        }

        HandleDamageEffects();
        HandleShieldFX();
    }

    private void LateUpdate()
    {
        if (HasTarget && !isStunned && !isDead)
        {
            aimBase.rotation = aimRot;
            aim.rotation = aimBaseRot;
            BaseNPC.HandleAim(aimBase, aim, minLookAngle, maxLookAngle, data.aimSpeed);
            aimRot = aimBase.rotation;
            aimBaseRot = aim.rotation;
        }
        else
        {
            aimBase.rotation = aimRot;
            aim.rotation = aimBaseRot;
        }
    }
}
