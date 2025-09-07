using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CropField : MonoBehaviour
{
    [Header(" Elements ")]
    [SerializeField] private Transform _tilesParent;
    private List<CropTile> _cropTiles = new List<CropTile>();

    [Header(" Settings ")]
    [SerializeField] private CropData _cropData;
    [SerializeField] private GameObject[] _stateIcons; // 0 = Empty, 1 = Sown, 2 = Watered
    private TileFieldState _state;

    private int _tilesSown = 0;
    private int _tilesWatered = 0;
    private int _tilesHarvested = 0;

    [Header(" Actions ")]
    public static Action<CropField> OnFullySown;
    public static Action<CropField> OnFullyWatered;
    public static Action<CropField> OnFullyHarvested;

    // Start is called before the first frame update
    void Start()
    {
        _state = TileFieldState.Empty;
        StoreTiles();
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void StoreTiles()
    {
        for (int i = 0; i < _tilesParent.childCount; i++)
        {
            CropTile cropTile = _tilesParent.GetChild(i).GetComponent<CropTile>();
            if (cropTile != null)
                _cropTiles.Add(cropTile);
        }
    }

    public void SeedsCollidedCallback(Vector3[] seedPositions)
    {
        for (int i = 0; i < seedPositions.Length; i++)
        {
            CropTile closestCropTile = GetClosestCropTile(seedPositions[i]);

            if (closestCropTile == null)
                continue;

            if (!closestCropTile.IsEmpty())
                continue;

            Sow(closestCropTile);
        }
    }

    private void Sow(CropTile cropTile)
    {
        cropTile.Sow(_cropData);
        _tilesSown++;

        if (_tilesSown == _cropTiles.Count)
            FieldFullySown();
    }

    public void WaterCollidedCallback(Vector3[] waterPositions)
    {
        for (int i = 0; i < waterPositions.Length; i++)
        {
            CropTile closestCropTile = GetClosestCropTile(waterPositions[i]);

            if (closestCropTile == null)
                continue;

            if (!closestCropTile.IsSown())
                continue;

            Water(closestCropTile);
        }
    }
    private void Water(CropTile cropTile)
    {
        cropTile.Water();
        _tilesWatered++;

        if (_tilesWatered == _cropTiles.Count)
            FieldFullyWatered();
    }

    private void FieldFullySown()
    {
        //Debug.Log("Field Fully Sown");

        _state = TileFieldState.Sown;

        OnFullySown?.Invoke(this);
        UpdateIcon();
    }
    private void FieldFullyWatered()
    {
        //Debug.Log("Field Fully Watered");

        _state = TileFieldState.Watered;

        OnFullyWatered?.Invoke(this);
        UpdateIcon();
    }

    private void FieldFullyHarvested()
    {
        //Debug.Log("Field Fully Harvested");

        _state = TileFieldState.Empty;

        _tilesSown = 0;
        _tilesWatered = 0;
        _tilesHarvested = 0;

        OnFullyHarvested?.Invoke(this);
        UpdateIcon();
    }

    private void UpdateIcon()
    {
        // 모든 아이콘 비활성화
        for (int i = 0; i < _stateIcons.Length; i++)
        {
            _stateIcons[i].SetActive(false);
        }

        // 현재 상태에 맞는 모델 활성화
        int modelIndex = (int)_state;
        if (modelIndex < _stateIcons.Length && _stateIcons[modelIndex] != null)
        {
            _stateIcons[modelIndex].SetActive(true);
        }
    }
    public void Harvest(Transform harvestSphere)
    {
        float sphereRadius = harvestSphere.localScale.x;

        for (int i = 0; i < _cropTiles.Count; i++)
        {
            if (_cropTiles[i].IsEmpty()) // Already harvested
                continue;

            float distanceCropTileSphere = 
                Vector3.Distance(harvestSphere.position, _cropTiles[i].transform.position);
        
            if (distanceCropTileSphere <= sphereRadius)
                HarvestTile(_cropTiles[i]);

        }
    }

    private void HarvestTile(CropTile cropTile)
    {
        cropTile.Harvest();

        _tilesHarvested++;

        if (_tilesHarvested == _cropTiles.Count)
            FieldFullyHarvested();

    }

    [NaughtyAttributes.Button]
    private void InstantlySowTiles()
    {
        for (int i = 0; i < _cropTiles.Count; i++)
            Sow(_cropTiles[i]);
    }

    [NaughtyAttributes.Button]
    private void InstantlyWaterTiles()
    {
        for (int i = 0; i < _cropTiles.Count; i++)
            Water(_cropTiles[i]);
    }

    private CropTile GetClosestCropTile(Vector3 seedPosition)
    {
        float closestDistance = 5000; // Finding radius from seedPosition
        int closestCropTileIndex = -1;

        for (int i = 0; i < _cropTiles.Count; i++)
        {
            CropTile cropTile = _cropTiles[i];
            float distanceTileSeed = 
                Vector3.Distance(seedPosition, cropTile.transform.position);

            if (distanceTileSeed < closestDistance)
            {
                closestDistance = distanceTileSeed;
                closestCropTileIndex = i;
            }
        }

        if (closestCropTileIndex == -1)
            return null;

        return _cropTiles[closestCropTileIndex];
    }

    public bool IsEmpty()
    {
        return _state == TileFieldState.Empty;
    }
    public bool IsSown()
    {
        return _state == TileFieldState.Sown;
    }

    public bool IsWatered()
    {
        return _state == TileFieldState.Watered;
    }
}
