using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crop : MonoBehaviour
{
    [Header(" Elements ")]
    [SerializeField] private Transform _cropRenderer;
    [SerializeField] private ParticleSystem _harvestedParticles;

    public void ScaleUp()
    {
        _cropRenderer.gameObject.LeanScale(Vector3.one, 1).setEase(LeanTweenType.easeOutBack);
    }
    public void ScaleDown()
    {
        _cropRenderer.gameObject.LeanScale(Vector3.zero, 1).
            setEase(LeanTweenType.easeOutBack).
            setOnComplete(() => Destroy(gameObject));

        _harvestedParticles.gameObject.SetActive(true);
        _harvestedParticles.transform.parent = null;
        _harvestedParticles.Play();
    }
}
