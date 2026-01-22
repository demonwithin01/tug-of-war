using UnityEngine;

[CreateAssetMenu()]
public class UpgradeSO : ScriptableObject
{
    public string upgradeName;
    public float attackSpeedModifier = 0f;
    public float attackDamageModifier = 0f;
    public float movementSpeedModifier = 0f;
}
