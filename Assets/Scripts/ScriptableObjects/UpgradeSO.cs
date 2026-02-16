using UnityEngine;

[CreateAssetMenu()]
public class UpgradeSO : ScriptableObject
{
    /// <summary>
    /// The name of the upgrade, to be displayed in the UI. Required so that we can show the correct name for each upgrade in the upgrade selector UI.
    /// </summary>
    public string upgradeName;
    /// <summary>
    /// The limit for how many times this upgrade can be taken by the player. Required so that we can ensure that upgrades that are only meant to be taken once, cannot be taken multiple times by the player.
    /// </summary>
    public int limit = 1;
    
    /// <summary>
    /// The modifier for the attack speed provided by this upgrade. Required so that we can apply the correct attack speed changes when this upgrade is taken by the player.
    /// </summary>
    public float attackSpeedModifier = 0f;
    /// <summary>
    /// The modifier for the attack damage provided by this upgrade. Required so that we can apply the correct attack damage changes when this upgrade is taken by the player.
    /// </summary>
    public float attackDamageModifier = 0f;
    /// <summary>
    /// The modifier for the movement speed provided by this upgrade. Required so that we can apply the correct movement speed changes when this upgrade is taken by the player.
    /// </summary>
    public float movementSpeedModifier = 0f;
    
}
