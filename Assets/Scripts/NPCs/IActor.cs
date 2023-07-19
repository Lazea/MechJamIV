using SOGameEventSystem.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface IActor
{
    public ActorData Data { get; }
    public GameObject GameObject { get; }

    // NPC State
    public bool IsKillable { get; set; }
    public bool IsInvincible { get; set; }
    public bool IsDead { get; }
    public bool IsStunned { get; }
    public bool HasTarget { get; }
    public bool TargetObscured { get; }
    public bool TargetInReach { get; }
    public bool TargetInCombatRange { get; }
    public bool TargetInAim { get; }

    // Target position
    public float TargetDistance { get; }
    public float TargetAngle { get; }
    public Vector3 TargetDir { get; }

    // Events
    public UnityEvent OnShotFired { get ; }
    public UnityEvent<Damage> OnDamage { get; }
    public UnityEvent OnKilled { get; }
    public TransformGameEvent OnDeath { get; }
    public IntGameEvent OnCreditsDeath { get; }

    // Projectile
    public void SpawnProjectiles();
    public void SpawnProjectile(Vector3 point, Quaternion orientation);

    // Movement
    public void SetPlayerAsDestination(Vector3 offset);
    public void SetStrafePosition(float minRange, float maxRange);
    public void GoToDestination();
    public void Stop();
    public bool IsAtDestination();
    public bool HasPath();
}
