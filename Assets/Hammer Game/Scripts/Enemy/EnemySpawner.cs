using UnityEngine;
using System;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private EnemySpawnConfig _config;
    
    [Header("Visual Elements")]
    [SerializeField] private GameObject _enemyRenderer;
    [SerializeField] private SpriteRenderer _spawnIndicator;

    public Action OnSpawnComplete;
    public bool IsSpawnComplete { get; private set; } = false;

    public void StartSpawnSequence()
    {
        IsSpawnComplete = false;
        SetRendererVisible(false);
        AnimateSpawnIndicator();
    }

    private void AnimateSpawnIndicator()
    {
        var targetScale = _spawnIndicator.transform.localScale * _config.IndicatorSizeMultiplier;
        
        LeanTween.scale(_spawnIndicator.gameObject, targetScale, _config.IndicatorDuration)
            .setLoopPingPong(_config.IndicatorLoops)
            .setOnComplete(CompleteSpawnSequence);
    }

    private void CompleteSpawnSequence()
    {
        IsSpawnComplete = true;
        SetRendererVisible(true);
        OnSpawnComplete?.Invoke();
    }

    private void SetRendererVisible(bool visible)
    {
        _enemyRenderer.SetActive(visible);
        _spawnIndicator.enabled = !visible;
    }
}