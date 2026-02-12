using System;
using UnityEngine;

public class RandomUpgradeSelectorUI : MonoBehaviour
{
    [SerializeField]
    private ShopEntranceController shopEntranceController;

    private void Start()
    {
        Hide();

        this.shopEntranceController.OnPlayerEnteredShop += ShopEntranceController_OnPlayerEnteredShop;
    }

    private void ShopEntranceController_OnPlayerEnteredShop(object sender, EventArgs e)
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
