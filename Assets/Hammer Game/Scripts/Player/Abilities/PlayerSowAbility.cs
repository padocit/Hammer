using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(PlayerAnimator))]
[RequireComponent(typeof(PlayerToolSelector))]
public class PlayerSowAbility : PlayerAbilityBase<CropField>
{
    [Header("Sow Settings")]
    private bool _isProcessingSeeds;
    
    protected override void Awake()
    {
        _requiredTool = PlayerToolSelector.Tool.Sow;
        _targetTag = "CropField";
        base.Awake();
    }
    
    protected override void SubscribeToEvents()
    {
        base.SubscribeToEvents();
        SeedParticles.OnSeedsCollided += OnSeedsCollided;
        CropField.OnFullySown += OnCropFieldFullySown;
    }
    
    protected override void UnsubscribeFromEvents()
    {
        base.UnsubscribeFromEvents();
        SeedParticles.OnSeedsCollided -= OnSeedsCollided;
        CropField.OnFullySown -= OnCropFieldFullySown;
    }
    
    private void OnSeedsCollided(Vector3[] seedPositions)
    {
        if (_currentTarget == null || !_isActive)
            return;
            
        _currentTarget.SeedsCollidedCallback(seedPositions);
    }
    
    private void OnCropFieldFullySown(CropField cropField)
    {
        if (cropField == _currentTarget)
        {
            StopInteraction();
        }
    }
    
    // 자동 툴 선택을 위해 툴 체크 제거
    public override bool CanInteract(CropField target)
    {
        return target != null && target.IsEmpty();
    }
    
    public override void StartInteraction(CropField target)
    {
        if (_currentTarget == target && _isActive) 
            return;
            
        _currentTarget = target;
        Activate(); // 자동 툴 선택
        
        _playerAnimator?.PlaySowAnimation();
        _isProcessingSeeds = true;
        
        Debug.Log($"Started sowing in field: {target.name}");
    }
    
    public override void StopInteraction()
    {
        if (_currentTarget != null)
        {
            _playerAnimator?.StopSowAnimation();
            _isProcessingSeeds = false;
            Debug.Log($"Stopped sowing in field: {_currentTarget.name}");
            _currentTarget = null;
        }
        
        Deactivate();
    }
    
    protected override void OnToolSelectedCallback(PlayerToolSelector.Tool selectedTool)
    {
        if (selectedTool != _requiredTool && _isActive)
        {
            StopInteraction();
        }
    }
}

