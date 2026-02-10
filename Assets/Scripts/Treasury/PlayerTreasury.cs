using System;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class PlayerTreasury : MonoBehaviour
{
    public static PlayerTreasury Instance { get; private set; }

    public event EventHandler<TreasuryChangedArgs> OnTreasuryChange;
    public event EventHandler<PurchaseAmountReachedArgs> OnPurchaseAmountReached;

    [SerializeField]
    private int purchaseAmount = 10;

    [SerializeField]
    private Transform coinPrefab;

    private int coins = 0;

    private void Awake()
    {
        Instance = this;
    }

    public void CoinCollected( int value )
    {
        this.coins += value;

        this.OnTreasuryChange?.Invoke( this, new TreasuryChangedArgs( value, this.coins ) );

        if ( this.coins >= this.purchaseAmount )
        {
            this.OnPurchaseAmountReached?.Invoke( this, new PurchaseAmountReachedArgs( this.coins ) );
        }
    }

    public void UpgradePurchased()
    {
        this.coins -= this.purchaseAmount;

        this.OnTreasuryChange?.Invoke( this, new TreasuryChangedArgs( -this.purchaseAmount, this.coins ) );
    }

    public void SpawnCoin( Vector3 position, int coinWorth )
    {
        Transform coinTransform = Instantiate( this.coinPrefab, position, Quaternion.identity );
        coinTransform.GetComponent<CoinController>().SetCoinValue( coinWorth );
    }
}
