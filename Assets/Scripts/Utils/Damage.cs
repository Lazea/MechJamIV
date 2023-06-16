[System.Serializable]
public class Damage
{
    public System.Guid guid;
    public float amount = 10f;
    public float shieldMultiplier = 1f;
    public DamageType damageType;

    public Damage(
        float amount,
        float shieldMultiplier,
        DamageType damageType)
    {
        guid = System.Guid.NewGuid();
        UnityEngine.Debug.Log("Damage new GUID: " + guid);
        this.amount = amount;
        this.shieldMultiplier = shieldMultiplier;
        this.damageType = damageType;
    }

    public Damage Clone()
    {
        return new Damage(amount, shieldMultiplier, damageType);
    }
}

public enum DamageType
{
    Normal,
    Energy,
    Fire,
    Shock
}
