using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    public bool CheckGUIDIsInBuffer(System.Guid guid);
    public int Health { get; set; }
    public int Shield { get; set; }
    public GameObject GameObject { get; }

    public void ApplyDamage(Damage damage);
}
