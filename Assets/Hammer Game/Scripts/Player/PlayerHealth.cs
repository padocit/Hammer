using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [Header(" Settings ")]
    [SerializeField] private int _maxHealth;
    private int _health;

    [Header(" Elements ")]
    [SerializeField] private Slider _healthSlider;
    [SerializeField] private TextMeshProUGUI _healthText;

    // Start is called before the first frame update
    void Start()
    {
        _health = _maxHealth;
        UpdateHealthUI();
    }

    public void TakeDamage(int damage)
    {
        _health = Mathf.Max(_health - damage, 0);
        UpdateHealthUI();

        if (_health <= 0)
            PassAway();
    }

    private void PassAway()
    {
        Debug.Log("Player Died");
        SceneManager.LoadScene(1);
    }

    private void UpdateHealthUI()
    {
        _healthSlider.value = (float)_health / _maxHealth;
        _healthText.text = _health + " / " + _maxHealth;
    }
}
