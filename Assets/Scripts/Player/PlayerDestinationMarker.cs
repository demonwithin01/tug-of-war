using UnityEngine;

public class PlayerDestinationMarker : MonoBehaviour
{
    [SerializeField] 
    private Animator southArrow;
    [SerializeField] 
    private Animator northArrow;
    [SerializeField] 
    private Animator eastArrow;
    [SerializeField] 
    private Animator westArrow;

    private void Awake()
    {
        this.gameObject.SetActive( false );
    }

    public void SetDestinationMarker( Vector3 position )
    {
        this.gameObject.SetActive( true );
        this.transform.position = position;
        this.northArrow.Play( "ArrowNorth", -1, 0f );
        this.southArrow.Play( "ArrowSouth", -1, 0f );
        this.eastArrow.Play( "ArrowEast", -1, 0f );
        this.westArrow.Play( "ArrowWest", -1, 0f );
    }
}
