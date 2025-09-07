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
        
        // PlayerRef�� �Ҵ�Ǿ� �ִٸ� ���
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
        // �÷��̾ �ı��� �� ��� ����
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
    
    // ��Ÿ�ӿ��� PlayerRef ���� (���û���)
    public void SetPlayerRef(PlayerRef playerRef)
    {
        // ���� ���� ����
        if (_playerRef != null)
        {
            _playerRef.UnregisterPlayer(this);
        }
        
        // ���ο� ���� ����
        _playerRef = playerRef;
        
        if (_playerRef != null)
        {
            _playerRef.RegisterPlayer(this);
        }
    }
}
