using UnityEngine;

[System.Serializable]
public class Damage
{
    public System.Guid guid;
    public float amount = 10f;
    public float shieldMultiplier = 1f;
    public bool isCrit;
    public bool hasEffect;
    public DamageEffect damageEffect;
    public DamageType damageType;
    public float pushForce;
    public float upPushForce;

    public GameObject source;

    public Damage(
        float amount,
        float shieldMultiplier,
        bool isCrit,
        bool hasEffect,
        DamageEffect damageEffect,
        DamageType damageType,
        float pushForce,
        float upPushForce,
        GameObject source)
    {
        guid = System.Guid.NewGuid();
        this.amount = amount;
        this.shieldMultiplier = shieldMultiplier;
        this.isCrit = isCrit;
        this.hasEffect = hasEffect;
        this.damageEffect = damageEffect;
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
        isCrit = damage.isCrit;
        hasEffect = damage.hasEffect;
        damageEffect = damage.damageEffect;
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
            isCrit,
            hasEffect,
            damageEffect,
            damageType,
            pushForce,
            upPushForce,
            source);
    }
}

[System.Serializable]
public class DamageEffect
{
    [Header("Life Time")]
    public float lifeTime;
    protected float tm;

    [Header("Damage")]
    public int damage;

    public float damageInterval;
    float damageTime;
    bool dealDamage;

    public virtual void UpdateEffect()
    {
        tm += Time.deltaTime;

        if (damageInterval <= 0f)
            return;

        damageTime += Time.deltaTime;
        if (damageTime >= damageInterval)
        {
            damageTime = 0f;
            dealDamage = true;
        }
    }

    public virtual bool IsEffectComplete()
    {
        return tm >= lifeTime;
    }

    public virtual bool CanDealDamage()
    {
        bool returnBool = dealDamage;
        dealDamage = false;
        return returnBool;
    }
}
