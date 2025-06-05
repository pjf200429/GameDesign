using UnityEngine;


public interface IWeaponStrategy
{
    void Attack(Transform attackOrigin, Vector3 targetPos);
}
