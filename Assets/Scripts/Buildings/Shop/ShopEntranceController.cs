using System;
using UnityEngine;

public class ShopEntranceController : MonoBehaviour
{
    public event EventHandler OnPlayerEnteredShop;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    private void OnTriggerEnter( Collider other )
    {
        PlayerUnitController playerUnitController = other.GetComponent<PlayerUnitController>();

        if ( playerUnitController != null )
        {
            OnPlayerEnteredShop?.Invoke( this, EventArgs.Empty );
        }
    }
}
