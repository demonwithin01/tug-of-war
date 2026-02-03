using UnityEngine;

public class PlayerDestinationMarker : MonoBehaviour
{
    private void Awake()
    {
        this.gameObject.SetActive( false );
    }

    public void SetDestinationMarker( Vector3 position )
    {
        this.gameObject.SetActive( true );
        this.transform.position = position;
    }
}
