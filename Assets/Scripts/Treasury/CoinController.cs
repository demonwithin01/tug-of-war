using UnityEngine;

public class CoinController : MonoBehaviour
{
    private int value;

    public int Value => value;

    public void SetCoinValue( int value )
    {
        this.value = value;
    }

    public void Collected()
    {
        PlayerTreasury.Instance.CoinCollected( this.value );
        Destroy( this.gameObject );
    }
}
