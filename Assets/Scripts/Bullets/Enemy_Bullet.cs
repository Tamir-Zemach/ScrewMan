using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Bullet : MonoBehaviour
{
    private Rigidbody2D _playerRb;
    public PlayerStateChecker PlayerStateChecker;
    public float _bulletSpeed;
    public float _knockBackForce = 10f;
    public int _bulletDiraction;
    public GameObject _onImpactAnim;
    public bool _bossEnemyBullet;


    private void Awake()
    {
        _playerRb = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody2D>();
        PlayerStateChecker = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStateChecker>();
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
    }

    void Movement()
    {
        transform.position = transform.position + new Vector3(_bulletDiraction, 0, 0) * Time.deltaTime * _bulletSpeed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Vector3 _knockBackDir = (collision.transform.position - transform.position).normalized;  //??
            if (PlayerStateChecker.isGrounded)
            {
                _playerRb.AddForce(Vector2.up * _knockBackForce, ForceMode2D.Impulse);
            }

            _playerRb.AddForce(_knockBackDir * _knockBackForce, ForceMode2D.Impulse);
            if (PlayerStateChecker.isDashingDown)
            {

                {
                    _playerRb.velocity = Vector3.zero;
                    _playerRb.AddForce(_knockBackDir * _knockBackForce, ForceMode2D.Impulse);
                    PlayerStateChecker.isDashingDown = false;
                }

            }

        }

       OnImpact(); 

    }
    void OnImpact()
    {
        if (!_bossEnemyBullet)
        {
            Instantiate(_onImpactAnim, gameObject.transform.position, gameObject.transform.rotation);
        }
        Destroy(gameObject);
    }

}




