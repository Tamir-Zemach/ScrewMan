
using UnityEngine;

public class Static_Turret_Bullet : MonoBehaviour
{
    private Rigidbody2D _playerRb;
    private PlayerStateChecker PlayerStateChecker;
    public float _bulletSpeed;
    public float _knockBackForce = 10f;
    public bool _bulletFacingRight = false;
    public bool _bulletFacingDown = false;
    public GameObject _onImpactAnim;

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
        Vector3 direction = _bulletFacingDown ? Vector3.down : (_bulletFacingRight ? Vector3.right : Vector3.left);
        transform.position += direction * Time.deltaTime * _bulletSpeed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Vector3 knockBackDir = (collision.transform.position - transform.position).normalized;
            if (!_bulletFacingDown)
            {
                 _playerRb.AddForce(Vector2.up * _knockBackForce, ForceMode2D.Impulse);    
            }
            _playerRb.AddForce(knockBackDir * _knockBackForce, ForceMode2D.Impulse);
            if (PlayerStateChecker.isDashingDown)
            {
                _playerRb.velocity = Vector3.zero;
                _playerRb.AddForce(knockBackDir * _knockBackForce, ForceMode2D.Impulse);
                PlayerStateChecker.isDashingDown = false;
            }

            OnImpact();
        }
        OnImpact();
    }



    void OnImpact()
    {
        Instantiate(_onImpactAnim, gameObject.transform.position, gameObject.transform.rotation);
        Destroy(gameObject);
    }

}


//_playerController._canMove = false;
//_playerController._inAirAfterJump = true;
//_playerRb.AddForce(_knockBackDir * _knockBackForce, ForceMode2D.Impulse);


//check that the Z position isnt interfering with the knockback diraction
//Vector2 knockBackDir2D = new Vector2(collision.transform.position.x - transform.position.x, collision.transform.position.y - transform.position.y).normalized;
//Vector3 knockBackDir = new Vector3(knockBackDir2D.x, knockBackDir2D.y, 0);