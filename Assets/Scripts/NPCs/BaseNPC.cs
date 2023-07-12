using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using SOGameEventSystem.Events;

public class BaseNPC : MonoBehaviour, IDamageable, IActor
{
    [Header("Data")]
    public ActorData data;
    public ActorData Data { get { return data;} }
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
    
    [Header("NPC State")]
    public NPCMovementState moveState;
    public NPCFireState fireState;
    public enum NPCMovementState
    {
        Idle,
        Moving,
        Chasing
    }
    public enum NPCFireState
    {
        Idle,
        Firing
    }
    
    [SerializeField]
    protected bool isDead;
    [SerializeField]
    protected bool isStunned;
    [SerializeField]
    protected bool hasTarget;
    [SerializeField]
    protected bool targetObscured;
    [SerializeField]
    protected bool targetInReach;
    [SerializeField]
    protected bool targetInCombatRange;
    [SerializeField]
    protected bool targetInAim;
    public LayerMask mask;

    // Target position
    [SerializeField]
    protected float targetDistance;
    [SerializeField]
    protected float targetAngle;
    [SerializeField]
    protected Vector3 targetDir;

    [Header("Aim")]
    public Transform aim;

    [Header("Projectile")]
    public GameObject projectile;
    public Transform projectileSpawn;

    [Header("Events")]
    public TransformGameEvent onDeath;
    public IntGameEvent onCreditsDeath;

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
        if(agent != null)
        {
            agent.avoidancePriority = UnityEngine.Random.Range(0, 75);
        }
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        CalculateTargetPosition();
        
        hasTarget = (GameManager.Instance.Player != null);
        targetObscured = IsTargetObscured();
        targetInReach = IsTargetInReach(data.chaseRange);
        targetInCombatRange = IsTargetInReach(data.combatRange);
        targetInAim = IsTargetInAim();
    }

    #region [Target States]
    protected virtual void CalculateTargetPosition()
    {
        if(GameManager.Instance.Player != null)
        {
            Vector3 disp = GameManager.Instance.PlayerCenter - aim.position;
            targetDistance = disp.magnitude;
            targetDir = disp.normalized;
            targetAngle = Vector3.Angle(targetDir, aim.forward);
        }
        else
        {
            targetDistance = 1000f;
            targetAngle = 180f;
            targetDir = Vector3.zero;
        }
    }

    public virtual bool IsTargetObscured()
    {
        Ray ray = new Ray(aim.position + Vector3.up, targetDir);
        return Physics.Raycast(ray, targetDistance, mask);
    }

    public virtual bool IsTargetInReach(float range)
    {
        if(targetDistance > range)
            return false;
            
        return !targetObscured;
    }

    public virtual bool IsTargetInAim()
    {
        if(targetAngle > data.aimAngle)
            return false;

        return !targetObscured;
    }
    #endregion

    #region [Health & Shield Handlers]
    public bool CheckGUIDIsInBuffer(Guid guid)
    {
        return CombatUtils.CheckGUIDIsInBuffer(guidBuffer, guid);
    }

    public void ApplyDamage(Damage damage)
    {
        if(health <= 0)
            return;

        CombatUtils.ApplyDamage(damage, this);

        if (shield <= 0)
            BreakShield();

        if (health <= 0)
            Kill();
    }

    [ContextMenu("Break Shield")]
    public void BreakShield()
    {
        shield = 0;

        // TODO: Call fx to provide feedback
    }

    [ContextMenu("Kill Unit")]
    public void Kill()
    {
        health = 0;
        // TODO: Set anim state to "Death" and destroy on animation event
        onDeath.Raise(transform);
        onCreditsDeath.Raise(data.credits);
        Destroy(gameObject);
    }
    #endregion

    #region [Movement]
    public void GoToTarget()
    {
        agent.SetDestination(GameManager.Instance.Player.transform.position);
        agent.isStopped = false;
    }

    public Vector3 GetStrafePosition()
    {
        Vector3 strafePosition = transform.right * UnityEngine.Random.Range(7f, 25f);
        strafePosition *= (float)UnityEngine.Random.Range(-1, 2);
        strafePosition += transform.forward * UnityEngine.Random.Range(-2f, 2f);
        return strafePosition;
    }

    public void Stop()
    {
        agent.isStopped = true;
    }

    public bool IsAtDestination()
    {
        float dist = Vector3.Distance(agent.destination, transform.position);
        return (dist <= agent.stoppingDistance);
    }
    #endregion

    [ContextMenu("Spawn Projectile")]
    public void SpawnProjectile()
    {
        Instantiate(
            projectile,
            projectileSpawn.position,
            projectileSpawn.rotation).GetComponent<Projectile>();
    }

    // Return angle in range of -180 to 180
    protected float NormalizeAngle(float a)
    {
        return (a > 180f) ? a - 360f : a;
    }

    protected virtual void HandleAim(Transform gunBase, Transform gunBarrel, float minLookAngle, float maxLookAngle)
    {
        Vector3 aimDir = GameManager.Instance.PlayerCenter - gunBase.position;
        var rot = Quaternion.LookRotation(aimDir.normalized);

        gunBase.rotation = Quaternion.Lerp(gunBase.rotation, rot, data.aimSpeed * Time.deltaTime);
        Vector3 topEulerAngles = gunBase.rotation.eulerAngles;
        topEulerAngles.x = 0f;
        topEulerAngles.z = 0f;
        gunBase.rotation = Quaternion.Euler(topEulerAngles);

        gunBarrel.rotation = Quaternion.Lerp(gunBarrel.rotation, rot, data.aimSpeed * Time.deltaTime);
        Vector3 gunEulerAngles = gunBarrel.localRotation.eulerAngles;
        gunEulerAngles.y = 0f;
        gunEulerAngles.z = 0f;
        gunEulerAngles.x = NormalizeAngle(gunEulerAngles.x);
        gunEulerAngles.x = Mathf.Clamp(gunEulerAngles.x, minLookAngle, maxLookAngle);
        gunBarrel.localRotation = Quaternion.Euler(gunEulerAngles);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, data.chaseRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, data.combatRange);

        if(agent != null && agent.hasPath)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(agent.destination, 1f);
            Gizmos.DrawLine(transform.position, agent.destination);
        }
    }
}
