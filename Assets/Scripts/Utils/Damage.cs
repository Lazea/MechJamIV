public class Damage
{
    public System.Guid guid;
    public float amount;
    public DamageType damageType;

    public Damage(float amount, DamageType damageType)
    {
        guid = System.Guid.NewGuid();
        this.amount = amount;
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
