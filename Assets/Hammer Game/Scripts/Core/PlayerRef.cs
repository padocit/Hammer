using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "PlayerRef", menuName = "Core/PlayerRef")]
public class PlayerRef : ScriptableObject
{
    [Header("Player Reference")]
    [SerializeField] private Player _player;
    
    [Header("Events")]
    public UnityEvent<Player> OnPlayerRegistered;
    public UnityEvent<Player> OnPlayerUnregistered;
    
    public Player Player 
    { 
        get => _player; 
        private set
        {
            var previousPlayer = _player;
            _player = value;
            
            if (previousPlayer != value)
            {
                if (previousPlayer != null)
                {
                    OnPlayerUnregistered?.Invoke(previousPlayer);
                }
                
                if (value != null)
                {
                    OnPlayerRegistered?.Invoke(value);
                }
            }
        }
    }
    
    public bool HasPlayer => _player != null;
    
    public void RegisterPlayer(Player player)
    {
        if (player == null)
        {
            Debug.LogWarning("Trying to register null player");
            return;
        }
        
        Player = player;
        Debug.Log($"Player registered: {player.name}");
    }
    
    public void UnregisterPlayer(Player player)
    {
        if (_player == player)
        {
            Player = null;
            Debug.Log($"Player unregistered: {player.name}");
        }
    }
    
    public void Clear()
    {
        Player = null;
    }
    
    // 에디터에서만 사용 (디버깅용)
    #if UNITY_EDITOR
    [ContextMenu("Clear Player Reference")]
    private void ClearPlayerReference()
    {
        Clear();
    }
    #endif
}