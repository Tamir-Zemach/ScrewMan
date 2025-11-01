using UnityEngine;

namespace Enemies
{
    public class Ragdoll : MonoBehaviour
    {
        [Header("Ragdoll Physics Settings")]
        [Tooltip("Force applied to each body part when the ragdoll is activated.")]
        [SerializeField] private float _moveForce = 2f;
        
        [Tooltip("Torque applied to each body part for spinning effect.")]
        [SerializeField] private float _spinForce = 2f;
        
        [Header("Destruction Settings")]
        [Tooltip("Maximum time to wait before destroying the ragdoll.")]
        [SerializeField] private float _maxLifetime = 3f;
        
        [Header("Audio Settings")]
        [Tooltip("Audio clip played when the enemy dies.")]
        [SerializeField] private AudioClip _dyingAudioClip;

        private Rigidbody2D[] _rigidBodies;
        private AudioSource _audioSource;
        private float _lifetimeTimer;

        private void Start()
        {
            _audioSource = GetComponent<AudioSource>();
            _rigidBodies = GetComponentsInChildren<Rigidbody2D>();

            if (_dyingAudioClip != null)
                _audioSource.PlayOneShot(_dyingAudioClip, 0.6f);

            ApplyRagdollForces();
        }

        private void Update()
        {
            HandleLifetime();
        }

        private void ApplyRagdollForces()
        {
            foreach (var rb in _rigidBodies)
            {
                var randomDirection = new Vector2(Random.Range(-1f, 1f), 1f);
                rb.AddForce(randomDirection * _moveForce, ForceMode2D.Impulse);
                rb.AddTorque(Random.Range(-1f, 1f) * _spinForce, ForceMode2D.Impulse);
            }
        }

        private void HandleLifetime()
        {
            _lifetimeTimer += Time.deltaTime;
            if (!(_lifetimeTimer > _maxLifetime)) return;
            Destroy(gameObject);
        }
    }
}