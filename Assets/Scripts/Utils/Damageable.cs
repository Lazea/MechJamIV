using System;
using System.Collections.Generic;
using UnityEngine;

public class Damageable : MonoBehaviour, IDamageable
{
    public GameObject GameObject { get { return gameObject; } }
    Queue<Guid> guidBuffer = new Queue<Guid>(GameSettings.guidBufferCapacity);
    public int health;
    public int Health
    {
        get { return health; }
        set { health = value; }
    }
    public int Shield
    {
        get { return 0; }
        set { ; }
    }

    // Start is called before the first frame update
    void Start()
    {
        
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
        Damage damageDealt = null;
        CombatUtils.ApplyDamage(damage, this, out damageDealt);

        if (health <= 0)
            DestroyDamageable();
    }

    [ContextMenu("Kill Player")]
    public void DestroyDamageable()
    {
        health = 0;

        // TODO: Set anim state to "Death" and destroy on animation event
        Destroy(gameObject);
    }
}
