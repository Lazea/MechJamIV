using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    public bool invulnerable;
    public bool killable;

    [Header("Events")]
    public SOGESys.Events.DamageGameEvent onHit;
    public SOGESys.Events.IntGameEvent onHealthChange;
    public SOGESys.Events.IntGameEvent onShieldChange;
    public SOGESys.BaseGameEvent onDeath;

    // Start is called before the first frame update
    void Start()
    {
        data.health = data.MaxHealth;
        data.shield = data.MaxShield;

        onHealthChange.Raise(data.health);
        onShieldChange.Raise(data.shield);
    }

    // Update is called once per frame
    void Update()
    {
        
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

        if(invulnerable)
            return;

        Damage damageDealt = null;
        CombatUtils.ApplyDamage(damage, this, out damageDealt);

        onHealthChange.Raise(data.health);
        onShieldChange.Raise(data.shield);

        if (data.health <= 0)
            Kill();
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

        onDeath.Raise();

        // TODO: Set anim state to "Death" and destroy on animation event
        Destroy(gameObject);
    }
}
