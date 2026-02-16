using System;
using System.Collections.Generic;
using UnityEngine;

public class RandomUpgradeSelectorUI : MonoBehaviour
{
    /// <summary>
    /// Reference to the shop entrance controller, required so that we can register to the event for when the player enters the shop trigger, which is when we want to show the upgrade selector UI.
    /// </summary>
    [SerializeField]
    private ShopEntranceController shopEntranceController;

    /// <summary>
    /// Reference to the upgrades controller, required so that we can get random upgrades to show in the UI, and inform it of which upgrade was selected by the player.
    /// </summary>
    [SerializeField]
    private UpgradesController upgradesController;

    /// <summary>
    /// Prefab for the upgrade panel UI. Required so that we can spawn panels for each upgrade option when the UI is shown.
    /// </summary>
    [SerializeField]
    private GameObject upgradePanelPrefab;

    /// <summary>
    /// Required so that we know where to put the upgrade panels in the UI hierarchy when we spawn them.
    /// </summary>
    [SerializeField]
    private GameObject panelsContainer;

    /// <summary>
    /// Maintains the currently displayed upgrade panels.
    /// </summary>
    private List<UpgradePanelUI> upgradePanels = new();

    /// <summary>
    /// Unity start method.
    /// </summary>
    private void Start()
    {
        // Ensure the UI is hidden at the start of the game.
        Hide();

        // Register the event handler for when the player enters the shop trigger.
        this.shopEntranceController.OnPlayerEnteredShop += ShopEntranceController_OnPlayerEnteredShop;
    }

    /// <summary>
    /// Handler for when the player enters the shop trigger. Shows the upgrade selector UI.
    /// </summary>
    private void ShopEntranceController_OnPlayerEnteredShop(object sender, EventArgs e)
    {
        Show();
    }

    /// <summary>
    /// Shows the upgrade selector UI, and spawns the upgrade panels.
    /// </summary>
    private void Show()
    {
        // Disable game input while the UI is active.
        GameInput.Instance.DisableGameInput();

        // Get random upgrades to show in the UI.
        List<UpgradeSO> randomUpgrades = this.upgradesController.GetRandomUpgradesForUI();

        // Spawn upgrade panels.
        foreach (var upgrade in randomUpgrades)
        {
            // Instantiate and initialize UpgradePanelUI here.
            GameObject panel = Instantiate(upgradePanelPrefab, panelsContainer.transform);
            UpgradePanelUI panelUI = panel.GetComponent<UpgradePanelUI>();
            panelUI.Initialise(upgrade, this);
            upgradePanels.Add(panelUI);
        }

        // Activate the UI.
        gameObject.SetActive( true );
    }

    /// <summary>
    /// Hides the upgrade selector UI, and destroys the upgrade panels.
    /// </summary>
    public void Hide()
    {
        // Deactivate the UI.
        gameObject.SetActive( false );
        
        // Re-enable game input.
        GameInput.Instance.EnableGameInput();

        // Destroy panels.
        foreach (UpgradePanelUI child in upgradePanels)
        {
            Destroy(child.gameObject);
        }
        upgradePanels.Clear();
    }

    /// <summary>
    /// Called when an upgrade panel is selected. Applies the upgrade and hides the UI.
    /// </summary>
    /// <param name="panel">The selected upgrade panel.</param>
    /// <param name="upgradeDetails">The details of the selected upgrade.</param>
    public void PanelSelected( UpgradePanelUI panel, UpgradeSO upgradeDetails )
    {
        // Inform the upgrades controller of the selected upgrade so that it can be applied to the player team.
        this.upgradesController.TakeUpgrade( upgradeDetails );

        // Hide the upgrade selector UI.
        Hide();
    }
}
