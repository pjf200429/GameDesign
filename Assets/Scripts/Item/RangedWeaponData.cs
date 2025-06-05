using UnityEngine;

/// <summary>
/// 远程武器数据（魔法 / 弹幕 / 子弹），继承自 EquipmentData
/// </summary>
[CreateAssetMenu(menuName = "Item/RangedWeaponData")]
public class RangedWeaponData : EquipmentData
{
    [Header("远程攻击属性")]
    public int Damage;                         // 子弹伤害值
    public float FireInterval;                 // 发射间隔（秒），等同于射速
    public float ProjectileSpeed;              // 子弹飞行速度
    public float MaxRange;                     // 子弹最大射程（可选，用于生命周期控制）

    [Header("子弹与特效")]
    [Tooltip("发射用的子弹预制体（需包含 Rigidbody2D 或 CharacterController 脚本等）")]
    public GameObject ProjectilePrefab;        // 子弹 Prefab
    [Tooltip("子弹命中时产生的特效 Prefab")]
    public GameObject HitEffectPrefab;         // 命中特效 Prefab
    [Tooltip("命中特效持续时间（秒），超过后自动销毁）")]
    public float HitEffectDuration = 2f;       // 特效持续时长

    [Header("远程动画")]
    public AnimationClip AttackAnimation;      // 远程施法 / 射击 动画剪辑

    [Header("远程音效")]
    public AudioClip AttackSound;              // 发射音效
}
