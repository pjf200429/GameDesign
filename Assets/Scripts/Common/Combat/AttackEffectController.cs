using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AttackEffectController : MonoBehaviour
{
    [Header("��Ч�ҵ㣨ͨ������������ˣ�")]
    public Transform effectSpawnPoint;

    AudioSource _audioSrc;
    EquipmentItem _currentEquipment;

    void Awake()
    {
        _audioSrc = GetComponent<AudioSource>();
    }

    /// <summary>
    /// �л�����ʱ���ⲿ���� PlayerCombat������
    /// </summary>
    public void SetCurrentWeapon(ItemBase item)
    {
        _currentEquipment = item as EquipmentItem;
    }

    /// <summary>
    /// Animation Event �� PlayerCombat �ڹؼ�֡ʱ����
    /// </summary>
    public void PlayAttackEffect()
    {
        if (_currentEquipment == null) return;
        WeaponData data = _currentEquipment.WeaponData;
        if (data == null) return;

        // 1) ����������Ч������У�
        if (data.effectPrefab != null && effectSpawnPoint != null)
        {
            var fx = Instantiate(
                data.effectPrefab,
                effectSpawnPoint.position,
                effectSpawnPoint.rotation
            );
            if (fx.TryGetComponent<ParticleSystem>(out var ps))
                ps.Play();
            Destroy(fx, data.effectDuration);
        }

        // 2) ������Ч������У�
        if (data.attackSound != null)
            _audioSrc.PlayOneShot(data.attackSound);
    }
}
