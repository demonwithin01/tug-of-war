using System;
using UnityEngine;

/// <summary>
/// Helper utility for performing actions after a timer has expired.
/// </summary>
public class TimedAction
{
    private float timeEnd;
    private float currentTimer = 0f;

    private Action timeEndAction;
    
    public TimedAction( float timeEnd, Action timeEndAction )
    {
        this.timeEnd = timeEnd;
        this.timeEndAction = timeEndAction;
    }

    /// <summary>
    /// Advances the timer by the elapsed time and triggers the end action if the timer has completed.
    /// This method should only be called once per Update frame.
    /// </summary>
    public void Tick( bool canTrigger )
    {
        this.currentTimer += Time.deltaTime;

        if ( canTrigger && this.currentTimer >= this.timeEnd )
        {
            this.timeEndAction.Invoke();

            Reset();
        }
    }

    /// <summary>
    /// Resets the timer down to zero.
    /// </summary>
    public void Reset()
    {
        this.currentTimer = 0f;
    }

    /// <summary>
    /// Resets the timer so that it will trigger the action on the next update.
    /// </summary>
    public void ResetToTrigger()
    {
        this.currentTimer = this.timeEnd;
    }
}
