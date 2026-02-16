using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UpgradesController : MonoBehaviour
{
    /// <summary>
    /// List of all the upgrades in the game, required so that we can show a random selection of these upgrades in the upgrade selector UI when the player enters the shop, and so that we can apply the effects of the taken upgrades to the player team when an upgrade is taken.
    /// </summary>
    [SerializeField]
    private List<UpgradeSO> allUpgrades;

    /// <summary>
    /// Maintains the upgrades that the player has taken, so that we can ensure that the player cannot take upgrades that they have already taken up to their limit, and so that we can apply the effects of the taken upgrades to the player team when an upgrade is taken.
    /// </summary>
    private List<UpgradeSO> takenUpgrades = new();

    /// <summary>
    /// Applies the given upgrade to the player team, and updates the treasury. Required so that when the player selects an upgrade in the upgrade selector UI, we can apply the effects of that upgrade to the player team, and update the treasury accordingly.
    /// </summary>
    /// <param name="upgrade">The upgrade to apply to the player team.</param>
    public void TakeUpgrade( UpgradeSO upgrade )
    {
        takenUpgrades.Add( upgrade );

        // Apply the upgrade to the player team.
        TeamController playerTeam = TeamsManager.Instance.FindPlayerTeam();

        playerTeam.Multipliers.IncreaseAttackDamageMultiplier( upgrade.attackDamageModifier );
        playerTeam.Multipliers.IncreaseAttackSpeedMultiplier( upgrade.attackSpeedModifier );
        playerTeam.Multipliers.IncreaseMovementSpeed( upgrade.movementSpeedModifier );


        // Update the treasury - may not remain.
        PlayerTreasury.Instance.UpgradePurchased();
    }

    /// <summary>
    /// Gets a random selection of the available upgrades, up to a default count of 3. Required so that we can show a random selection of upgrades in the upgrade selector UI when the player enters the shop.
    /// </summary>
    /// <returns>A list of randomly selected upgrades.</returns>
    public List<UpgradeSO> GetRandomUpgradesForUI()
    {
        return GetRandomUpgrades( 3 );
    }

    /// <summary>
    /// Gets a random selection of the available upgrades, up to the specified count. Required so that we can show a random selection of upgrades in the upgrade selector UI when the player enters the shop.
    /// </summary>
    /// <param name="count">The number of random upgrades to select.</param>
    /// <returns>A list of randomly selected upgrades.</returns>
    private List<UpgradeSO> GetRandomUpgrades( int count )
    {
        List<UpgradeSO> availableUpgrades = this.GetAvailableUpgrades();

        return availableUpgrades
            .OrderBy( s => Random.value )
            .Take( count )
            .ToList();
    }

    /// <summary>
    /// Gets the upgrades that are still available for the player to take (i.e. those that have not yet been taken up to their limit). Required so that we can ensure that the upgrade selector UI only shows upgrades that the player can actually take.
    /// </summary>
    /// <returns>A list of available upgrades that the player can take.</returns>
    private List<UpgradeSO> GetAvailableUpgrades()
    {
        List<UpgradeSO> availableUpgrades = this.allUpgrades
            .Where( s => takenUpgrades.Count( t => t == s ) < s.limit )
            .ToList();

        return availableUpgrades;
    }

    
}
