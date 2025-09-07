using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [Header(" Elements ")]
    private Transform _target = null;

    [Header(" Settings ")]
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _knockbackDuration = 0.3f;
    
    [Header(" Knockback ")]
    private bool _isKnockedBack = false;
    private Vector3 _knockbackVelocity;
    private float _knockbackTimer;

    // Update is called once per frame
    void Update()
    {
        if (_isKnockedBack)
            HandleKnockback();
        else if (_target != null)
            FollowPlayer();
    }
    public void SetTarget(Transform target)
    {
        _target = target;
    }

    public void ApplyKnockback(Vector3 knockbackForce)
    {
        _isKnockedBack = true;
        _knockbackVelocity = knockbackForce;
        _knockbackTimer = _knockbackDuration;
    }

    private void HandleKnockback()
    {
        if (_knockbackTimer > 0)
        {
            // Apply knockback movement
            Vector3 knockbackMovement = _knockbackVelocity * Time.deltaTime;
            transform.position += knockbackMovement;
            
            // Reduce knockback velocity over time
            _knockbackVelocity = Vector3.Lerp(_knockbackVelocity, Vector3.zero, Time.deltaTime / _knockbackTimer);
            _knockbackTimer -= Time.deltaTime;
        }
        else
        {
            _isKnockedBack = false;
            _knockbackVelocity = Vector3.zero;
        }
    }

    private void FollowPlayer()
    {
        // Y = 0 plane movement
        Vector3 directionToPlayer = _target.transform.position - transform.position;
        directionToPlayer.y = 0;
        directionToPlayer = directionToPlayer.normalized;

        Vector3 targetPosition = transform.position + directionToPlayer * _moveSpeed * Time.deltaTime;

        transform.position = targetPosition;
        // Look at player position directly for instant rotation
        if (directionToPlayer != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(directionToPlayer);
        }
    }
}
