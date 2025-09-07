using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[RequireComponent(typeof(InventoryDisplay))]
public class InventoryManager : MonoBehaviour
{
    private Inventory _inventory;
    private InventoryDisplay _inventoryDisplay;
    private string _dataPath;

    // Start is called before the first frame update
    void Start()
    {
        _dataPath = Application.dataPath + "/InventoryData.txt";

        LoadInventory();
        ConfigureInventoryDisplay();

        CropTile.OnCropHarvested += CropHarvestedCallback;
        ResourceNode.OnResourceHarvested += ResourceHarvestedCallback;
    }

    private void OnDestroy()
    {
        CropTile.OnCropHarvested -= CropHarvestedCallback;
        ResourceNode.OnResourceHarvested -= ResourceHarvestedCallback;
    }

    private void ConfigureInventoryDisplay()
    {
        _inventoryDisplay = GetComponent<InventoryDisplay>();
        _inventoryDisplay.Configure(_inventory);
    }

    private void CropHarvestedCallback(CropType cropType)
    {
        // Update inventory
        _inventory.CropHarvestedCallback(cropType);

        _inventoryDisplay.UpdateDisplay(_inventory);
        SaveInventory();
    }

    private void ResourceHarvestedCallback(ResourceType resourceType)
    {
        // Update inventory
        _inventory.ResourceCollectedCallback(resourceType);

        _inventoryDisplay.UpdateDisplay(_inventory);
        SaveInventory();
    }

    [NaughtyAttributes.Button]
    public void ClearInventory()
    {
        _inventory.Clear();
        UpdateInventory();
    }

    public void UpdateInventory()
    {
        _inventoryDisplay.UpdateDisplay(_inventory);
        SaveInventory();
    }

    public Inventory GetInventory()
    {
        return _inventory;
    }

    // ���� ó�� ���� �ذ�
    private void LoadInventory()
    {
        try
        {
            if (File.Exists(_dataPath))
            {
                string data = File.ReadAllText(_dataPath);
                
                if (!string.IsNullOrEmpty(data))
                {
                    _inventory = JsonUtility.FromJson<Inventory>(data);
                }
                
                if (_inventory == null)
                {
                    _inventory = new Inventory();
                    Debug.Log("Failed to load inventory data, creating new inventory");
                }
                else
                {
                    Debug.Log("Inventory loaded successfully");
                }
            }
            else
            {
                // ������ ������ ���� �����ϰ� ��� �ݱ�
                CreateEmptyInventoryFile();
                _inventory = new Inventory();
                Debug.Log("Inventory file not found, created new inventory");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error loading inventory: {e.Message}");
            _inventory = new Inventory();
        }
    }
    
    private void CreateEmptyInventoryFile()
    {
        try
        {
            // using ���� ����ؼ� ���� ��Ʈ���� �ڵ����� ����
            using (var fileStream = File.Create(_dataPath))
            {
                // ���ϸ� �����ϰ� ��� ����
            }
            
            // �� �κ��丮 ������ ����
            var emptyInventory = new Inventory();
            string emptyData = JsonUtility.ToJson(emptyInventory, true);
            File.WriteAllText(_dataPath, emptyData);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error creating inventory file: {e.Message}");
        }
    }
    
    private void SaveInventory()
    {
        try
        {
            string data = JsonUtility.ToJson(_inventory, true);
            File.WriteAllText(_dataPath, data);
            Debug.Log("Inventory saved successfully");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error saving inventory: {e.Message}");
        }
    }
}
