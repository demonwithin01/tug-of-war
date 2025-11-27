using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }

    [SerializeField]
    private Transform playerBase;

    [SerializeField]
    private Transform aiBase;

    private void Awake()
    {
        Instance = this;
    }
}
