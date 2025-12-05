/// <summary>
/// Manages health for a unit.
/// </summary>
public class UnitHealth2
{
    /// <summary>
    /// Gets the units health.
    /// </summary>
    public int CurrentHealth { get; private set; }

    /// <summary>
    /// Gets whether the unit is currently alive.
    /// </summary>
    /// <remarks>
    /// We track if the unit is still alive because the game object will still exist while the death animation is playing.
    /// </remarks>
    public bool IsAlive { get; private set; }

    public UnitHealth2( int maxHealth )
    {
        this.CurrentHealth = maxHealth;
        this.IsAlive = true;
    }

    /// <summary>
    /// Adjusts the unit's health.
    /// </summary>
    /// <param name="damage">The damage to apply to the unit.</param>
    /// <returns>True if the unit dies from this damage.</returns>
    public bool TakeDamage( int damage )
    {
        // If the unit is not alive, then do nothing.
        if ( this.IsAlive )
        {
            // Remove the damage from the health.
            this.CurrentHealth -= damage;

            // If the health is less than or equal to zero...
            if ( this.CurrentHealth <= 0 )
            {
                // Then mark them as not alive.
                this.IsAlive = false;

                // And return that they died.
                return true;
            }
        }

        // Otherwise always return false, even if they are not alive as this is to return whether they just died from the attack.
        return false;
    }
}
