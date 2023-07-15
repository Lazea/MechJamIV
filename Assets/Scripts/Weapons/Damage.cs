using UnityEngine;

[System.Serializable]
public class Damage
{
    public System.Guid guid;
    public float amount = 10f;
    public float shieldMultiplier = 1f;
    public bool isCrit;
    public DamageType damageType;
    public float pushForce;
    public float upPushForce;

    public GameObject source;

    public Damage(
        float amount,
        float shieldMultiplier,
        DamageType damageType,
        float pushForce,
        float upPushForce,
        GameObject source)
    {
        guid = System.Guid.NewGuid();
        this.amount = amount;
        this.shieldMultiplier = shieldMultiplier;
        this.damageType = damageType;
        this.pushForce = pushForce;
        this.upPushForce = upPushForce;
        this.source = source;
    }

    public Damage(Damage damage)
    {
        guid = System.Guid.NewGuid();
        amount = damage.amount;
        shieldMultiplier = damage.shieldMultiplier;
        damageType = damage.damageType;
        pushForce = damage.pushForce;
        upPushForce = damage.upPushForce;
        source = damage.source;
    }

    public Damage Clone()
    {
        return new Damage(
            amount,
            shieldMultiplier,
            damageType,
            pushForce,
            upPushForce,
            source);
    }
}
