using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    [Header(" Settings ")]
    [SerializeField] private int _maxHealth;
    private int _health;

    [Header(" Elements ")]
    [SerializeField] private Slider _healthSlider;
    [SerializeField] private TextMeshProUGUI _healthText;
    private EnemyMovement _enemyMovement;

    [Header(" Actions ")]
    public Action onPassAway;
    public Action OnDeath;

    // Start is called before the first frame update
    void Start()
    {
        _health = _maxHealth;
        _enemyMovement = GetComponent<EnemyMovement>();
        UpdateHealthUI();
    }

    public void TakeDamage(int damage)
    {
        int realDamage = Mathf.Min(damage, _health);
        Debug.Log("Enemy took " + realDamage + " damage.");
        _health -= realDamage;

        UpdateHealthUI();

        if (_health <= 0)
            PassAway();
    }

    public void TakeDamage(int damage, Vector3 knockbackForce)
    {
        TakeDamage(damage);
        
        // Apply knockback if enemy is still alive
        if (_health > 0 && _enemyMovement != null)
        {
            _enemyMovement.ApplyKnockback(knockbackForce);
        }
    }

    private void PassAway()
    {
        onPassAway?.Invoke();
    }
    
    private void UpdateHealthUI()
    {
        _healthSlider.value = (float)_health / _maxHealth;
        _healthText.text = _health + " / " + _maxHealth;
    }
}
