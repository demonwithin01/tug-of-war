using System;
using UnityEngine;

public class UnitAttackAnimationTrigger : MonoBehaviour
{
    public event EventHandler AttackTriggered;

    public void AttackCompleted()
    {
        AttackTriggered?.Invoke(this, EventArgs.Empty);
    }
}
