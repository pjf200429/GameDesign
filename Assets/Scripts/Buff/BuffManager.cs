
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages all Buff instances attached to the current object, responsible for adding, updating, and removing them
/// </summary>

public class BuffManager : MonoBehaviour
{
    /// <summary>
    /// All Buff instances currently active
    /// </summary>
    private readonly List<BuffInstance> _activeBuffs = new List<BuffInstance>();

    private void Update()
    {
        float dt = Time.deltaTime;

        var expiredList = new List<BuffInstance>();

        foreach (var buff in _activeBuffs)
        {
            buff.UpdateBuff(dt, gameObject);
            if (buff.ElapsedTime >= buff.Duration)
                expiredList.Add(buff);
        }

        foreach (var deadBuff in expiredList)
        {
            deadBuff.OnBuffExpired -= HandleBuffExpired;
            _activeBuffs.Remove(deadBuff);
        }
    }

       /// <summary>
    /// Adds a new Buff to the current object
    /// </summary>
    /// <param name="type">Type of the Buff</param>
    /// <param name="duration">Duration in seconds</param>
    /// <param name="value">Buff value</param>
    /// <param name="stackCount">Initial stack count (default is 1)</param>
    /// <param name="canStack">Whether this Buff can stack with others of the same type (true = stack; false = refresh existing Buff)</param>
    public void AddBuff(BuffType type, float duration, float value, int stackCount = 1, bool canStack = false)
    {
        BuffInstance existing = _activeBuffs.Find(b => b.Type == type);
        if (existing != null)
        {
            if (canStack)
            {
                existing.AddStack(value, true, duration);
            }
            else
            {
                existing.Refresh(duration, value);
            }
            return;
        }

        var newBuff = new BuffInstance(type, duration, value, stackCount);
        newBuff.OnBuffExpired += HandleBuffExpired;
        _activeBuffs.Add(newBuff);
    }

    /// <summary>
    /// Called by BuffInstance to remove a Buff when it expires
    /// </summary>
    /// <param name="buff">The Buff instance to remove</param>

    private void HandleBuffExpired(BuffInstance buff)
    {
        buff.OnBuffExpired -= HandleBuffExpired;
        _activeBuffs.Remove(buff);
    }

    /// <summary>
    /// Allows external removal of a Buff (e.g., certain skills can dispel Debuffs)
    /// </summary>
    /// <param name="type">The type of Buff to remove</param>

    public void RemoveBuff(BuffType type)
    {
        BuffInstance existing = _activeBuffs.Find(b => b.Type == type);
        if (existing != null)
        {
            BuffEffects.OnBuffRemove(gameObject, existing);
            existing.OnBuffExpired -= HandleBuffExpired;
            _activeBuffs.Remove(existing);
        }
    }

    /// <summary>
    /// Checks if a specific Buff currently exists
    /// </summary>
    /// <param name="type">Type of the Buff</param>
    /// <returns>True if it exists, otherwise false</returns>
    public bool HasBuff(BuffType type)
    {
        return _activeBuffs.Exists(b => b.Type == type);
    }
}
