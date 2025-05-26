using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Static_Turret : MonoBehaviour
{
    public float _shootingDelayTime = 1;
    public float _timeBetweenShotAndAnim = 1;
    public float _hitColorTimeDelay = 0.2f;
    public GameObject _enemyBullet;
    private Vector3 _bulletSpawnPos;
    private SpriteRenderer[] _turretRenderer;
    private GameManager _gameManager;
    public int _enemyLifeIndex;
    [SerializeField] private bool _isShooting;
    public bool _isShootingForAnim;
    private Animator _animator;
    [SerializeField] private GameObject _deadStaticEnemy;

    // Start is called before the first frame update
    void Start()
    {
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        _turretRenderer = GetComponentsInChildren<SpriteRenderer>();
        _bulletSpawnPos = transform.Find("Bullet_Spawn_Position").position;
        _animator = GetComponentInChildren<Animator>();

    }

    // Update is called once per frame
    void Update()
    {
        if (_enemyLifeIndex <= 0)
        {
            Instantiate(_deadStaticEnemy, gameObject.transform.position, gameObject.transform.rotation);
            Destroy(gameObject);
        }
        if (!_isShooting && _gameManager._hasGameStarted)
        {
            Shoot();
        }
        AnimationStates();
        

    }
    public void Shoot()
    {
        if (_gameManager._hasGameStarted)
        {
            StartCoroutine(ShootWithDelay());
        }
    }
    IEnumerator ShootWithDelay()
    {
        _isShooting = true;
        while (_gameManager._hasGameStarted)
        {
            yield return new WaitForSeconds(_shootingDelayTime);
            _isShootingForAnim = true;

            yield return new WaitForSeconds(_timeBetweenShotAndAnim);
            Instantiate(_enemyBullet, _bulletSpawnPos, _enemyBullet.transform.rotation);
            _isShootingForAnim = false;

        }
        _isShooting = false;
    }





    IEnumerator GetHit()
    {
        //change color to red for a moment after getting hit
        foreach (var turretPart in _turretRenderer)
        {
            turretPart.color = new Color(0.5f, 0.5f, 0.5f, 1);
        }
        _enemyLifeIndex -= 1;
        yield return new WaitForSeconds(_hitColorTimeDelay);
        foreach (var turretPart in _turretRenderer)
        {
            turretPart.color = Color.white;
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            StartCoroutine(GetHit());
        }
    }


    void AnimationStates()
    {
        _animator.SetBool("isShooting", _isShootingForAnim);
    }

    
}