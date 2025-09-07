using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkWalls : MonoBehaviour
{
    [Header(" Elements ")]
    [SerializeField] private GameObject _frontWall;
    [SerializeField] private GameObject _rightWall;
    [SerializeField] private GameObject _backWall;
    [SerializeField] private GameObject _leftWall;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Configure(int configuration)
    {
        // Set walls active if the wall is locked.
        _frontWall.SetActive(!IsWallUnlockedBitSet(configuration, 0));
        _rightWall.SetActive(!IsWallUnlockedBitSet(configuration, 1));
        _backWall.SetActive(!IsWallUnlockedBitSet(configuration, 2));
        _leftWall.SetActive(!IsWallUnlockedBitSet(configuration, 3));
    }

    public bool IsWallUnlockedBitSet(int configuration, int bitOfWall)
    {
        if ((configuration & (1 << bitOfWall)) > 0)
            return true;

        return false;
    }
}
