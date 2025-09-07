using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

[RequireComponent(typeof(ChunkWalls))]
public class Chunk : UnlockableBase
{
    private ChunkWalls _chunkWalls;

    [Header(" Actions ")]
    public static Action OnChunkUnlocked;
    public static Action OnChunkPriceChanged;

    private void Awake()
    {
        _chunkWalls = GetComponent<ChunkWalls>();
        
        // Base class events를 static events에 연결
        OnUnlocked += () => OnChunkUnlocked?.Invoke();
        OnPriceChanged += () => OnChunkPriceChanged?.Invoke();
    }

    protected override void OnUnlockComplete()
    {
        // Chunk specific unlock logic
        Debug.Log($"Chunk {gameObject.name} unlocked!");
    }

    protected override void OnLockComplete()
    {
        // Chunk specific lock logic
        Debug.Log($"Chunk {gameObject.name} locked!");
    }

    public void UpdateWalls(int configuration)
    {
        _chunkWalls.Configure(configuration);
    }

    // Legacy methods for backward compatibility
    public void RelockChunk()
    {
        Relock();
    }
}
