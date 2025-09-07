using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using System.Linq;

public class WorldManager : MonoBehaviour
{
    [Header(" Elements ")]
    [SerializeField] private Transform _world;
    private Chunk[,] _grid;
    private List<IUnlockable> _allUnlockables;

    [Header(" Settings ")]
    [SerializeField] private int _gridSize;
    [SerializeField] private int _gridScale;

    [Header(" Data ")]
    private WorldData _worldData;
    private string _dataPath;
    private bool _shouldSave;

    private void Awake()
    {
        Chunk.OnChunkUnlocked += ChunkUnlockedCallback;
        Chunk.OnChunkPriceChanged += ChunkPriceChangedCallback;
    }

    // Start is called before the first frame update
    void Start()
    {
        _dataPath = Application.dataPath + "/WorldData.txt";
        LoadWorld();
        Initialize();

        InvokeRepeating("TrySaveGame", 1, 1);
    }

    private void OnDestroy()
    {
        Chunk.OnChunkUnlocked -= ChunkUnlockedCallback;
        Chunk.OnChunkPriceChanged -= ChunkPriceChangedCallback;

        // 모든 unlockable 객체들의 이벤트 구독 해제
        if (_allUnlockables != null)
        {
            foreach (var unlockable in _allUnlockables)
            {
                if (unlockable != null)
                {
                    unlockable.OnPriceChanged -= UnlockablePriceChangedCallback;
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Initialize()
    {
        // 모든 unlockable 객체들 수집
        CollectAllUnlockables();

        // Chunk들만 따로 초기화 (기존 로직 유지)
        InitializeChunks();

        // 나머지 unlockable 객체들 초기화
        InitializeOtherUnlockables();

        InitializeGrid();
        UpdateChunkWalls();
        UpdateGridRenderers();
    }

    private void CollectAllUnlockables()
    {
        _allUnlockables = new List<IUnlockable>();

        for (int i = 0; i < _world.childCount; i++)
        {
            IUnlockable unlockable = _world.GetChild(i).GetComponent<IUnlockable>();
            if (unlockable != null)
            {
                _allUnlockables.Add(unlockable);
                
                // Chunk가 아닌 경우에만 이벤트 구독 (Chunk는 static 이벤트 사용)
                if (!(unlockable is Chunk))
                {
                    unlockable.OnPriceChanged += UnlockablePriceChangedCallback;
                }
            }
        }
    }

    private void InitializeChunks()
    {
        var chunks = _allUnlockables.OfType<Chunk>().ToList();
        
        for (int i = 0; i < chunks.Count; i++)
        {
            if (i < _worldData.ChunkPrices.Count)
            {
                chunks[i].Initialize(_worldData.ChunkPrices[i]);
            }
            else
            {
                chunks[i].Initialize(chunks[i].InitialPrice);
            }
        }
    }

    private void InitializeOtherUnlockables()
    {
        var nonChunkUnlockables = _allUnlockables.Where(u => !(u is Chunk)).ToList();

        for (int i = 0; i < nonChunkUnlockables.Count; i++)
        {
            var unlockable = nonChunkUnlockables[i];
            string objectName = ((MonoBehaviour)unlockable).gameObject.name;

            // WorldData에서 해당 객체의 가격 찾기
            var savedData = _worldData.UnlockablePrices.FirstOrDefault(data => data.objectName == objectName);
            
            if (savedData != null)
            {
                unlockable.Initialize(savedData.price);
            }
            else
            {
                unlockable.Initialize(unlockable.InitialPrice);
            }
        }
    }

    private void InitializeGrid()
    {
        _grid = new Chunk[_gridSize, _gridSize];

        // Chunk들만 그리드에 배치
        var chunks = _allUnlockables.OfType<Chunk>();
        
        foreach (var chunk in chunks)
        {
            Vector2Int chunkGridPosition =
                new Vector2Int((int)chunk.transform.position.x / _gridScale, (int)chunk.transform.position.z / _gridScale);
        
            chunkGridPosition += new Vector2Int(_gridSize / 2, _gridSize / 2);

            if (IsValidGridPosition(chunkGridPosition.x, chunkGridPosition.y))
            {
                _grid[chunkGridPosition.x, chunkGridPosition.y] = chunk;
            }
        }
    }

    private void UpdateChunkWalls()
    {
        // Loop along the x-axis and y-axis
        for (int x = 0; x < _gridSize; x++)
        {
            for (int y = 0; y < _gridSize; y++)
            {
                Chunk chunk = _grid[x, y];

                if (chunk == null)
                    continue;

                Chunk frontChunk = IsValidGridPosition(x, y + 1) ? _grid[x, y + 1] : null;
                Chunk rightChunk = IsValidGridPosition(x + 1, y) ? _grid[x + 1, y] : null;
                Chunk backChunk  = IsValidGridPosition(x, y - 1) ? _grid[x, y - 1] : null;
                Chunk leftChunk  = IsValidGridPosition(x - 1, y) ? _grid[x - 1, y] : null;

                // Bitmasking to determine the configuration of walls
                int configuration = 0;

                if (frontChunk && frontChunk.IsUnlocked)
                    configuration |= 1;

                if (rightChunk && rightChunk.IsUnlocked)
                    configuration |= (1 << 1);

                if (backChunk && backChunk.IsUnlocked)
                    configuration |= (1 << 2);

                if (leftChunk && leftChunk.IsUnlocked)
                    configuration |= (1 << 3);

                // Use the configuration to set walls
                if (chunk.isActiveAndEnabled)
                    chunk.UpdateWalls(configuration);
            }
        }
    }

    private void UpdateGridRenderers()
    {
        // Loop along the x-axis and y-axis
        for (int x = 0; x < _gridSize; x++)
        {
            for (int y = 0; y < _gridSize; y++)
            {
                Chunk chunk = _grid[x, y];

                if (chunk == null)
                    continue;

                if (chunk.IsUnlocked)
                    continue;

                Chunk frontChunk = IsValidGridPosition(x, y + 1) ? _grid[x, y + 1] : null;
                Chunk rightChunk = IsValidGridPosition(x + 1, y) ? _grid[x + 1, y] : null;
                Chunk backChunk = IsValidGridPosition(x, y - 1) ? _grid[x, y - 1] : null;
                Chunk leftChunk = IsValidGridPosition(x - 1, y) ? _grid[x - 1, y] : null;

                if ((frontChunk && frontChunk.IsUnlocked) ||
                    (rightChunk && rightChunk.IsUnlocked) ||
                    (backChunk && backChunk.IsUnlocked) ||
                    (leftChunk && leftChunk.IsUnlocked))
                {
                    chunk.DisplayLockedElements();
                }
            }
        }
    }

    private bool IsValidGridPosition(int x, int y)
    {
        return x >= 0 && x < _gridSize && y >= 0 && y < _gridSize;
    }

    private void TrySaveGame()
    {
        if (_shouldSave)
        {
            SaveWorld();
            _shouldSave = false;
        }
    }

    private void ChunkUnlockedCallback()
    {
        Debug.Log("Chunk Unlocked");
        UpdateChunkWalls();
        UpdateGridRenderers();
        SaveWorld();
    }

    private void ChunkPriceChangedCallback()
    {
        _shouldSave = true;
    }

    private void UnlockablePriceChangedCallback()
    {
        _shouldSave = true;
    }

    private void LoadWorld()
    {
        if (!File.Exists(_dataPath))
        {
            CreateNewWorldData();
        }
        else
        {
            string data = File.ReadAllText(_dataPath);
            _worldData = JsonUtility.FromJson<WorldData>(data);

            if (_worldData == null)
            {
                CreateNewWorldData();
                return;
            }

            // 누락된 데이터 업데이트
            UpdateMissingData();
        }
    }

    private void CreateNewWorldData()
    {
        FileStream fs = new FileStream(_dataPath, FileMode.Create);
        _worldData = new WorldData();

        // 모든 unlockable 객체들의 초기 가격 저장
        for (int i = 0; i < _world.childCount; i++)
        {
            IUnlockable unlockable = _world.GetChild(i).GetComponent<IUnlockable>();
            if (unlockable != null)
            {
                if (unlockable is Chunk)
                {
                    _worldData.ChunkPrices.Add(unlockable.InitialPrice);
                }
                else
                {
                    string objectName = ((MonoBehaviour)unlockable).gameObject.name;
                    _worldData.UnlockablePrices.Add(new UnlockableData(objectName, unlockable.InitialPrice));
                }
            }
        }

        string worldDataString = JsonUtility.ToJson(_worldData, true);
        byte[] worldDataBytes = System.Text.Encoding.UTF8.GetBytes(worldDataString);

        fs.Write(worldDataBytes);
        fs.Close();
    }

    private void UpdateMissingData()
    {
        // 기존 Chunk 데이터 업데이트 로직 유지
        var chunks = _world.GetComponentsInChildren<Chunk>();
        int missingChunks = chunks.Length - _worldData.ChunkPrices.Count;

        for (int i = 0; i < missingChunks; i++)
        {
            int chunkIndex = chunks.Length - missingChunks + i;
            _worldData.ChunkPrices.Add(chunks[chunkIndex].InitialPrice);
        }

        // 새로운 unlockable 객체들 확인 및 추가
        var nonChunkUnlockables = _world.GetComponentsInChildren<MonoBehaviour>()
            .Where(mb => mb is IUnlockable && !(mb is Chunk))
            .Cast<IUnlockable>();

        foreach (var unlockable in nonChunkUnlockables)
        {
            string objectName = ((MonoBehaviour)unlockable).gameObject.name;
            
            if (!_worldData.UnlockablePrices.Any(data => data.objectName == objectName))
            {
                _worldData.UnlockablePrices.Add(new UnlockableData(objectName, unlockable.InitialPrice));
            }
        }
    }

    private void SaveWorld()
    {
        _worldData = new WorldData();

        // Chunk 가격들 저장
        var chunks = _allUnlockables.OfType<Chunk>().ToList();
        foreach (var chunk in chunks)
        {
            _worldData.ChunkPrices.Add(chunk.CurrentPrice);
        }

        // 다른 unlockable 객체들 가격 저장
        var nonChunkUnlockables = _allUnlockables.Where(u => !(u is Chunk));
        foreach (var unlockable in nonChunkUnlockables)
        {
            string objectName = ((MonoBehaviour)unlockable).gameObject.name;
            _worldData.UnlockablePrices.Add(new UnlockableData(objectName, unlockable.CurrentPrice));
        }

        string data = JsonUtility.ToJson(_worldData, true);
        File.WriteAllText(_dataPath, data);

        Debug.LogWarning("World saved");
    }

    [NaughtyAttributes.Button]
    private void ResetWorld()
    {
        foreach (var unlockable in _allUnlockables)
        {
            unlockable.Relock();
        }

        _shouldSave = true;
    }

    [NaughtyAttributes.Button]
    private void DebugUnlockables()
    {
        Debug.Log($"Total Unlockables: {_allUnlockables?.Count ?? 0}");
        
        if (_allUnlockables != null)
        {
            foreach (var unlockable in _allUnlockables)
            {
                string typeName = unlockable.GetType().Name;
                string objectName = ((MonoBehaviour)unlockable).gameObject.name;
                Debug.Log($"{typeName}: {objectName} - Price: {unlockable.CurrentPrice}, Unlocked: {unlockable.IsUnlocked}");
            }
        }
    }
}
