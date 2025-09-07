using UnityEngine;

[CreateAssetMenu(fileName = "EnemyAttackConfig", menuName = "Configs/Enemy/Attack")]
public class EnemyAttackConfig : ScriptableObject
{
    [Header("Attack Properties")]
    public int Damage = 10;
    public float AttackRange = 2f;
    public float AttackFrequency = 1f;
    
    public float AttackCooldown => 1f / AttackFrequency;
}