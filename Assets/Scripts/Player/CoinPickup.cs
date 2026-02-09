using UnityEngine;

public class CoinPickupo : MonoBehaviour
{
    
    private void OnTriggerEnter(Collider other)
    {
        if ( other.CompareTag( "Coin" ) )
        {
            CoinController coin = other.transform.GetComponent<CoinController>();

            if ( coin != null )
            {
                coin.Collected();
            }
            else
            {
                Debug.LogError( "PlayerKing collided with an object tagged as Coin, but it doesn't have a CoinController component." );
            }
        }
    }

}
