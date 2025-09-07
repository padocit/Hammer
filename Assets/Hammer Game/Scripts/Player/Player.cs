using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerHealth))]
public class Player : MonoBehaviour
{
    [Header(" Components ")]
    private PlayerHealth _playerHealth;
    
    [Header("Player Registry")]
    [SerializeField] private PlayerRef _playerRef;

    private void Awake()
    {
       _playerHealth = GetComponent<PlayerHealth>();
        
        // PlayerRef가 할당되어 있다면 등록
        if (_playerRef != null)
        {
            _playerRef.RegisterPlayer(this);
        }
        else
        {
            Debug.LogWarning("PlayerRef is not assigned to Player component");
        }
    }

    private void OnDestroy()
    {
        // 플레이어가 파괴될 때 등록 해제
        if (_playerRef != null)
        {
            _playerRef.UnregisterPlayer(this);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(int damage)
    {
        _playerHealth.TakeDamage(damage);
    }
    
    // 런타임에서 PlayerRef 설정 (선택사항)
    public void SetPlayerRef(PlayerRef playerRef)
    {
        // 이전 참조 해제
        if (_playerRef != null)
        {
            _playerRef.UnregisterPlayer(this);
        }
        
        // 새로운 참조 설정
        _playerRef = playerRef;
        
        if (_playerRef != null)
        {
            _playerRef.RegisterPlayer(this);
        }
    }
}
