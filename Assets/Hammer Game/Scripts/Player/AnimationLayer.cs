using UnityEngine;

public enum AnimationLayer
{
    Base = 0,
    Combat = 1,
    Farming = 2,
    Water = 3,
    Harvest = 4,
    Tree = 5,
    Rock = 6
}

public static class AnimationExtensions
{
    public static void SetLayerWeight(this Animator animator, AnimationLayer layer, float weight)
    {
        animator.SetLayerWeight((int)layer, weight);
    }
}