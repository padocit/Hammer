using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "GameEvents", menuName = "Events/GameEvents")]
public class GameEvents : ScriptableObject
{
    [Header("Enemy Events")]
    public UnityEvent<Enemy> OnEnemySpawned;
    public UnityEvent<Enemy> OnEnemyDied;
    
    [Header("Player Events")]
    public UnityEvent<Player> OnPlayerDamaged;
    public UnityEvent<Player> OnPlayerDied;
}