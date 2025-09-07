using UnityEngine;

public interface IDamageable
{
    void TakeDamage(int damage);
    void TakeDamage(int damage, Vector3 knockback);
}

public interface IMovable
{
    void SetTarget(Transform target);
    void Move(Vector3 direction);
    void Stop();
}

public interface IAttacker
{
    void SetTarget(Transform target);
    void Attack();
}