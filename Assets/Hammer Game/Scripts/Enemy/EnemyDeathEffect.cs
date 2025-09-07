using UnityEngine;

public class EnemyDeathEffect : MonoBehaviour
{
    [Header("Death Effects")]
    [SerializeField] private ParticleSystem _deathParticleSystem;
    [SerializeField] private AudioClip _deathSound;
    [SerializeField] private float _destroyDelay = 0.1f;

    private void Awake()
    {
        // EnemyHealth�� ��� �̺�Ʈ�� ����
        var enemyHealth = GetComponent<EnemyHealth>();
        if (enemyHealth != null)
        {
            enemyHealth.onPassAway += PlayDeathEffects;
        }
    }

    private void OnDestroy()
    {
        var enemyHealth = GetComponent<EnemyHealth>();
        if (enemyHealth != null)
        {
            enemyHealth.onPassAway -= PlayDeathEffects;
        }
    }

    private void PlayDeathEffects()
    {
        // ��ƼŬ ȿ�� ���
        PlayDeathParticle();
        
        // ���� ȿ�� ���
        PlayDeathSound();
        
        // Enemy ������Ʈ �ı� (�ణ�� ������)
        Destroy(gameObject, _destroyDelay);
    }

    private void PlayDeathParticle()
    {
        if (_deathParticleSystem != null)
        {
            // ��ƼŬ�� �θ𿡼� �и��Ͽ� ���������� ���
            _deathParticleSystem.transform.SetParent(null, true);
            _deathParticleSystem.Play();
            
            // ��ƼŬ ����� ������ ��ƼŬ ������Ʈ�� �ı�
            var main = _deathParticleSystem.main;
            float particleDuration = main.duration + main.startLifetime.constantMax;
            Destroy(_deathParticleSystem.gameObject, particleDuration);
        }
    }

    private void PlayDeathSound()
    {
        if (_deathSound != null)
        {
            // 3D ��ġ���� ���� ��� (LeanAudio ��� Ȱ��)
            AudioSource.PlayClipAtPoint(_deathSound, transform.position);
        }
    }
}