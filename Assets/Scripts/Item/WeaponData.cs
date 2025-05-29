// WeaponData.cs
// �������ݣ��������Ĳ�������Ч����Ч��������
using UnityEngine;

[CreateAssetMenu(menuName = "Item/WeaponData")]
public class WeaponData : ScriptableObject
{
    [Header("������Ϣ")]
    public string WeaponID;                    // Ψһ��ʶ
    public string DisplayName;                 // ��ʾ������
    public Sprite Icon;                        // UI ͼ��

    [Header("��������")]
    public int Damage;                         // �˺�ֵ
    public float AttackInterval;               // ����������룩
    public float AttackRange;                  // ������Χ���뾶��
    public Vector2 Offset;                     // �����ж�����ƫ��
    public float KnockbackForce;               // ��������

    [Header("����")]
    public AnimationClip AttackAnimation;      // ������������

    [Header("�����Ч")]
    public GameObject effectPrefab;            // ������Ч Prefab
    [Tooltip("��Ч���ʱ�䣨�룩���������Զ����٣�")]
    public float effectDuration = 2f;          // ��Ч����ʱ��

    [Header("�����Ч")]
    public AudioClip attackSound;              // ���ŵ���Ч
}
