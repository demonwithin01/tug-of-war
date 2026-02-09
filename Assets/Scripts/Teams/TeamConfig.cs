using UnityEngine;

public class TeamConfig : MonoBehaviour
{
    [SerializeField]
    private int teamNumber;

    [SerializeField]
    private Transform teamBaseLocation;

    public int TeamNumber => this.teamNumber;

    public Transform TeamBaseLocation => this.teamBaseLocation;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

}
