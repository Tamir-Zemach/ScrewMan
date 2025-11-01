
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private PlayerStateChecker PlayerStateChecker;  
    private Rigidbody2D _bulletRB;
    private Collider2D _bulletCollider;
    private Transform _bullettransform;
    public float _verticalSpeed;
    public float _horizontalSpeed;
    public float _bounceSpeed;
    [SerializeField] private float _maxWaitTime;
    private float _waitTimeBeforeDestroy;

    private bool _hasBounced = false;
    public float _rotationSpeed;
    public float _growinSpeed;
    public float _startSize;

    private AudioSource _audioSource;
    [SerializeField] private AudioClip _hitMonsterAudioClip;
    [SerializeField] private AudioClip _hitWaterEnemyAudioClip;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _bulletRB = GetComponent<Rigidbody2D>();
        _bulletCollider = GetComponent<Collider2D>();
        _bullettransform = GetComponent<Transform>();
        _bullettransform.localScale = new Vector3(_startSize, _startSize, _startSize);
        _bulletRB.AddForce(Vector2.up * _verticalSpeed, ForceMode2D.Impulse);
        PlayerStateChecker = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStateChecker>();
        SideOfShot();
    }
    void Update()
    {
        WaitUntilDestroy();
        GrowInScale();
    }
    void GrowInScale()
    {
        //the bullet start small and grows to full size
        if (_bullettransform.localScale.x < 1 && _bullettransform.localScale.y < 1 && _bullettransform.localScale.z < 1)
        {
            transform.localScale += new Vector3(0.1f, 0.1f, 0.1f) * Time.deltaTime * _growinSpeed ;
        }
    }

    void WaitUntilDestroy()
    {
        //after a while the bullet is destroyed 
        _waitTimeBeforeDestroy += Time.deltaTime;
        if (_waitTimeBeforeDestroy > _maxWaitTime)
        {
            Destroy(gameObject);
        }

    }
    void SideOfShot()
    {
        //depand on the side of rhe player - the bullet will fly in a diraction and rotate also 
        if (PlayerStateChecker.IsLookingRight)
        {
            _bulletRB.AddForce(Vector2.right * _horizontalSpeed, ForceMode2D.Impulse);
            _bulletRB.AddTorque(-_rotationSpeed, ForceMode2D.Impulse);
        }
        else
        {
            _bulletRB.AddForce(Vector2.left * _horizontalSpeed, ForceMode2D.Impulse);
            _bulletRB.AddTorque(_rotationSpeed, ForceMode2D.Impulse);
        }

    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player") && !collision.gameObject.CompareTag("Bullet"))
        {
            if (collision.gameObject.CompareTag("Enemy"))
            {
                _audioSource.PlayOneShot(_hitMonsterAudioClip, 0.4f);
                transform.parent = collision.transform;
                StuckOnObject();
            }
            if (collision.gameObject.CompareTag("Static_Enemy"))
            {
                _audioSource.PlayOneShot(_hitWaterEnemyAudioClip, 1f);
                transform.parent = collision.transform;
                StuckOnObject();
            }
            else if (_hasBounced)
            {
                StuckOnObject();
                if (collision.gameObject.CompareTag("Moveable_Object"))
                {
                    transform.parent = collision.transform;
                }
            }
            else
            {
                _bulletRB.AddForce(Vector2.up * _bounceSpeed, ForceMode2D.Impulse);
                _hasBounced = true;
            }
        }
    }

    void StuckOnObject()
    {
        //after one bounce the bullet stick to the surface
        _bulletRB.velocity = Vector2.zero;
        _bulletRB.isKinematic = true;
        _bulletRB.freezeRotation = true;
        _bulletCollider.enabled = false;

    }

}

