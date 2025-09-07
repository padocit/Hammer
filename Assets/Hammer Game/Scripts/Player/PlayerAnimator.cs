using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [Header(" Elements ")]
    [SerializeField] private Animator _animator;
    [SerializeField] private ParticleSystem _waterParticles;

    [Header(" Settings ")]
    [SerializeField] private float _moveSpeedMultiplier;

    public void ManageAnimations(Vector3 moveVector)
    {
        if (moveVector.magnitude > 0)
        {
            _animator.SetFloat("moveSpeed", moveVector.magnitude * _moveSpeedMultiplier);
            PlayRunAnimation();

            _animator.transform.forward = moveVector.normalized;
        }
        else
        {
            PlayIdleAnimation();
        }
    }

    private void PlayRunAnimation()
    {
        _animator.Play("Run");
    }
    private void PlayIdleAnimation()
    {
        _animator.Play("Idle");
    }

    #region Roguelike Animation
    public void PlayAttackAnimation()
    {
        _animator.SetLayerWeight(1, 1);
    }

    public void StopAttackAnimation()
    {
        _animator.SetLayerWeight(1, 0);
    }
    #endregion

    #region Farmer Animation
    public void PlaySowAnimation()
    {
        _animator.SetLayerWeight(1, 1);
    }
    public void StopSowAnimation()
    {
        _animator.SetLayerWeight(1, 0);
    }

    public void PlayWaterAnimation()
    {
        _animator.SetLayerWeight(2, 1);
    }
    public void StopWaterAnimation()
    {
        _animator.SetLayerWeight(2, 0);
        _waterParticles.Stop();
    }

    public void PlayHarvestAnimation()
    {
        _animator.SetLayerWeight(3, 1);
    }
    public void StopHarvestAnimation()
    {
        _animator.SetLayerWeight(3, 0);
    }

    public void PlayTreeAnimation()
    {
        _animator.SetLayerWeight(4, 1);
    }
    public void StopTreeAnimation()
    {
        _animator.SetLayerWeight(4, 0);
    }

    public void PlayRockAnimation()
    {
        _animator.SetLayerWeight(5, 1);
    }
    public void StopRockAnimation()
    {
        _animator.SetLayerWeight(5, 0);
    }
    #endregion
}
