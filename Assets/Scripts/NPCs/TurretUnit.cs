using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretUnit : MonoBehaviour, IDamageable
{
    [SerializeField]
    ActorData data;

    Queue<Guid> guidBuffer = new Queue<Guid>(GameSettings.guidBufferCapacity);
    public int health;
    public int Health
    {
        get { return health; }
        set { health = value; }
    }
    public int shield;
    public int Shield
    {
        get { return shield; }
        set { shield = value; }
    }

    Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        health = data.health;
        shield = data.shield;

        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool CheckGUIDIsInBuffer(Guid guid)
    {
        return CombatUtils.CheckGUIDIsInBuffer(guidBuffer, guid);
    }

    public void ApplyDamage(Damage damage)
    {
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
        Destroy(gameObject);
    }
}
