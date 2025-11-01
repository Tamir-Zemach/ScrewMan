using System.Collections;
using UnityEngine;

namespace Enemies
{
    public class StaticTurret : MonoBehaviour
    {
        [Header("Shooting Settings")]
        [Tooltip("Delay before turret starts shooting after activation.")]
        [SerializeField] private float _shootingDelayTime = 1f;

        [Tooltip("Delay between shooting animation and bullet spawn.")]
        [SerializeField] private float _timeBetweenShotAndAnim = 1f;

        [Tooltip("Time to display hit color before reverting.")]
        [SerializeField] private float _hitColorTimeDelay = 0.2f;

        [Tooltip("Bullet prefab to instantiate when shooting.")]
        [SerializeField] private GameObject _enemyBullet;

        [Header("Enemy State")]
        [Tooltip("Initial life index of the turret.")]
        public int EnemyLifeIndex = 3;

        [Tooltip("Prefab to instantiate when turret is destroyed.")]
        [SerializeField] private GameObject _deadStaticEnemy;

        [Header("Internal State")]
        [Tooltip("Flag to prevent multiple shooting coroutines.")]
        [SerializeField] private bool _isShooting;
        
        [Tooltip("Flag used to trigger shooting animation.")]
        public bool IsShootingForAnim;

        private Vector3 _bulletSpawnPos;
        private SpriteRenderer[] _turretRenderers;
        private GameManager _gameManager;
        private Animator _animator;

        private void Start()
        {
            _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
            _turretRenderers = GetComponentsInChildren<SpriteRenderer>();
            _bulletSpawnPos = transform.Find("Bullet_Spawn_Position").position;
            _animator = GetComponentInChildren<Animator>();
        }

        private void Update()
        {
            if (EnemyLifeIndex <= 0)
            {
                Instantiate(_deadStaticEnemy, transform.position, transform.rotation);
                Destroy(gameObject);
                return;
            }

            if (!_isShooting && _gameManager.HasGameStarted)
            {
                Shoot();
            }

            UpdateAnimationState();
        }

        private void Shoot()
        {
            StartCoroutine(ShootWithDelay());
        }

        private IEnumerator ShootWithDelay()
        {
            _isShooting = true;

            while (_gameManager.HasGameStarted)
            {
                yield return new WaitForSeconds(_shootingDelayTime);
                IsShootingForAnim = true;

                yield return new WaitForSeconds(_timeBetweenShotAndAnim);
                Instantiate(_enemyBullet, _bulletSpawnPos, _enemyBullet.transform.rotation);
                IsShootingForAnim = false;
            }

            _isShooting = false;
        }

        private IEnumerator GetHit()
        {
            foreach (var spriteRenderer in _turretRenderers)
            {
                spriteRenderer.color = new Color(0.5f, 0.5f, 0.5f, 1f);
            }

            EnemyLifeIndex--;

            yield return new WaitForSeconds(_hitColorTimeDelay);

            foreach (var spriteRenderer in _turretRenderers)
            {
                spriteRenderer.color = Color.white;
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Bullet"))
            {
                StartCoroutine(GetHit());
            }
        }

        private void UpdateAnimationState()
        {
            _animator.SetBool("isShooting", IsShootingForAnim);
        }
    }
}