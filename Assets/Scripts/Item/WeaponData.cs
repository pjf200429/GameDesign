// WeaponData.cs
// 配置数据：各武器的参数、特效、音效、动画等
using UnityEngine;

[CreateAssetMenu(menuName = "Item/WeaponData")]
public class WeaponData : ScriptableObject
{
    [Header("基础信息")]
    public string WeaponID;                    // 唯一标识
    public string DisplayName;                 // 显示用名称
    public Sprite Icon;                        // UI 图标

    [Header("攻击属性")]
    public int Damage;                         // 伤害值
    public float AttackInterval;               // 攻击间隔（秒）
    public float AttackRange;                  // 攻击范围（半径）
    public Vector2 Offset;                     // 攻击判定中心偏移
    public float KnockbackForce;               // 击退力度

    [Header("动画")]
    public AnimationClip AttackAnimation;      // 攻击动作剪辑

    [Header("打击特效")]
    public GameObject effectPrefab;            // 命中特效 Prefab
    [Tooltip("特效存活时间（秒），超过后自动销毁）")]
    public float effectDuration = 2f;          // 特效持续时长

    [Header("打击音效")]
    public AudioClip attackSound;              // 播放的音效
}
