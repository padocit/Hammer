using System;
using UnityEngine;

public interface IUnlockable
{
    // Properties
    bool IsUnlocked { get; }
    int CurrentPrice { get; }
    int InitialPrice { get; }

    // Events
    event Action OnUnlocked;
    event Action OnPriceChanged;

    // Methods
    void Initialize(int loadedPrice);
    void TryUnlock();
    void Relock();
}