public class Damage
{
    public System.Guid guid;
    public float amount;
    public float shieldMultiplier = 1f;
    public DamageType damageType;

    public Damage(
        float amount,
        float shieldMultiplier,
        DamageType damageType)
    {
        guid = System.Guid.NewGuid();
        this.amount = amount;
        this.shieldMultiplier = shieldMultiplier;
        this.damageType = damageType;
    }
}

public enum DamageType
{
    Normal,
    Energy,
    Fire,
    Shock
}
