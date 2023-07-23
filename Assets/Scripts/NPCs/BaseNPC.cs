using SOGameEventSystem.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class BaseNPC : MonoBehaviour, IDamageable, IActor
{
    [Header("Data")]
    [SerializeField]
    protected ActorData data;
    public ActorData Data { get { return data; } }
    public GameObject GameObject { get { return gameObject; } }

    [Header("Health")]
    [SerializeField]
    protected int health;
    public int Health
    {
        get { return health; }
        set { health = value; }
    }
    [SerializeField]
    protected int shield;
    public int Shield
    {
        get { return shield; }
        set { shield = value; }
    }
    Queue<Guid> guidBuffer = new Queue<Guid>(GameSettings.guidBufferCapacity);

    [Header("Damage Effects")]
    public DamageEffect shockDamageEffect;
    public DamageEffect fireDamageEffect;

    [Header("Projectile")]
    public GameObject projectile;
    public Transform[] projectileSpawns;

    [Header("Vulnerability")]
    [SerializeField]
    protected bool isKillable;
    public bool IsKillable
    {
        get { return isKillable; }
        set { isKillable = value; }
    }
    [SerializeField]
    protected bool isInvincible;
    public bool IsInvincible
    {
        get { return isInvincible; }
        set { isInvincible = value; }
    }

    [Header("NPC State")]
    protected bool isDead;
    public bool IsDead { get { return isDead; } }
    protected bool tookDamage;
    protected bool isStunned;
    public bool IsStunned { get { return isStunned; } }
    protected bool hasTarget;
    public bool HasTarget { get { return hasTarget; } }
    protected bool targetObscured;
    public bool TargetObscured { get { return targetObscured; } }
    protected bool targetInReach;
    public bool TargetInReach { get { return targetInReach; } }
    protected bool targetInCombatRange;
    public bool TargetInCombatRange { get { return targetInCombatRange; } }
    protected bool targetInAim;
    public bool TargetInAim { get { return targetInAim; } }
    public LayerMask coverMask;

    // Target position
    protected float targetDistance;
    public float TargetDistance { get { return targetDistance; } }
    protected float targetAngle;
    public float TargetAngle { get { return targetAngle; } }
    protected Vector3 targetDir;
    public Vector3 TargetDir { get { return targetDir; } }

    [Header("Mesh Renderers")]
    public MeshRenderer[] meshRenderers;
    public float shieldPowerOnHit = 0.15f;
    public float shieldSmooth = 0.1f;
    bool shieldBroken;
    public ParticleSystem[] shieldBreakFX;

    [Header("Effects On Kill")]
    public GameObject[] effects;
    public GameObject[] debris;

    [Header("Events")]
    [SerializeField]
    protected UnityEvent onShotFired;
    public UnityEvent OnShotFired { get { return onShotFired; } }
    [SerializeField]
    protected UnityEvent<Damage> onDamage;
    public UnityEvent<Damage> OnDamage { get { return onDamage; } }
    [SerializeField]
    protected UnityEvent onFireDamage;
    [SerializeField]
    protected UnityEvent onShockStart;
    [SerializeField]
    protected UnityEvent onShockEnd;
    [SerializeField]
    protected UnityEvent onKilled;
    public UnityEvent OnKilled { get { return onKilled; } }
    [SerializeField]
    protected TransformGameEvent onDeath;
    public TransformGameEvent OnDeath { get { return onDeath; } }
    [SerializeField]
    protected IntGameEvent onCreditsDeath;
    public IntGameEvent OnCreditsDeath { get { return onCreditsDeath; } }

    // Components
    protected Animator anim;
    protected NavMeshAgent agent;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        health = data.health;
        shield = data.shield;

        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        CalculateTargetPosition(transform.position, transform.forward);

        hasTarget = (GameManager.Instance.Player != null);
        targetObscured = IsTargetObscured(transform.position, coverMask);
        if (tookDamage)
            targetInReach = true;
        else
            targetInReach = IsTargetInReach(data.chaseRange);
        targetInCombatRange = IsTargetInReach(data.combatRange);
        targetInAim = IsTargetInAim();

        HandleDamageEffects();
        HandleShieldFX();

        if (isStunned)
            Stop();
    }

    #region [Target States]
    protected virtual void CalculateTargetPosition(Vector3 point, Vector3 forward)
    {
        if (GameManager.Instance.Player != null)
        {
            Vector3 disp = GameManager.Instance.PlayerCenter - point;
            targetDistance = disp.magnitude;
            targetDir = disp.normalized;
            targetAngle = Vector3.Angle(targetDir, forward);
        }
        else
        {
            targetDistance = 1000f;
            targetAngle = 180f;
            targetDir = Vector3.zero;
        }
    }

    public virtual bool IsTargetObscured(Vector3 point, LayerMask mask)
    {
        Ray ray = new Ray(point + Vector3.up, targetDir);
        return Physics.Raycast(ray, targetDistance, mask);
    }

    public virtual bool IsTargetInReach(float range)
    {
        if (targetDistance > range)
            return false;

        return !targetObscured;
    }

    public virtual bool IsTargetInAim()
    {
        if (targetAngle > data.aimAngle)
            return false;

        return !targetObscured;
    }
    #endregion

    #region [Health & Shield Handlers]
    public virtual bool CheckGUIDIsInBuffer(Guid guid)
    {
        return CombatUtils.CheckGUIDIsInBuffer(guidBuffer, guid);
    }

    public virtual void ApplyDamage(Damage damage)
    {
        tookDamage = true;

        if (isInvincible)
            return;

        if (health <= 0)
            return;

        if (isDead)
            return;

        if(anim != null)
            anim.SetTrigger("Hurt");

        Damage damageDealt = null;
        CombatUtils.ApplyDamage(damage, false, this, out damageDealt);
        ApplyDamageEffect(damage);

        if (shield <= 0 && !shieldBroken)
        {
            shieldBroken = true;
            BreakShield();
        }
        else if(!shieldBroken)
        {
            PlayShieldHit();
        }

        if (damageDealt != null)
        {
            onDamage.Invoke(damageDealt);
        }

        if (health <= 0)
            Kill();
    }

    protected void HandleDamageEffects()
    {
        if (isDead)
        {
            if (anim != null)
                anim.SetBool("Stunned", false);
            isStunned = false;
            onShockEnd.Invoke();
            return;
        }

        if (shockDamageEffect != null)
        {
            if (shockDamageEffect.IsEffectComplete())
            {
                shockDamageEffect = null;
                if (anim != null)
                    anim.SetBool("Stunned", false);
                isStunned = false;
                onShockEnd.Invoke();
            }
            else
            {
                shockDamageEffect.UpdateEffect();
                if (anim != null)
                    anim.SetBool("Stunned", true);
                isStunned = true;
            }
        }

        Damage damageDealt = null;
        if (fireDamageEffect != null)
        {
            if (fireDamageEffect.IsEffectComplete())
            {
                fireDamageEffect = null;
            }
            else
            {
                fireDamageEffect.UpdateEffect();
                if (fireDamageEffect.CanDealDamage())
                {
                    CombatUtils.ApplyDamage(
                        fireDamageEffect.damage,
                        0f,
                        true,
                        this,
                        out damageDealt);

                    if (anim != null)
                        anim.SetTrigger("Hurt");

                    onFireDamage.Invoke();

                    if (shield <= 0)
                        BreakShield();

                    if (damageDealt != null)
                    {
                        onDamage.Invoke(damageDealt);
                    }

                    if (health <= 0)
                        Kill();
                }
            }
        }
    }

    protected void ApplyDamageEffect(Damage damage)
    {
        if (!damage.hasEffect)
            return;

        switch (damage.damageType)
        {
            case DamageType.Fire:
                if (fireDamageEffect == null)
                {
                    fireDamageEffect = damage.damageEffect;
                    onFireDamage.Invoke();
                }
                break;
            case DamageType.Shock:
                if (shockDamageEffect == null)
                {
                    shockDamageEffect = damage.damageEffect;
                    onShockStart.Invoke();
                }
                break;
        }
    }

    [ContextMenu("Break Shield")]
    public virtual void BreakShield()
    {
        shield = 0;
        PlayShieldBreak();
    }

    [ContextMenu("Kill Unit")]
    public virtual void Kill()
    {
        if (!isKillable)
            return;

        health = 0;

        if (anim != null)
            anim.SetBool("Dead", true);
        isDead = true;

        onKilled.Invoke();
        onDeath.Raise(transform);
        onCreditsDeath.Raise(data.credits);
    }

    public virtual void DestroyUnit()
    {
        foreach(var e in effects)
        {
            e.transform.parent = null;
            e.SetActive(true);
        }

        foreach(var d in debris)
        {
            d.transform.parent = null;
            d.SetActive(true);
        }

        Destroy(gameObject);
    }
    #endregion

    #region [Movement]
    public void SetPlayerAsDestination(Vector3 offset)
    {
        if(hasTarget)
        {
            agent.SetDestination(
                GameManager.Instance.Player.transform.position + offset);
        }
        else
        {
            Stop();
        }
    }

    public void SetStrafePosition(float minRange, float maxRange)
    {
        float range = UnityEngine.Random.Range(minRange, maxRange);
        float dir = UnityEngine.Random.Range(0f, 1f);
        range *= (dir >= 0.5f) ? 1 : -1;
        float absRange = Mathf.Abs(range);

        Vector3 right = new Vector3(targetDir.z, targetDir.y, -targetDir.x);
        Vector3 position = transform.position;
        position += right * range;
        position += targetDir * absRange * 0.5f;

        Vector3 destination = transform.position;
        int tries = 0;
        while(true)
        {
            NavMeshHit hit;
            if(NavMesh.SamplePosition(position, out hit, absRange * 2f, agent.areaMask))
            {
                destination = hit.position;
                break;
            }

            tries++;
            if (tries >= 5)
                break;
        }

        agent.SetDestination(destination);
    }

    public void GoToDestination()
    {
        if (agent != null)
            agent.isStopped = false;
    }

    public void Stop()
    {
        if(agent != null)
            agent.isStopped = true;
    }

    public bool IsAtDestination()
    {
        float dist = Vector3.Distance(agent.destination, transform.position);
        return (dist <= agent.stoppingDistance);
    }

    public bool HasPath()
    {
        return agent.hasPath;
    }
    #endregion

    [ContextMenu("Spawn Projectiles")]
    public virtual void SpawnProjectiles()
    {
        if (isStunned)
            return;

        foreach (var p in projectileSpawns)
            SpawnProjectile(p.position, p.rotation);
        anim.SetTrigger("Fire");
        onShotFired.Invoke();
    }

    public virtual void SpawnProjectile(Vector3 point, Quaternion orientation)
    {
        if (isStunned)
            return;

        CombatUtils.SpawnProjectile(
            point,
            orientation,
            projectile,
            gameObject);
    }

    // Return angle in range of -180 to 180
    public static float NormalizeAngle(float a)
    {
        return (a > 180f) ? a - 360f : a;
    }

    public static void HandleAim(
        Transform gunBase,
        Transform gunBarrel,
        float minLookAngle,
        float maxLookAngle,
        float aimSpeed)
    {
        Vector3 aimDir = GameManager.Instance.PlayerCenter - gunBase.position;
        var rot = Quaternion.LookRotation(aimDir.normalized);

        gunBase.rotation = Quaternion.Lerp(gunBase.rotation, rot, aimSpeed * Time.deltaTime);
        Vector3 topEulerAngles = gunBase.rotation.eulerAngles;
        topEulerAngles.x = 0f;
        topEulerAngles.z = 0f;
        gunBase.rotation = Quaternion.Euler(topEulerAngles);

        gunBarrel.rotation = Quaternion.Lerp(gunBarrel.rotation, rot, aimSpeed * Time.deltaTime);
        Vector3 gunEulerAngles = gunBarrel.localRotation.eulerAngles;
        gunEulerAngles.y = 0f;
        gunEulerAngles.z = 0f;
        gunEulerAngles.x = NormalizeAngle(gunEulerAngles.x);
        gunEulerAngles.x = Mathf.Clamp(gunEulerAngles.x, minLookAngle, maxLookAngle);
        gunBarrel.localRotation = Quaternion.Euler(gunEulerAngles);
    }

    #region [Shield FX]
    protected void HandleShieldFX()
    {
        float shieldPower = Mathf.Lerp(
            GetShieldPower(),
            0f,
            shieldSmooth * Time.deltaTime);
        SetShieldPower(shieldPower);
    }

    [ContextMenu("Shield Hit")]
    public void PlayShieldHit()
    {
        SetShieldPower(shieldPowerOnHit);
    }

    [ContextMenu("Shield Break")]
    public void PlayShieldBreak()
    {
        SetShieldPower(0f);
        foreach (var fx in shieldBreakFX)
            fx.Play();
    }

    float GetShieldPower()
    {
        if(meshRenderers.Length > 0)
            return meshRenderers[0].material.GetFloat("_Shield_Power");
        return 0f;
    }

    void SetShieldPower(float value)
    {
        foreach (var mr in meshRenderers)
            mr.material.SetFloat("_Shield_Power", value);
    }
    #endregion

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, data.chaseRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, data.combatRange);

        if (agent != null && agent.hasPath)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(agent.destination, 1f);
            Gizmos.DrawLine(transform.position, agent.destination);
        }
    }
}
