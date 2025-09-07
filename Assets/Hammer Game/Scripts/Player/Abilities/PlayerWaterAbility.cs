using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerAnimator))]
[RequireComponent(typeof(PlayerToolSelector))]
public class PlayerWaterAbility : PlayerAbilityBase<CropField>
{
    [Header("Water Settings")]
    private bool _isProcessingWater;
    
    protected override void Awake()
    {
        _requiredTool = PlayerToolSelector.Tool.Water;
        _targetTag = "CropField";
        base.Awake();
    }
    
    protected override void SubscribeToEvents()
    {
        base.SubscribeToEvents();
        WaterParticles.OnWaterCollided += OnWaterCollided;
        CropField.OnFullyWatered += OnCropFieldFullyWatered;
    }
    
    protected override void UnsubscribeFromEvents()
    {
        base.UnsubscribeFromEvents();
        WaterParticles.OnWaterCollided -= OnWaterCollided;
        CropField.OnFullyWatered -= OnCropFieldFullyWatered;
    }
    
    private void OnWaterCollided(Vector3[] waterPositions)
    {
        if (_currentTarget == null || !_isActive)
            return;
            
        _currentTarget.WaterCollidedCallback(waterPositions);
    }
    
    private void OnCropFieldFullyWatered(CropField cropField)
    {
        if (cropField == _currentTarget)
        {
            StopInteraction();
        }
    }
    
    // �ڵ� �� ������ ���� �� üũ ����
    public override bool CanInteract(CropField target)
    {
        return target != null && target.IsSown() && !target.IsWatered();
    }
    
    public override void StartInteraction(CropField target)
    {
        if (_currentTarget == target && _isActive) 
            return;
            
        _currentTarget = target;
        Activate(); // �ڵ� �� ����
        
        _playerAnimator?.PlayWaterAnimation();
        _isProcessingWater = true;
        
        Debug.Log($"Started watering field: {target.name}");
    }
    
    public override void StopInteraction()
    {
        if (_currentTarget != null)
        {
            _playerAnimator?.StopWaterAnimation();
            _isProcessingWater = false;
            Debug.Log($"Stopped watering field: {_currentTarget.name}");
        }
        
        // DeactivateInternal ������� ��ȯ ȣ�� ����
        DeactivateInternal();
    }
    
    protected override void OnToolSelectedCallback(PlayerToolSelector.Tool selectedTool)
    {
        if (selectedTool != _requiredTool && _isActive)
        {
            StopInteraction();
        }
    }
}


