using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IDamageable
{
    public PlayerData data;

    Queue<Guid> guidBuffer = new Queue<Guid>(GameSettings.guidBufferCapacity);
    public int Health
    {
        get { return data.Health; }
        set { data.Health = value; }
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
        CombatUtils.ApplyDamage(damage, this);

        if (data.Health <= 0)
            Kill();
    }

    [ContextMenu("Kill Player")]
    public void Kill()
    {
        data.Health = 0;

        // TODO: Set anim state to "Death" and destroy on animation event
        Destroy(gameObject);
    }
}
