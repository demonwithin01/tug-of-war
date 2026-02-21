
using System;
using System.Diagnostics;

public class ArcherUnitAnimationController : UnitAnimationController
{
    public event EventHandler AttackTriggered;

    protected override void Start()
    {
        base.Start();
    }

    public void AttackCompleted()
    {
        AttackTriggered?.Invoke(this, EventArgs.Empty);
    }
}
