using Unity.VisualScripting;
using UnityEngine;

public class PlayerDestinationAnimationComplete : MonoBehaviour
{
    [SerializeField]
    private PlayerDestinationMarker destinationMarker;

    /// <summary>
    /// Handle when the arrow animation completes.
    /// </summary>
    public void ArrowAnimationEnd()
    {
        this.destinationMarker.gameObject.SetActive( false );
    }
}
