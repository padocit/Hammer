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
        // ĳ�õ� �÷��̾ �ִٸ� ���� ����
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
            
            // �̹� ��ϵ� �÷��̾ �ִٸ� ĳ��
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
        
        // ������ �Ϸ�� ���¶�� ��� Ÿ�� ����
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
            
            // �÷��̾ ���������Ƿ� Enemy ��Ȱ��ȭ �Ǵ� �ı�
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
        // �÷��̾ �Ҿ��� ���� ó��
        // �ɼ� 1: Enemy �ı�
        // Destroy(gameObject);
        
        // �ɼ� 2: Enemy ��Ȱ��ȭ
        // gameObject.SetActive(false);
        
        // �ɼ� 3: ��� ���·� ��ȯ
        _movement.SetTarget(null);
        _attack.SetTarget(null);
    }

    private void HandleDeath()
    {
        OnDeath?.Invoke();
        // ��� �� ó���� ���� ������Ʈ�� �и�
    }
}