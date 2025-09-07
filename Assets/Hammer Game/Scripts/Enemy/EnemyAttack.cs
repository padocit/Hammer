using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private EnemyAttackConfig _config;
    
    private Transform _target;
    private float _attackTimer;

    private void Update()
    {
        if (_target == null) return;

        UpdateAttackTimer();
        
        if (CanAttack())
        {
            TryAttack();
        }
    }

    public void SetTarget(Transform target)
    {
        _target = target;
    }

    private void UpdateAttackTimer()
    {
        _attackTimer += Time.deltaTime;
    }

    private bool CanAttack()
    {
        return _attackTimer >= _config.AttackCooldown;
    }

    private void TryAttack()
    {
        var distanceToTarget = Vector3.Distance(transform.position, _target.position);
        
        if (distanceToTarget <= _config.AttackRange)
        {
            PerformAttack();
        }
    }

    private void PerformAttack()
    {
        _attackTimer = 0f;
        
        if (_target.TryGetComponent<IDamageable>(out var damageable))
        {
            damageable.TakeDamage(_config.Damage);
        }
    }
}