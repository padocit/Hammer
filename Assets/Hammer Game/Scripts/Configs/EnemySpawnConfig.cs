using UnityEngine;

[CreateAssetMenu(fileName = "EnemySpawnConfig", menuName = "Configs/Enemy/Spawn")]
public class EnemySpawnConfig : ScriptableObject
{
    [Header("Spawn Timing")]
    public float IndicatorDuration = 0.3f;
    public int IndicatorLoops = 3;
    
    [Header("Visual")]
    public float IndicatorSizeMultiplier = 1.2f;
}