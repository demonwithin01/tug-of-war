using TMPro;
using UnityEngine;

public class UpgradePanelUI : MonoBehaviour
{
    /// <summary>
    /// Reference to the text element in the upgrade panel UI that displays the title/description of the upgrade. Required so that we can update the text to show the correct details for the upgrade that this panel is displaying.
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI descriptionTitle;

    /// <summary>
    /// Maintains the details of the upgrade that this panel is displaying, so that it can inform the upgrade selector UI of which upgrade was selected when this panel is selected by the player.
    /// </summary>
    private UpgradeSO upgradeDetails;

    /// <summary>
    /// Reference to the upgrade selector UI that this panel is part of, so that it can inform the selector UI when it is selected by the player.
    /// </summary>
    private RandomUpgradeSelectorUI upgradeSelectorUI;

    /// <summary>
    /// Initialises the upgrade panel with the given details, and a reference to the upgrade selector UI that it is part of, so that it can inform the selector UI when it is selected by the player.
    /// </summary>
    /// <param name="upgradeDetails">The details of the upgrade to display in this panel.</param>
    /// <param name="upgradeSelectorUI">The upgrade selector UI that this panel is part of, so that it can inform the selector UI when it is selected by the player.</param>
    public void Initialise( UpgradeSO upgradeDetails, RandomUpgradeSelectorUI upgradeSelectorUI )
    {
        this.upgradeDetails = upgradeDetails;
        this.upgradeSelectorUI = upgradeSelectorUI;

        UpdateVisual();
    }

    /// <summary>
    /// Called when the upgrade panel is selected by the player. Informs the upgrade selector UI of the selection.
    /// </summary>
    public void PanelSelected()
    {
        this.upgradeSelectorUI.PanelSelected( this, this.upgradeDetails );
    }

    /// <summary>
    /// Updates the visual elements of the upgrade panel based on the current upgrade details.
    /// </summary>
    private void UpdateVisual()
    {
        this.descriptionTitle.text = upgradeDetails.upgradeName;
    }
}
