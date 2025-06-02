
using System;
using UnityEngine;

/// <summary>
/// Represents a single Buff instance, responsible for tracking its own state and determining its lifecycle
/// </summary>
public class BuffInstance
{
    /// <summary>
    /// Buff type
    /// </summary>
    public BuffType Type { get; private set; }

    /// <summary>
    /// Buff duration
    /// </summary>
    public float Duration { get; private set; }

    /// <summary>
    /// Buff value
    /// </summary>
    public float Value { get; private set; }

    /// <summary>
    /// Elapsed time (in seconds)
    /// </summary>

    public float ElapsedTime { get; private set; }

    /// <summary>
    /// Indicates whether the one-time "apply" logic has already been executed (primarily used for Buffs with a one-time effect, such as increasing max health, which needs to be applied immediately)
    /// </summary>
    public bool IsApplied { get; private set; }

    /// <summary>
    /// Tracks the current stack count when stacking is supported
    /// If stacking is not required, this can be ignored or always set to 1
    /// </summary>
    public int StackCount { get; private set; }

    /// <summary>
    /// This event is invoked when the Buff ends normally, notifying external systems to remove its effects
    /// </summary>

    public event Action<BuffInstance> OnBuffExpired;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="type">Type of the Buff</param>
    /// <param name="duration">Duration in seconds</param>
    /// <param name="value">Buff value</param>
    /// <param name="stackCount">Number of stacks, default is 1</param>
    public BuffInstance(BuffType type, float duration, float value, int stackCount = 1)
    {
        Type = type;
        Duration = duration;
        Value = value;
        StackCount = stackCount;
        ElapsedTime = 0f;
        IsApplied = false;
    }
    /// <summary>
    /// Called every frame to advance Buff timing and trigger the corresponding logic
    /// </summary>
    /// <param name="deltaTime">Time.deltaTime</param>
    /// <param name="target">The GameObject that the Buff affects</param>

    public void UpdateBuff(float deltaTime, GameObject target)
    {
   
        if (!IsApplied)
        {
            BuffEffects.OnBuffApply(target, this);
            IsApplied = true;
        }

        BuffEffects.OnBuffTick(target, this, deltaTime);

     
        ElapsedTime += deltaTime;
        if (ElapsedTime >= Duration)
        {
     
            BuffEffects.OnBuffRemove(target, this);
    
            OnBuffExpired?.Invoke(this);
        }
    }

    /// <summary>
    /// If you need to refresh (rather than stack) an existing Buff of the same type, call this method to reset its duration and update its value
    /// </summary>
    /// <param name="newDuration">The new duration</param>
    /// <param name="newValue">The new value</param>
    public void Refresh(float newDuration, float newValue)
    {
        Duration = newDuration;
        Value = newValue;
        ElapsedTime = 0f;
        IsApplied = false;
    }

    /// <summary>
    /// Call this method to add stacks
    /// </summary>
    /// <param name="additionalValue">The value to add to the Buff</param>
    /// <param name="additionalDuration">Whether to extend or refresh the remaining duration</param>
    /// <param name="refreshDuration">The new duration if a refresh is needed</param>
    public void AddStack(float additionalValue, bool refreshDuration = false, float refreshDurationValue = 0f)
    {
        StackCount++;
        Value += additionalValue;
        if (refreshDuration)
        {
            Duration = refreshDurationValue;
            ElapsedTime = 0f;
            IsApplied = false;
        }
    }
}
