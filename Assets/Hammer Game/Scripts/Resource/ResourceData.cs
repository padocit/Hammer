using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Resource Data", menuName = "Scriptable Objects/ResourceData", order = 1)]
public class ResourceData : ScriptableObject
{
    [Header(" Settings ")]
    public Resource ResourcePrefab;
    public ResourceType ResourceType;
    public Sprite Icon;
}
