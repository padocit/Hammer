using UnityEngine;

public class EnemyDeathEffect : MonoBehaviour
{
    [Header("Death Effects")]
    [SerializeField] private ParticleSystem _deathParticleSystem;
    [SerializeField] private AudioClip _deathSound;
    [SerializeField] private float _destroyDelay = 0.1f;

    private void Awake()
    {
        // EnemyHealth의 사망 이벤트에 구독
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
        // 파티클 효과 재생
        PlayDeathParticle();
        
        // 사운드 효과 재생
        PlayDeathSound();
        
        // Enemy 오브젝트 파괴 (약간의 딜레이)
        Destroy(gameObject, _destroyDelay);
    }

    private void PlayDeathParticle()
    {
        if (_deathParticleSystem != null)
        {
            // 파티클을 부모에서 분리하여 독립적으로 재생
            _deathParticleSystem.transform.SetParent(null, true);
            _deathParticleSystem.Play();
            
            // 파티클 재생이 끝나면 파티클 오브젝트도 파괴
            var main = _deathParticleSystem.main;
            float particleDuration = main.duration + main.startLifetime.constantMax;
            Destroy(_deathParticleSystem.gameObject, particleDuration);
        }
    }

    private void PlayDeathSound()
    {
        if (_deathSound != null)
        {
            // 3D 위치에서 사운드 재생 (LeanAudio 방식 활용)
            AudioSource.PlayClipAtPoint(_deathSound, transform.position);
        }
    }
}