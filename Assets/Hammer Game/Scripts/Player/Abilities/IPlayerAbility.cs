using UnityEngine;

public interface IPlayerAbility
{
    bool IsActive { get; }
    void Activate();
    void Deactivate();
    void HandleTargetEntered<T>(T target) where T : Component;
    void HandleTargetExited<T>(T target) where T : Component;
}

public interface ITargetHandler<T> where T : Component
{
    bool CanInteract(T target);
    void StartInteraction(T target);
    void StopInteraction();
}