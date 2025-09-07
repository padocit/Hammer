using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryDisplay : MonoBehaviour
{
    [Header(" Elements ")]
    [SerializeField] private Transform _cropContainersParent;
    [SerializeField] private UICropContainer _uiCropContainerPrefab;
    [SerializeField] private Transform _resourceContainersParent;
    [SerializeField] private UICropContainer _uiResourceContainerPrefab;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Configure(Inventory inventory)
    {
        InventoryItem[] items = inventory.GetInventoryItems();

        // Separate crops and resources
        List<InventoryItem> crops = new List<InventoryItem>();
        List<InventoryItem> resources = new List<InventoryItem>();

        for (int i = 0; i < items.Length; i++)
        {
            if (items[i].ItemType == ItemType.Crop)
            {
                crops.Add(items[i]);
            }
            else if (items[i].ItemType == ItemType.Resource)
            {
                resources.Add(items[i]);
            }
        }

        // Configure crop containers
        ConfigureCropContainers(crops.ToArray());
        
        // Configure resource containers
        ConfigureResourceContainers(resources.ToArray());
    }

    private void ConfigureCropContainers(InventoryItem[] cropItems)
    {
        for (int i = 0; i < cropItems.Length; i++)
        {
            UICropContainer containerInstance 
                = Instantiate(_uiCropContainerPrefab, _cropContainersParent);

            Sprite itemIcon = GetItemIcon(cropItems[i]);
            containerInstance.Configure(itemIcon, cropItems[i].Amount);
        }
    }

    private void ConfigureResourceContainers(InventoryItem[] resourceItems)
    {
        for (int i = 0; i < resourceItems.Length; i++)
        {
            UICropContainer containerInstance 
                = Instantiate(_uiResourceContainerPrefab, _resourceContainersParent);

            Sprite itemIcon = GetItemIcon(resourceItems[i]);
            containerInstance.Configure(itemIcon, resourceItems[i].Amount);
        }
    }

    private Sprite GetItemIcon(InventoryItem item)
    {
        if (item.ItemType == ItemType.Crop)
        {
            return DataManager.Instance.GetCropSpriteFromCropType(item.CropType);
        }
        else if (item.ItemType == ItemType.Resource)
        {
            return DataManager.Instance.GetResourceSpriteFromResourceType(item.ResourceType);
        }

        Debug.LogError($"Unknown ItemType: {item.ItemType}");
        return null;
    }

    /* Method 1: Just Destroy and Recreate
    public void UpdateDisplay(Inventory inventory)
    {
        InventoryItem[] items = inventory.GetInventoryItems();

        // Clear the Crop Containers Parent if there are any UI crop containers.
        while (_cropContainersParent.childCount > 0)
        {
            Transform container = _cropContainersParent.GetChild(0);
            container.SetParent(null);
            Destroy(container.gameObject);
        }

        // Clear the Resource Containers Parent if there are any UI resource containers.
        while (_resourceContainersParent.childCount > 0)
        {
            Transform container = _resourceContainersParent.GetChild(0);
            container.SetParent(null);
            Destroy(container.gameObject);
        }

        // Create the UI containers from scratch again.
        Configure(inventory);
    }
    */

    // Method 2: Reuse
    public void UpdateDisplay(Inventory inventory)
    {
        InventoryItem[] items = inventory.GetInventoryItems();

        // Separate crops and resources
        List<InventoryItem> crops = new List<InventoryItem>();
        List<InventoryItem> resources = new List<InventoryItem>();

        for (int i = 0; i < items.Length; i++)
        {
            if (items[i].ItemType == ItemType.Crop)
            {
                crops.Add(items[i]);
            }
            else if (items[i].ItemType == ItemType.Resource)
            {
                resources.Add(items[i]);
            }
        }

        // Update crop containers
        UpdateCropContainers(crops.ToArray());
        
        // Update resource containers
        UpdateResourceContainers(resources.ToArray());
    }

    private void UpdateCropContainers(InventoryItem[] cropItems)
    {
        for (int i = 0; i < cropItems.Length; i++)
        {
            UICropContainer containerInstance;

            if (i < _cropContainersParent.childCount)
            {
                containerInstance =
                    _cropContainersParent.GetChild(i).GetComponent<UICropContainer>();
                containerInstance.gameObject.SetActive(true);

                containerInstance.UpdateDisplay(cropItems[i].Amount);
            }
            else
            {
                containerInstance
                  = Instantiate(_uiCropContainerPrefab, _cropContainersParent);
                
                Sprite itemIcon = GetItemIcon(cropItems[i]);
                containerInstance.Configure(itemIcon, cropItems[i].Amount);
            }
        }

        int remainingContainers = _cropContainersParent.childCount - cropItems.Length;

        if (remainingContainers <= 0)
            return;

        for (int i = 0; i < remainingContainers; i++)
        {
            _cropContainersParent.GetChild(cropItems.Length + i).
                gameObject.SetActive(false);
        }
    }

    private void UpdateResourceContainers(InventoryItem[] resourceItems)
    {
        for (int i = 0; i < resourceItems.Length; i++)
        {
            UICropContainer containerInstance;

            if (i < _resourceContainersParent.childCount)
            {
                containerInstance =
                    _resourceContainersParent.GetChild(i).GetComponent<UICropContainer>();
                containerInstance.gameObject.SetActive(true);

                containerInstance.UpdateDisplay(resourceItems[i].Amount);
            }
            else
            {
                containerInstance
                  = Instantiate(_uiResourceContainerPrefab, _resourceContainersParent);
                
                Sprite itemIcon = GetItemIcon(resourceItems[i]);
                containerInstance.Configure(itemIcon, resourceItems[i].Amount);
            }
        }

        int remainingContainers = _resourceContainersParent.childCount - resourceItems.Length;

        if (remainingContainers <= 0)
            return;

        for (int i = 0; i < remainingContainers; i++)
        {
            _resourceContainersParent.GetChild(resourceItems.Length + i).
                gameObject.SetActive(false);
        }
    }
}
