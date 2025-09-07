using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(EnemySpawner))]
[RequireComponent(typeof(EnemyMovement))]
[RequireComponent(typeof(EnemyHealth))]
public class Enemy : MonoBehaviour
{
    [Header("Components")]
    private EnemyMovement _movement;
    private EnemyHealth _health;
    private EnemySpawner _spawner;
    private EnemyAttack _attack;

    [Header("Player Reference")]
    [SerializeField] private PlayerRef _playerRef;
    private Player _cachedPlayer;

    [Header("Events")]
    public UnityEvent OnSpawnComplete;
    public UnityEvent OnDeath;

    private void Awake()
    {
        InitializeComponents();
        SubscribeToEvents();
        SubscribeToPlayerEvents();
    }

    private void Start()
    {
        // 캐시된 플레이어가 있다면 스폰 시작
        if (_cachedPlayer != null)
        {
            _spawner.StartSpawnSequence();
        }
        else
        {
            Debug.LogWarning("Player not found. Destroying enemy...");
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        UnsubscribeFromEvents();
        UnsubscribeFromPlayerEvents();
    }

    private void InitializeComponents()
    {
        _health = GetComponent<EnemyHealth>();
        _movement = GetComponent<EnemyMovement>();
        _spawner = GetComponent<EnemySpawner>();
        _attack = GetComponent<EnemyAttack>();
    }

    private void SubscribeToEvents()
    {
        _health.OnDeath += HandleDeath;
        _spawner.OnSpawnComplete += HandleSpawnComplete;
    }

    private void UnsubscribeFromEvents()
    {
        if (_health != null) _health.OnDeath -= HandleDeath; 
        if (_spawner != null) _spawner.OnSpawnComplete -= HandleSpawnComplete;
    }

    private void SubscribeToPlayerEvents()
    {
        if (_playerRef != null)
        {
            _playerRef.OnPlayerRegistered.AddListener(OnPlayerRegistered);
            _playerRef.OnPlayerUnregistered.AddListener(OnPlayerUnregistered);
            
            // 이미 등록된 플레이어가 있다면 캐시
            if (_playerRef.HasPlayer)
            {
                _cachedPlayer = _playerRef.Player;
            }
        }
        else
        {
            Debug.LogError("PlayerRef is not assigned to Enemy component");
        }
    }

    private void UnsubscribeFromPlayerEvents()
    {
        if (_playerRef != null)
        {
            _playerRef.OnPlayerRegistered.RemoveListener(OnPlayerRegistered);
            _playerRef.OnPlayerUnregistered.RemoveListener(OnPlayerUnregistered);
        }
    }

    private void OnPlayerRegistered(Player player)
    {
        _cachedPlayer = player;
        Debug.Log($"Enemy cached new player: {player.name}");
        
        // 스폰이 완료된 상태라면 즉시 타겟 설정
        if (_spawner != null && _spawner.IsSpawnComplete)
        {
            SetPlayerAsTarget();
        }
    }

    private void OnPlayerUnregistered(Player player)
    {
        if (_cachedPlayer == player)
        {
            _cachedPlayer = null;
            Debug.Log("Enemy lost player reference");
            
            // 플레이어가 없어졌으므로 Enemy 비활성화 또는 파괴
            HandlePlayerLost();
        }
    }

    private void HandleSpawnComplete()
    {
        SetPlayerAsTarget();
        OnSpawnComplete?.Invoke();
    }

    private void SetPlayerAsTarget()
    {
        if (_cachedPlayer != null)
        {
            _movement.SetTarget(_cachedPlayer.transform);
            _attack.SetTarget(_cachedPlayer.transform);
        }
    }

    private void HandlePlayerLost()
    {
        // 플레이어를 잃었을 때의 처리
        // 옵션 1: Enemy 파괴
        // Destroy(gameObject);
        
        // 옵션 2: Enemy 비활성화
        // gameObject.SetActive(false);
        
        // 옵션 3: 대기 상태로 전환
        _movement.SetTarget(null);
        _attack.SetTarget(null);
    }

    private void HandleDeath()
    {
        OnDeath?.Invoke();
        // 사망 시 처리는 별도 컴포넌트로 분리
    }
}