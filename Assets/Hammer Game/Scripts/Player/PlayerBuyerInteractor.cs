using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBuyerInteractor : MonoBehaviour
{
    [Header(" Elements ")]
    [SerializeField] private InventoryManager _inventoryManager;

    [Header(" Settings ")]
    [SerializeField] private int _swordPrice = 10; // Price per sword in coins

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
        if (other.CompareTag("Buyer"))
            SellSwords();
    }

    private void SellSwords()
    {

        Debug.Log("Selling swords...");
        int swordAmount = CurrencyManager.Instance.GetCurrency(CurrencyType.Sword);

        if (swordAmount <= 0)
            return;

        int coinsEarned = swordAmount * _swordPrice;

        // Remove all swords from currency
        CurrencyManager.Instance.UseCurrency(CurrencyType.Sword, swordAmount);

        // Give coins to the player
        CurrencyManager.Instance.AddCoins(coinsEarned);
    }
}
