using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ResourceNode : MonoBehaviour
{
    private ResourceNodeState _state;
    private ResourceNodeType _type;

    [Header(" Settings ")]
    [SerializeField] private GameObject[] _stateModels; // 0 = Full, 1 = Half, 2 = Empty
    [SerializeField] private float _regrowTime = 5f;
    [SerializeField] private ResourceData _resourceData;

    [Header(" Events ")]
    public static Action<ResourceType> OnResourceHarvested;
    public static Action<ResourceNode> OnFullyHarvested;

    private Coroutine _regrowCoroutine;
    private Resource _currentResource;

    // Start is called before the first frame update
    void Start()
    {
        _state = ResourceNodeState.Full;
        SpawnResource();
        UpdateModel();
    }

    private void SpawnResource()
    {
        if (_resourceData != null && _resourceData.ResourcePrefab != null)
        {
            _currentResource = Instantiate(_resourceData.ResourcePrefab, transform.position, Quaternion.identity, transform);
        }
    }

    public void Gather()
    {
        if (_state == ResourceNodeState.Empty)
            return;

        // 리소스 획득 이벤트 발생
        OnResourceHarvested?.Invoke(_resourceData.ResourceType);
        _currentResource.ShotParticle();

        // 상태 변경
        if (_state == ResourceNodeState.Full)
        {
            _state = ResourceNodeState.Half;
        }
        else if (_state == ResourceNodeState.Half)
        {
            _state = ResourceNodeState.Empty;
            OnFullyHarvested?.Invoke(this);
            StartRegrowProcess();
        }

        UpdateModel();
    }

    private void UpdateModel()
    {
        // 모든 모델 비활성화
        for (int i = 0; i < _stateModels.Length; i++)
        {
            _stateModels[i].SetActive(false);
        }

        // 현재 상태에 맞는 모델 활성화
        int modelIndex = (int)_state;
        if (modelIndex < _stateModels.Length && _stateModels[modelIndex] != null)
        {
            _stateModels[modelIndex].SetActive(true);
        }
    }

    private void StartRegrowProcess()
    {
        if (_regrowCoroutine != null)
            StopCoroutine(_regrowCoroutine);

        _regrowCoroutine = StartCoroutine(RegrowCoroutine());
    }

    private IEnumerator RegrowCoroutine()
    {
        yield return new WaitForSeconds(_regrowTime);
        
        _state = ResourceNodeState.Full;
        UpdateModel();
        
        _regrowCoroutine = null;
    }

    public bool IsFull()
    {
        return _state == ResourceNodeState.Full;
    }

    public bool IsHalf()
    {
        return _state == ResourceNodeState.Half;
    }

    public bool IsEmpty()
    {
        return _state == ResourceNodeState.Empty;
    }

    public bool CanGather()
    {
        return _state != ResourceNodeState.Empty;
    }

    public ResourceNodeState GetState()
    {
        return _state;
    }

    public ResourceNodeType GetResourceNodeType()
    {
        return _type;
    }

    public ResourceType GetResourceType()
    {
        return _resourceData.ResourceType;
    }

    public ResourceData GetResourceData()
    {
        return _resourceData;
    }

    private void OnDestroy()
    {
        if (_regrowCoroutine != null)
            StopCoroutine(_regrowCoroutine);
    }
}
