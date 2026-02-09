using UnityEngine;

public class RandomUpgradeSelectorUI : MonoBehaviour
{
    private void Start()
    {
        Hide();

        PlayerTreasury.Instance.OnPurchaseAmountReached += this.PlayerTreasury_OnPurchaseAmountReached;
    }

    private void PlayerTreasury_OnPurchaseAmountReached( object sender, PurchaseAmountReachedArgs e )
    {
        Show();
    }

    private void Show()
    {
        gameObject.SetActive( true );
        GameInput.Instance.DisableGameInput();
    }

    private void Hide()
    {
        gameObject.SetActive( false );
        GameInput.Instance.EnableGameInput();
    }

    public void PanelSelected( UpgradePanelUI panel, UpgradeSO upgradeDetails )
    {
        TeamController playerTeam = TeamsManager.Instance.FindPlayerTeam();

        playerTeam.Multipliers.IncreaseAttackDamageMultiplier( upgradeDetails.attackDamageModifier );
        playerTeam.Multipliers.IncreaseAttackSpeedMultiplier( upgradeDetails.attackSpeedModifier );
        playerTeam.Multipliers.IncreaseMovementSpeed( upgradeDetails.movementSpeedModifier );

        PlayerTreasury.Instance.UpgradePurchased();
        Hide();
    }
}
