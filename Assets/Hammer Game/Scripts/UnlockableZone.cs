using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockableZone : UnlockableBase
{
    private void Start()
    {
        Debug.Log($"UnlockableZone {gameObject.name} - Initial Price: {InitialPrice}");
    }

    protected override void OnUnlockComplete()
    {
        Debug.Log($"Building {gameObject.name} constructed!");
    }

    protected override void OnLockComplete()
    {
        Debug.Log($"Building {gameObject.name} demolished!");
    }
}
