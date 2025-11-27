using UnityEngine;

public class TraitsManager : MonoBehaviour
{
    public static TraitsManager Instance { get; private set; }

    [SerializeField]
    private GameInput gameInput;

    public float SwordsmanMoveSpeedModifier { get; private set; } = 0f;

    private void Awake()
    {
        Instance = this;
    }

    public void TEMP_IncreaseMoveSpeed()
    {
        SwordsmanMoveSpeedModifier = Mathf.Min( SwordsmanMoveSpeedModifier + 0.1f, 3.5f );
    }
}
