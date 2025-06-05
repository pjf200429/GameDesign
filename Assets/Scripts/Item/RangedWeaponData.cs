using UnityEngine;

/// <summary>
/// Զ���������ݣ�ħ�� / ��Ļ / �ӵ������̳��� EquipmentData
/// </summary>
[CreateAssetMenu(menuName = "Item/RangedWeaponData")]
public class RangedWeaponData : EquipmentData
{
    [Header("Զ�̹�������")]
    public int Damage;                         // �ӵ��˺�ֵ
    public float FireInterval;                 // ���������룩����ͬ������
    public float ProjectileSpeed;              // �ӵ������ٶ�
    public float MaxRange;                     // �ӵ������̣���ѡ�������������ڿ��ƣ�

    [Header("�ӵ�����Ч")]
    [Tooltip("�����õ��ӵ�Ԥ���壨����� Rigidbody2D �� CharacterController �ű��ȣ�")]
    public GameObject ProjectilePrefab;        // �ӵ� Prefab
    [Tooltip("�ӵ�����ʱ��������Ч Prefab")]
    public GameObject HitEffectPrefab;         // ������Ч Prefab
    [Tooltip("������Ч����ʱ�䣨�룩���������Զ����٣�")]
    public float HitEffectDuration = 2f;       // ��Ч����ʱ��

    [Header("Զ�̶���")]
    public AnimationClip AttackAnimation;      // Զ��ʩ�� / ��� ��������

    [Header("Զ����Ч")]
    public AudioClip AttackSound;              // ������Ч
}
