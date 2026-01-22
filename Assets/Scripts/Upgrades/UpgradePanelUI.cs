using TMPro;
using UnityEngine;

public class UpgradePanelUI : MonoBehaviour
{
    [SerializeField]
    private UpgradeSO upgradeDetails;

    [SerializeField]
    private TextMeshProUGUI descriptionTitle;

    [SerializeField]
    private RandomUpgradeSelectorUI upgradeSelectorUI;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdateVisual();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PanelSelected()
    {
        this.upgradeSelectorUI.PanelSelected( this, this.upgradeDetails );
    }

    private void UpdateVisual()
    {
        this.descriptionTitle.text = upgradeDetails.upgradeName;
    }
}
