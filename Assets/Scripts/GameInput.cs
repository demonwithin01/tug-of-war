using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour
{
    [SerializeField]
    private PlayerKing playerKing;

    public static GameInput Instance { get; private set; }

    private PlayerInputActions playerInputActions;
    
    private void Awake()
    {
        Instance = this; 

        this.playerInputActions = new PlayerInputActions();

        this.playerInputActions.Player.Enable();

        this.playerInputActions.Player.ImproveUnitSpeed.performed += this.ImproveUnitSpeed_performed;
        this.playerInputActions.Player.Pickup.performed += this.Pickup_performed;
    }

    private void ImproveUnitSpeed_performed( UnityEngine.InputSystem.InputAction.CallbackContext obj )
    {
        TraitsManager.Instance.TEMP_IncreaseMoveSpeed();
    }

    private void Pickup_performed( UnityEngine.InputSystem.InputAction.CallbackContext obj )
    {
        
        Camera mainCamera = Camera.main;
        Ray ray = mainCamera.ScreenPointToRay( Mouse.current.position.ReadValue() );

        if ( Physics.Raycast( ray, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask( "Coin" ) ) ) // Refactor 7 to CONST...
        {
            CoinController coin = hit.transform.GetComponent<CoinController>();

            if ( coin != null )
            {
                PlayerTreasury.Instance.CoinCollected( coin.Value );
                Destroy( hit.transform.gameObject );
            }
        }
        else if ( Physics.Raycast( ray, out RaycastHit groundHit, Mathf.Infinity, LayerMask.GetMask( "Ground" ) ))
        {
            playerKing.SetDestination( groundHit.point );
        }
    }

}
