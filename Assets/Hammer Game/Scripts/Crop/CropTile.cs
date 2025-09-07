using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CropTile : MonoBehaviour
{
    private TileFieldState state;

    [Header(" Elements ")]
    [SerializeField] private Transform _cropParent;
    [SerializeField] private MeshRenderer _tileRenderer;
    private Crop _crop;
    private CropData _cropData;

    [Header(" Events ")]
    public static Action<CropType> OnCropHarvested;

    // Start is called before the first frame update
    void Start()
    {
        state = TileFieldState.Empty;
    }

    public void Sow(CropData cropData)
    {
        state = TileFieldState.Sown;
        _crop = Instantiate(cropData.CropPrefab, transform.position, Quaternion.identity, _cropParent);
        _cropData = cropData;
    }

    public void Water()
    {
        state = TileFieldState.Watered;
        _crop.ScaleUp();
        _tileRenderer.gameObject.LeanColor(Color.white * .3f, 1);
    }

    public void Harvest()
    {
        state = TileFieldState.Empty;
        _crop.ScaleDown();
        _tileRenderer.gameObject.LeanColor(Color.white, 1);

        OnCropHarvested?.Invoke(_cropData.CropType);
    }

    public bool IsEmpty()
    {
        return state == TileFieldState.Empty;
    }

    public bool IsSown()
    {
        return state == TileFieldState.Sown;
    }

    public bool IsWatered()
    {
        return state == TileFieldState.Watered;
    }
}
