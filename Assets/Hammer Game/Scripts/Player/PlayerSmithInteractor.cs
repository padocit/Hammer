using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSmithInteractor : MonoBehaviour
{
    [Header(" Elements ")]
    [SerializeField] private InventoryManager _inventoryManager;
    private bool _isMinigameRunning;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (_isMinigameRunning == false && other.CompareTag("Smith"))
            MakeSwords();
    }

    private void MakeSwords()
    {
        Inventory inventory = _inventoryManager.GetInventory();
        InventoryItem[] items = inventory.GetInventoryItems();

        InventoryItem stone = null;
        InventoryItem wood = null;
        int numStoneInInventory = 0;
        int numWoodInInventory = 0;
        for (int i = 0; i < items.Length; i++)
        {
            // Calculate the number of swords made
            if (items[i].ItemType == ItemType.Resource && items[i].ResourceType == ResourceType.Stone)
            {
                numStoneInInventory += items[i].Amount;
                stone = items[i];
            }
            if (items[i].ItemType == ItemType.Resource && items[i].ResourceType == ResourceType.Wood)
            {
                numWoodInInventory += items[i].Amount;
                wood = items[i];
            }
        }

        int numSwordsMade = Mathf.Min(numStoneInInventory, numWoodInInventory);

        if (numSwordsMade <= 0)
            return;

        stone.Amount -= numSwordsMade;
        wood.Amount -= numSwordsMade;

        // Minigame to determine actual number of swords made (x0, x1, x3)
        _isMinigameRunning = true;
        MinigameManager.Instance.ShowBarMinigame((multiplier) =>
        {
            Debug.Log($"Made {numSwordsMade} swords with x{multiplier} multiplier!");

            // Give swords to the player
            CurrencyManager.Instance.AddCurrency(CurrencyType.Sword, numSwordsMade * multiplier);
            _isMinigameRunning = false;
        });


        // Update the inventory
        _inventoryManager.UpdateInventory();
    }
}
