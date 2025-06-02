using UnityEngine;

/// <summary>
/// Centralizes the specific logic for applying, maintaining, and removing various Buffs
/// </summary>
public static class BuffEffects
{
    /// <summary>
    /// Triggered when the Buff is first applied (one-time effect logic, such as increasing attack/defense)
    /// </summary>

    public static void OnBuffApply(GameObject target, BuffInstance buff)
    {
        switch (buff.Type)
        {
            //—— Damage-over-Time Buffs ——
            //case BuffType.Poison:
            //case BuffType.Burning:
            //    // Do nothing on Apply
            //    break;

            //—— Attack multiplier up —— 
            case BuffType.AttackUp:
                {
                  
                    var attr = target.GetComponent<PlayerAttributes>();
                    if (attr != null)
                    {
                        attr.AddAttackMultiplier(buff.Value * buff.StackCount);
                        Debug.Log($"[BuffEffects] 对象 {target.name} 获得 AttackUp Buff，Increase Multiplier = {buff.Value * buff.StackCount}");
                    }
                }
                break;

            //—— Defense up —— 
            case BuffType.DefenseUp:
                {
                    var attr = target.GetComponent<PlayerAttributes>();
                    if (attr != null)
                    {
                 
                        attr.AddDefense((int)(buff.Value * buff.StackCount));
                        Debug.Log($"[BuffEffects] 对象 {target.name} 获得 DefenseUp Buff，Increase Defense = {buff.Value * buff.StackCount}");
                    }
                }
                break;

            //—— HealthUp —— 
            //case BuffType.HealthUp:
            //    ...
            //    break;

            default:
               
                break;
        }
    }
    /// <summary>
    /// Called each frame while the Buff is active (used for continuous damage and similar logic)
    /// </summary>
    public static void OnBuffTick(GameObject target, BuffInstance buff, float deltaTime)
    {
        switch (buff.Type)
        {
            case BuffType.Poison:
                {
                    int damageThisFrame = Mathf.CeilToInt(buff.Value * buff.StackCount * deltaTime);
                    var playerHealth = target.GetComponent<PlayerHealthController>();
                    if (playerHealth != null)
                        playerHealth.TakeDamage(damageThisFrame);
                    else
                    {
                        var enemyHealth = target.GetComponent<EnemyHealthController>();
                        if (enemyHealth != null)
                            enemyHealth.TakeDamage(damageThisFrame);
                    }
                }
                break;

            case BuffType.Burning:
                {
                    int damageThisFrame = Mathf.CeilToInt(buff.Value * buff.StackCount * deltaTime);
                    var playerHealth = target.GetComponent<PlayerHealthController>();
                    if (playerHealth != null)
                        playerHealth.TakeDamage(damageThisFrame);
                    else
                    {
                        var enemyHealth = target.GetComponent<EnemyHealthController>();
                        if (enemyHealth != null)
                            enemyHealth.TakeDamage(damageThisFrame);
                    }
                }
                break;

           
            default:
                break;
        }
    }

    /// <summary>
    /// Callback when the Buff expires or the old instance needs to be removed due to refresh/stacking (reverting attribute modifications)
    /// </summary>
    public static void OnBuffRemove(GameObject target, BuffInstance buff)
    {
        switch (buff.Type)
        {
            case BuffType.AttackUp:
                {
                    var attr = target.GetComponent<PlayerAttributes>();
                    if (attr != null)
                    {
                        attr.AddAttackMultiplier(-buff.Value * buff.StackCount);
                        
                    }
                }
                break;

            case BuffType.DefenseUp:
                {
                    var attr = target.GetComponent<PlayerAttributes>();
                    if (attr != null)
                    {
                        attr.AddDefense(-(int)(buff.Value * buff.StackCount));
                       
                    }
                }
                break;

            default:
                break;
        }
    }
}
