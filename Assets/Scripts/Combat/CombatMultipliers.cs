public class CombatMultipliers
{
    public float AttackSpeed { get; private set; } = 1f;
    public float AttackDamage { get; private set; } = 1f;
    public float MovementSpeed { get; private set; } = 1f;

    public void IncreaseAttackDamageMultiplier( float amount )
    {
        this.AttackDamage += amount;
    }

    public void IncreaseAttackSpeedMultiplier( float amount )
    {
        this.AttackSpeed += amount;
    }

    public void IncreaseMovementSpeed( float amount )
    {
        this.MovementSpeed += amount;
    }
}