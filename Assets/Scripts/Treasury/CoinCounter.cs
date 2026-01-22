using TMPro;
using UnityEngine;

public class CoinCounter : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI textMesh;

    private void Start()
    {
        PlayerTreasury.Instance.OnTreasuryChange += this.GameController_OnCoinCollected;
    }

    private void GameController_OnCoinCollected( object sender, TreasuryChangedArgs e )
    {
        this.textMesh.text = "Coins: " + e.Total;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
