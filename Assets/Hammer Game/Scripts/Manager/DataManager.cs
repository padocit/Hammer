using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }

    [Header(" Data ")]
    [SerializeField] private CropData[] _cropDatas;
    [SerializeField] private ResourceData[] _resourceDatas;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public Sprite GetCropSpriteFromCropType(CropType cropType)
    {
        foreach (CropData cropData in _cropDatas)
            if (cropData.CropType == cropType)
                return cropData.Icon;

        Debug.LogError($"No Icon found for CropType: {cropType}");
        return null;
    }

    public int GetCropPriceFromCropType(CropType cropType)
    {
        foreach (CropData cropData in _cropDatas)
            if (cropData.CropType == cropType)
                return cropData.Price;

        Debug.LogError($"No Price found for CropType: {cropType}");
        return 0;
    }

    public Sprite GetResourceSpriteFromResourceType(ResourceType resourceType)
    {
        foreach (ResourceData resourceData in _resourceDatas)
            if (resourceData.ResourceType == resourceType)
                return resourceData.Icon;

        Debug.LogError($"No Icon found for ResourceType: {resourceType}");
        return null;
    }

    //public ResourceData GetResourceDataFromResourceType(ResourceType resourceType)
    //{
    //    foreach (ResourceData resourceData in _resourceDatas)
    //        if (resourceData.ResourceType == resourceType)
    //            return resourceData;

    //    Debug.LogError($"No ResourceData found for ResourceType: {resourceType}");
    //    return null;
    //}
}
