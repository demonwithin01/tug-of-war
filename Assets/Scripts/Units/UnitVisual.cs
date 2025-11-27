using UnityEngine;

public class UnitVisual : MonoBehaviour
{
    [SerializeField]
    private SkinnedMeshRenderer skinnedMeshRenderer;

    public void ApplyUnitColour( Material material )
    {
        this.skinnedMeshRenderer.material = material;
    }
}
