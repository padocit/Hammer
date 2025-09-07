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

        // ��ƼŬ �ý����� ���纻�� �����Ͽ� ���
        GameObject particleCopy = Instantiate(_harvestedParticles.gameObject);
        ParticleSystem particleSystem = particleCopy.GetComponent<ParticleSystem>();
        
        particleCopy.transform.position = _harvestedParticles.transform.position;
        particleCopy.SetActive(true);
        particleSystem.Play();

        // ��ƼŬ�� ������ �ڵ����� �ı��ǵ��� ����
        var main = particleSystem.main;
        Destroy(particleCopy, main.duration + main.startLifetime.constantMax);
    }
}
