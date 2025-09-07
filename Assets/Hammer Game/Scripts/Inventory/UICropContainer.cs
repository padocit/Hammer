using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UICropContainer : MonoBehaviour
{
    [Header(" Elements ")]
    [SerializeField] private Image _iconImage;
    [SerializeField] private TextMeshProUGUI _amountText; 

    public void Configure(Sprite icon, int amount)
    {
        _iconImage.sprite = icon;
        _amountText.text = amount.ToString();
    }

    public void UpdateDisplay(int amount)
    {
        _amountText.text = amount.ToString();
    }
}
