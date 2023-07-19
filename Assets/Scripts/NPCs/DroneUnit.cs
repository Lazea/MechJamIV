using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DroneUnit : BaseNPC
{
    [Header("Aim Transforms")]
    public Transform aim;
    public Transform aimBase;
    public float minLookAngle;
    public float maxLookAngle;

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

        HandleDamageEffects();

        if (isStunned)
            Stop();
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
