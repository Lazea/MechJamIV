using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MechUnit : BaseNPC
{
    [Header("Aim Transforms")]
    public Transform aim;
    public float aimSmooth;
    Vector3 target;

    [Header("Move Smooth")]
    [Min(1f)]
    public float speedScale = 1f;
    public float moveSmooth;
    Vector3 vel;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        agent.updatePosition = false;
        agent.updateRotation = false;
    }

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

        // Handle aiming
        HandleAim();

        // Handle animation movement
        HandleMovement();

        // Handle Damage Effects
        HandleDamageEffects();

        // Handle shield effects
        HandleShieldFX();

        if (isStunned)
            Stop();
    }

    public void HandleAim()
    {
        if (!HasTarget)
            return;

        if (isStunned || isDead)
            return;

        Vector3 _target = GameManager.Instance.PlayerCenter;
        _target.y = 0f;
        target = Vector3.Lerp(
            target,
            _target,
            aimSmooth);
        target.y = transform.position.y;
        transform.LookAt(target);

        aim.LookAt(GameManager.Instance.PlayerCenter);
    }

    public void HandleMovement()
    {
        Vector3 worldDeltaPosition = agent.nextPosition - transform.position;
        worldDeltaPosition.y = 0f;
        worldDeltaPosition.Normalize();

        vel = Vector3.Lerp(
            vel,
            transform.InverseTransformVector(worldDeltaPosition),
            moveSmooth);
        anim.SetFloat("SpeedX", vel.x);
        anim.SetFloat("SpeedY", vel.z);
        anim.SetFloat("Speed", vel.magnitude * speedScale);
    }

    private void OnAnimatorMove()
    {
        Vector3 rootPosition = anim.rootPosition;
        rootPosition.y = agent.nextPosition.y;
        transform.position = rootPosition;
        agent.nextPosition = rootPosition;
    }
}
