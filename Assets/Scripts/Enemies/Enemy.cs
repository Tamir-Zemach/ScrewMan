using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class Enemy : MonoBehaviour
{
    public PlayerStateChecker PlayerStateChecker;
    public GameObject _borderRight;
    public GameObject _borderLeft;
    public float speed = 1.0f;
    private Vector3 _startPos;
    public float _dir = 1;
    public float _shootingDelayTime = 1;
    public float _delayBeforeShot = 1;
    public float _bulletDistanceFromTurret = 1;
    public float _distanceBetweenBorders = 1;

    public bool isFacingRight = true;
    public bool _isSeeingPlayer = false;
    public bool _isSeeingPlatform = false;
    public float _playerCheckingDis;
    public LayerMask _playerLayer;
    public LayerMask _wallAndPlatformsLayer;

    public GameObject _enemyBulletRight;
    public GameObject _enemyBulletLeft;
    public int _rayCastDir;
    private bool _isShooting = false;

    public int _enemyLifeIndex;
    [SerializeField] private SpriteRenderer[] _spriteRenderer;
    public float _hitColorTimeDelay = 0.2f;

    public bool _eventAfterDeath = false;
    public GameObject _triggerToEneable;

    private Animator _animator;
    public AnimationClip _walkAnim;
    public float _walkAnimSpeed;


    public bool _bossEnemy;
    public bool _guardEnemy;
    private int _maxHealth;
    public Boss_Health_Bar _bar;
    public GameObject _bossDyingParticles;

    private bool _stopMoving;
    [SerializeField] private GameObject _regdoll;
    private PolygonCollider2D _polygonCollider;



    void Start()
    {
        _spriteRenderer = gameObject.GetComponentsInChildren<SpriteRenderer>();
        _polygonCollider = gameObject.GetComponent<PolygonCollider2D>();
        PlayerStateChecker = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStateChecker>();
        _borderRight = GameObject.FindGameObjectWithTag("Right_Border");
        _borderLeft = GameObject.FindGameObjectWithTag("Left_Border");
        _startPos = transform.position;
        _animator = GetComponentInChildren<Animator>();
        _maxHealth = _enemyLifeIndex;
    }

    private void Update()
    {
        if (!_stopMoving)
        {
            _rayCastDir = isFacingRight ? 1 : -1;
            CheckingForPlayer();
            Movement();
            Shoot();
            Dying();
            AnimationController();
        }


    }
    void Shoot()
    {
        if (_isSeeingPlayer && !_isShooting)
        {
            StartCoroutine(ShootWithDelay());
        }
    }

    IEnumerator ShootWithDelay()
    {
        _isShooting = true;
        while (_isSeeingPlayer && !_stopMoving)
        {
            yield return new WaitForSeconds(_delayBeforeShot);
            if (isFacingRight)
            {
                Instantiate(_enemyBulletRight, transform.position + new Vector3(_rayCastDir * _bulletDistanceFromTurret, 0, 0), _enemyBulletRight.transform.rotation);
            }
            else
            {
                Instantiate(_enemyBulletLeft, transform.position + new Vector3(_rayCastDir * _bulletDistanceFromTurret, 0, 0), _enemyBulletLeft.transform.rotation);
            }

            yield return new WaitForSeconds(_shootingDelayTime);
        }
        _isShooting = false;

    }


    void Movement()
    {
        if (!_isSeeingPlayer && !_isShooting)
        {
            _dir += Time.deltaTime * speed;
            transform.position = _startPos + new Vector3(isFacingRight ? _dir : -_dir, 0, 0);
        }

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.gameObject.CompareTag("Right_Border") || collision.gameObject.CompareTag("Left_Border"))
        {
            _startPos = transform.position;
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
            isFacingRight = collision.gameObject.CompareTag("Left_Border");
            _dir = 0;
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            StartCoroutine(GetHit());
        }

    }
    void CheckingForPlayer()
    {
        _isSeeingPlatform = Physics2D.Raycast(transform.position, new Vector2(_rayCastDir, 0), _playerCheckingDis, _wallAndPlatformsLayer);
        _isSeeingPlayer = Physics2D.Raycast(transform.position, new Vector2(_rayCastDir, 0), _playerCheckingDis, _playerLayer) & !PlayerStateChecker._isWaitingForDashInTheGround & !_isSeeingPlatform;
        Debug.DrawRay(transform.position, new Vector2(_rayCastDir, 0) * _playerCheckingDis, Color.red);
    }

    IEnumerator GetHit()
    {
        //change color to red for a moment after getting hit
        foreach (var _enemyPart in _spriteRenderer)
        {
            _enemyPart.color = new Color(0.5f, 0.5f, 0.5f, 1);
        }
        _enemyLifeIndex -= 1;
        if (_bossEnemy)
        {
            _bar.UpdateHealthBar(_enemyLifeIndex, _maxHealth);
        }
        yield return new WaitForSeconds(_hitColorTimeDelay);
        foreach (var _enemyPart in _spriteRenderer)
        {
            _enemyPart.color = Color.white;
        }

    }
    void Dying()
    {
        if (_enemyLifeIndex <= 0)
        {
            if (_eventAfterDeath)
            {
                _triggerToEneable.SetActive(true);
            }
            if (!_bossEnemy && !_guardEnemy)
            {
                Instantiate(_regdoll, gameObject.transform.position, gameObject.transform.rotation);
                Destroy(gameObject);
            }
            if (_guardEnemy)
            {
                StartCoroutine(DyingEvent(2));
            }
            if (_bossEnemy)
            {
                foreach (var _enemyPart in _spriteRenderer)
                {
                    _enemyPart.sortingLayerName = "Art_Non_Interactable";
                }
                StartCoroutine(BossDyingParticles());
                StartCoroutine(DyingEvent(3));
            }
            

        }
    }
    IEnumerator DyingEvent(float timetowait)
    {
        _polygonCollider.enabled = false;
        StopCoroutine(ShootWithDelay());
        _animator.SetBool("Dying", true);
        _stopMoving = true;   
        yield return new WaitForSeconds(timetowait);
        Destroy(gameObject);
    }

    IEnumerator BossDyingParticles()
    {
       for (int i = 0; i < 8; i++)
         {
            float particleOffset = Random.Range(-1f, 1f);
            Debug.Log(particleOffset);
           Instantiate(_bossDyingParticles, gameObject.transform.position + new Vector3(particleOffset, particleOffset, 0), gameObject.transform.rotation);
            yield return new WaitForSeconds(0.4f);
        }
    }

    void AnimationController()
    {
        _animator.SetFloat("WalkingSpeed", _walkAnimSpeed);
        _animator.SetBool("IsShooting", _isShooting);
        _animator.SetBool("IsSeeingPlayer", _isSeeingPlayer);
        
    }

    


}


