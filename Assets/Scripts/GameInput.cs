using UnityEngine;

public class GameInput : MonoBehaviour
{
    public static GameInput Instance { get; private set; }

    private PlayerInputActions playerInputActions;
    
    private void Awake()
    {
        Instance = this; 

        playerInputActions = new PlayerInputActions();

        playerInputActions.Player.Enable();

        playerInputActions.Player.ImproveUnitSpeed.performed += this.ImproveUnitSpeed_performed;
    }

    private void ImproveUnitSpeed_performed( UnityEngine.InputSystem.InputAction.CallbackContext obj )
    {
        TraitsManager.Instance.TEMP_IncreaseMoveSpeed();
    }
}
