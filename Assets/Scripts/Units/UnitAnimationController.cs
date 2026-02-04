using UnityEngine;

public class UnitAnimationController : MonoBehaviour
{
    // States
    private bool isMoving = true;

    // Unity components
    private Animator animator;

    /// <summary>
    /// Animation state names.
    /// </summary>
    private static class AnimationState
    {
        public const string IsRunning = "IsRunning";
        public const string IsAttacking = "IsAttacking";
    }

    /// <summary>
    /// Animation names.
    /// </summary>
    private static class AnimationName
    {
        public const string Attack = "Attack";
        public const string Death = "Death";
    }
    
    private void Awake()
    {
        // Get the components that this controller will rely on.
        this.animator = GetComponent<Animator>();

    }

    private void Start()
    {
        // Set initial animation states.
        this.animator.SetBool( AnimationState.IsRunning, this.isMoving );
        this.animator.SetBool( AnimationState.IsAttacking, false );
    }

    public void StartRunning()
    {
        this.isMoving = true;

        this.animator.SetBool( AnimationState.IsRunning, true );

        this.StopAttacking();
    }

    public void StopRunning()
    {
        this.isMoving = false;

        this.animator.SetBool( AnimationState.IsRunning, false );
    }

    public void PerformAttack()
    {
        this.animator.SetBool( AnimationState.IsAttacking, true );
        this.animator.Play( AnimationName.Attack, layer: -1, normalizedTime: 0f );
    }

    public void StopAttacking()
    {
        this.animator.SetBool( AnimationState.IsAttacking, false );
    }

    public void PlayDeathAnimation()
    {
        this.animator.Play( AnimationName.Death, layer: -1, normalizedTime: 0f );
    }
}
