[System.Serializable]
public class Damage
{
    public System.Guid guid;
    public float amount = 10f;
    public float shieldMultiplier = 1f;
    public DamageType damageType;
    public float pushForce;
    public float upPushForce;

    public Damage(
        float amount,
        float shieldMultiplier,
        DamageType damageType,
        float pushForce,
        float upPushForce)
    {
        guid = System.Guid.NewGuid();
        this.amount = amount;
        this.shieldMultiplier = shieldMultiplier;
        this.damageType = damageType;
        this.pushForce = pushForce;
        this.upPushForce = upPushForce;
    }

    public Damage Clone()
    {
        return new Damage(amount, shieldMultiplier, damageType, pushForce, upPushForce);
    }
}
