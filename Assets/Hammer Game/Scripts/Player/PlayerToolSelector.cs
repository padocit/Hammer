using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class PlayerToolSelector : MonoBehaviour
{
    public enum Tool { None, Sow, Water, Harvest, Resource }
    private Tool _activeTool;

    [Header(" Elements ")]
    [SerializeField] private Image[] _toolImages;

    [Header(" Settings ")]
    [SerializeField] private Color _selectedToolColor;

    [Header(" Actions ")]
    public Action<Tool> OnToolSelected;

    // Start is called before the first frame update
    void Start()
    {
        SelectTool(Tool.None);
    }

    // 기존 메서드 (하위 호환성 유지)
    public void SelectTool(int toolIndex)
    {
        SelectTool((Tool)toolIndex);
    }

    // 새로운 메서드 (Tool enum 직접 사용)
    public void SelectTool(Tool tool)
    {
        _activeTool = tool;
        int toolIndex = (int)tool;

        for (int i = 0; i < _toolImages.Length; i++)
            _toolImages[i].color = (i == toolIndex) ? _selectedToolColor : Color.white;
   
        OnToolSelected?.Invoke(_activeTool);
    }

    public Tool GetActiveTool()
    {
        return _activeTool;
    }

    public bool CanSow()
    {
        return _activeTool == Tool.Sow;
    }

    public bool CanWater()
    {
        return _activeTool == Tool.Water;
    }
    
    public bool CanHarvest()
    {
        return _activeTool == Tool.Harvest;
    }

    public bool CanGatherResource()
    {
        return _activeTool == Tool.Resource;
    }
}
