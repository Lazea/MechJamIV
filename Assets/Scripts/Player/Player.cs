using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using SOGESys = SOGameEventSystem;

/// <summary>
/// Represents the player character in the game and implements damage-related functionality.
/// </summary>
public class Player : MonoBehaviour, IDamageable
{
    public PlayerData data;
    public GameObject GameObject { get { return gameObject; } }

    Queue<Guid> guidBuffer = new Queue<Guid>(GameSettings.guidBufferCapacity);
    public int Health
    {
        get { return data.health; }
        set { data.health = value; }
    }
    public int Shield
    {
        get { return data.shield; }
        set { data.shield = value; }
    }

    [Header("Damage Effects")]
    public DamageEffect shockDamageEffect;
    public DamageEffect fireDamageEffect;

    [Header("Vulnerability")]
    public bool invulnerable;
    public bool killable;

    [Header("Effects On Kill")]
    public GameObject[] effects;
    public GameObject[] debris;

    [Header("Events")]
    public UnityEvent onFireDamage;
    public UnityEvent onShockStart;
    public UnityEvent onShockEnd;
    public SOGESys.Events.DamageGameEvent onHit;
    public SOGESys.Events.IntGameEvent onHealthChange;
    public SOGESys.Events.IntGameEvent onShieldChange;
    public UnityEvent onDeathEvent;
    public SOGESys.BaseGameEvent onDeath;

    Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        if(data.resetHealthOnStart)
        {
            data.health = data.MaxHealth;
            data.resetHealthOnStart = false;
        }
        data.shield = data.MaxShield;

        onHealthChange.Raise(data.health);
        onShieldChange.Raise(data.shield);

        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        HandleDamageEffects();
    }

    void HandleDamageEffects()
    {
        if (data.health <= 0)
        {
            anim.SetBool("Stunned", false);
            onShockEnd.Invoke();
            return;
        }

        if (shockDamageEffect != null)
        {
            if (shockDamageEffect.IsEffectComplete())
            {
                shockDamageEffect = null;
                anim.SetBool("Stunned", false);
                onShockEnd.Invoke();
            }
            else
            {
                shockDamageEffect.UpdateEffect();
                anim.SetBool("Stunned", true);
            }
        }

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
                    Damage dmg = null;
                    CombatUtils.ApplyDamage(
                        fireDamageEffect.damage,
                        0f,
                        true,
                        this,
                        out dmg);

                    anim.SetTrigger("Hurt");
                    onFireDamage.Invoke();
                    onHealthChange.Raise(data.health);
                    onShieldChange.Raise(data.shield);

                    if (data.health <= 0)
                        Kill();
                }
            }
        }
    }

    /// <summary>
    /// Checks if the specified GUID is in the buffer.
    /// </summary>
    /// <param name="guid">The GUID to check.</param>
    /// <returns>True if the GUID is in the buffer, false otherwise.</returns>
    public bool CheckGUIDIsInBuffer(Guid guid)
    {
        return CombatUtils.CheckGUIDIsInBuffer(guidBuffer, guid);
    }

    /// <summary>
    /// Applies damage to the player character.
    /// </summary>
    /// <param name="damage">The damage to apply.</param>
    public void ApplyDamage(Damage damage)
    {
        onHit.Raise(damage);

        if (invulnerable)
            return;

        if (data.health <= 0)
            return;

        anim.SetTrigger("Hurt");

        Damage damageDealt = null;
        CombatUtils.ApplyDamage(damage, false, this, out damageDealt);
        ApplyDamageEffect(damage);

        onHealthChange.Raise(data.health);
        onShieldChange.Raise(data.shield);

        if (data.health <= 0)
            Kill();
    }

    void ApplyDamageEffect(Damage damage)
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

    /// <summary>
    /// Kills the player character.
    /// </summary>
    [ContextMenu("Kill Player")]
    public void Kill()
    {
        if(!killable)
            return;

        data.health = 0;

        anim.SetBool("Dead", true);
    }

    public void PlayerDeath()
    {
        foreach (var e in effects)
        {
            e.transform.parent = null;
            e.SetActive(true);
        }

        foreach (var d in debris)
        {
            d.transform.parent = null;
            d.SetActive(true);
        }

        onDeath.Raise();
        onDeathEvent.Invoke();

        Destroy(gameObject);
    }
}
