using System;
using UnityEngine;

public class UnitHealth : MonoBehaviour
{
    public class DiedEventArgs
    {
        public AttackController Unit { get; private set; }

        public DiedEventArgs( AttackController unit )
        {
            this.Unit = unit;
        }
    }

    public event EventHandler<DiedEventArgs> Died;

    [SerializeField]
    private int baseMaxHealth = 100;

    private int currentHealth = 0;

    private AttackController attackController;

    public bool IsAlive { get; private set; } = true;

    private void Awake()
    {
        this.currentHealth = this.baseMaxHealth; // Modifiers here.
        this.attackController = this.GetComponent<AttackController>();
    }

    private void OnDestroy()
    {
        this.Died?.Invoke( this, new DiedEventArgs( this.attackController ) );
    }

    public void TakeDamage( int damage )
    {
        if ( this.IsAlive )
        {
            this.currentHealth -= damage;

            if ( currentHealth < 0 )
            {
                this.IsAlive = false;
                Died?.Invoke( this, new DiedEventArgs( this.attackController ) );
                //Destroy( this, 1f );
            }
        }
    }
}
