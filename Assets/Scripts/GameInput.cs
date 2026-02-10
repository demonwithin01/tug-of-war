using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour
{
    [SerializeField]
    private PlayerUnitController playerKing;

    public static GameInput Instance { get; private set; }

    private PlayerInputActions playerInputActions;
    
    private void Awake()
    {
        Instance = this; 

        this.playerInputActions = new PlayerInputActions();

        this.playerInputActions.Player.Enable();

        this.playerInputActions.Player.Pickup.performed += this.Pickup_performed;
    }

    private void Pickup_performed( InputAction.CallbackContext obj )
    {
        Camera mainCamera = Camera.main;
        Ray ray = mainCamera.ScreenPointToRay( Mouse.current.position.ReadValue() );

        if ( Physics.Raycast( ray, out RaycastHit groundHit, Mathf.Infinity, LayerMask.GetMask( "Ground" ) ))
        {
            playerKing.SetDestination( groundHit.point );
        }
    }

    public void DisableGameInput()
    {
        this.playerInputActions.Player.Disable();
    }

    public void EnableGameInput()
    {
        this.playerInputActions.Player.Enable();
    }
}
