using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : MonoBehaviour
{
    [Header(" Elements ")]
    //[SerializeField] private Transform _resourceRenderer;
    [SerializeField] private ParticleSystem _harvestedParticles;

    public void ShotParticle()
    {
        if (_harvestedParticles == null)
            return;

        // 파티클 시스템의 복사본을 생성하여 재생
        GameObject particleCopy = Instantiate(_harvestedParticles.gameObject);
        ParticleSystem particleSystem = particleCopy.GetComponent<ParticleSystem>();
        
        particleCopy.transform.position = _harvestedParticles.transform.position;
        particleCopy.SetActive(true);
        particleSystem.Play();

        // 파티클이 끝나면 자동으로 파괴되도록 설정
        var main = particleSystem.main;
        Destroy(particleCopy, main.duration + main.startLifetime.constantMax);
    }
}
