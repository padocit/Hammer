using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(PlayerAnimator))]
public class PlayerAttackAbility : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] private Transform _playerRenderer;
    [SerializeField] private Collider _hammerCollider;
    [SerializeField] private float _attackRange = 3f;
    [SerializeField] private LayerMask _enemyMask;
    [SerializeField] private int _damage = 10;
    [SerializeField] private float _knockbackForce = 5f;
    [SerializeField] private float _attackCooldown = 0.5f;
    
    private PlayerAnimator _playerAnimator;
    private bool _canAttack = true;
    private float _lastAttackTime;
    
    // 성능 최적화: 오브젝트 풀링과 캐싱
    private readonly List<Enemy> _nearbyEnemies = new List<Enemy>(16);
    private readonly Collider[] _enemyColliderBuffer = new Collider[32];
    
    // 거리 기반 캐싱 (가장 가까운 적 캐싱)
    private Enemy _cachedClosestEnemy;
    private float _lastEnemySearchTime;
    private const float ENEMY_SEARCH_INTERVAL = 0.1f; // 0.1초마다 검색
    
    private void Awake()
    {
        _playerAnimator = GetComponent<PlayerAnimator>();
    }
    
    private void Update()
    {
        if (_canAttack && CanPerformAttack())
        {
            TryAttack();
        }
    }
    
    private bool CanPerformAttack()
    {
        return Time.time >= _lastAttackTime + _attackCooldown;
    }
    
    private void TryAttack()
    {
        var closestEnemy = GetClosestEnemyOptimized();
        if (closestEnemy == null) return;
        
        _lastAttackTime = Time.time;
        
        // 적을 향해 회전
        LookAtTarget(closestEnemy.transform.position);
        
        // 공격 애니메이션 재생
        _playerAnimator.PlayAttackAnimation();
        
        // 공격 실행 (각 공격마다 독립적으로 실행)
        PerformAttack();
    } 
    
    // 성능 최적화된 적 탐지
    private Enemy GetClosestEnemyOptimized()
    {
        // 캐시된 결과 사용 (검색 빈도 제한)
        if (Time.time - _lastEnemySearchTime < ENEMY_SEARCH_INTERVAL && _cachedClosestEnemy != null)
        {
            float distanceToCache = Vector3.Distance(transform.position, _cachedClosestEnemy.transform.position);
            if (distanceToCache <= _attackRange)
            {
                return _cachedClosestEnemy;
            }
        }
        
        _lastEnemySearchTime = Time.time;
        
        // 버퍼를 사용한 OverlapSphere (GC 할당 최소화)
        int hitCount = Physics.OverlapSphereNonAlloc(
            transform.position, 
            _attackRange, 
            _enemyColliderBuffer, 
            _enemyMask
        );
        
        Enemy closestEnemy = null;
        float minDistanceSqr = _attackRange * _attackRange;
        
        for (int i = 0; i < hitCount; i++)
        {
            var enemy = _enemyColliderBuffer[i].GetComponent<Enemy>();
            if (enemy == null) continue;
            
            float distanceSqr = (enemy.transform.position - transform.position).sqrMagnitude;
            if (distanceSqr < minDistanceSqr)
            {
                minDistanceSqr = distanceSqr;
                closestEnemy = enemy;
            }
        }
        
        _cachedClosestEnemy = closestEnemy;
        return closestEnemy;
    }
    
    private void PerformAttack()
    {
        Vector3 hammerCenter = _hammerCollider.bounds.center;
        float hammerRadius = _hammerCollider.bounds.size.magnitude * 0.5f;
        
        int hitCount = Physics.OverlapSphereNonAlloc(
            hammerCenter, 
            hammerRadius, 
            _enemyColliderBuffer, 
            _enemyMask
        );
        
        // 각 공격마다 모든 범위 내 적에게 데미지
        for (int i = 0; i < hitCount; i++)
        {
            var enemy = _enemyColliderBuffer[i].GetComponent<Enemy>();
            if (enemy == null) continue;
            
            ApplyDamageToEnemy(enemy);
        }
        
        Debug.Log($"Attack performed! Hit {hitCount} enemies.");
    }
    
    private void ApplyDamageToEnemy(Enemy enemy)
    {
        Vector3 knockbackDirection = (enemy.transform.position - transform.position).normalized;
        knockbackDirection.y = 0;
        
        var enemyHealth = enemy.GetComponent<EnemyHealth>();
        enemyHealth?.TakeDamage(_damage, knockbackDirection * _knockbackForce);
    }
    
    private void LookAtTarget(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        direction.y = 0;
        
        if (direction != Vector3.zero)
        {
            _playerRenderer.rotation = Quaternion.LookRotation(direction);
        }
    }
    
    public void SetCanAttack(bool canAttack) => _canAttack = canAttack;
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _attackRange);
        
        if (_hammerCollider != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(_hammerCollider.bounds.center, _hammerCollider.bounds.size.magnitude * 0.5f);
        }
    }
}