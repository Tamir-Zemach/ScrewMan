
using System.Xml.Serialization;
using Player;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerStateChecker : MonoBehaviour
{

    private GameManager GameManager;
    private InputGetter InputGetter;
    private ParticleController ParticleController;
    private DashFunctions DashFunctions;

    public Rigidbody2D _rb;
    public Vector2 _safePos;

    public float _startY;
    [SerializeField] private float _maxDashPosY = 3;
    [SerializeField] private float _MaxHorizontalSpeed = 10;
    [SerializeField] private float PeakHeight = 0.2f;
    [SerializeField] private float _gravityFallForce = 3;
    public float _gravityDefault;

    [Header("Can do abillities")]
    //public bool _canMoveAtAll;
    public bool _canMove;
    public bool _canDashDown;
    public bool _canShoot;
    public bool _canGetHit = true;

    [Header("States bools")]
    public bool isGrounded;
    public bool _isAtEdge;
    public bool isDashingDown;
    public bool isDashingUp;
    public bool _isWaitingForDashInTheGround;
    public bool _hitUndashableGround;

    [Header("In triggers")]
    public bool _inEndTrigger;
    public bool _inBossTrigger;
    public bool _checkPointAfterBoss;

    [Header("Looking & Shooting")]
    public bool _isShooting;
    public bool IsLookingRight = true;

    [Header("Surface Bools")]
    public bool _hittingWoodSurfce;
    public bool _playWoodParticle;
    public bool _hittingGroundSurfce;
    public bool _playGroundParticle;
    public bool _playSoundGroundDash;

    [Header("Dash & Air Bools")]
    public bool _dashJump;
    public bool _dashJumpWithDir;
    public bool _freezeBeforeDash;
    public bool _inAirAfterJump;
    public bool _insideObj;
    public bool _stopMomentom;
    public bool _inTheGroundMovement;



    int _combinedLayers;
    [SerializeField] private LayerMask _layersForCheckingGround;
    [SerializeField] private LayerMask _layersForUndashableGround;
    [SerializeField] private LayerMask _canMoveThroughLayer;

    public bool[] IsInSpecialCameraZone = new bool[5];

    /// <summary>
    [SerializeField] float _cubeOffset = 0.26f;
    [SerializeField] float _cubeAngle = 0;
    [SerializeField] float _cubeX = 0.75f;
    [SerializeField] float _cubeY = 0.45f;
    /// </summary>

    public float RaycastCheckDisForEdge = 1f;
    public float _aboveRayCheckingStartPos = 2.26f;

    public float _waitTime;


    private SpecialZone zone;
    private void Awake()
    {
        GameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        ParticleController = gameObject.GetComponent<ParticleController>();
        InputGetter = gameObject.GetComponent<InputGetter>();
        DashFunctions = gameObject.GetComponent<DashFunctions>();
        _rb = gameObject.GetComponent<Rigidbody2D>();
        _gravityDefault = _rb.gravityScale;
        _combinedLayers = _layersForCheckingGround;
    }
    void Start()
    {

        if (_checkPointAfterBoss)
        {
            _canDashDown = true;
            _canShoot = true;
        }
        _canMove = true;
    }
    private void Update()
    {
        if (GameManager._hasGameStarted)
        {
            DrawRaysForTesting();

            HandleGroundChecks();
            FlipXOnDir();
            CheckForDashableGround();
            JumpDashSpeedCheck();
            SpeedCheck();
            ParticleStateCheck();
            GravityModifairOnRegularJump();
            //SpecialCameraZoneNullCheck();   
        }
    }
    private void FixedUpdate()
    {
        if (GameManager._hasGameStarted)
        {
            CheckDashingOutTheGround();
        }
    }
    void HandleGroundChecks()
    {
        CheckGround();
        CheckEdge();
        CheckAbove();
    }
    void FlipXOnDir()
    {
        if ((InputGetter._dir < 0 && IsLookingRight) || (InputGetter._dir > 0 && !IsLookingRight))
        {
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
            IsLookingRight = !IsLookingRight; 
        }
    }
    void CheckGround()
    {
        bool LastGroundState = isGrounded;
        isGrounded = Physics2D.OverlapBox(transform.position + new Vector3(0, _cubeOffset, 0), new Vector2(_cubeX, _cubeY), _cubeAngle, _combinedLayers);
        if (isGrounded)
        {
            if (_rb.gravityScale != _gravityDefault)
            {
                //make sure the gravity stays normal
                _rb.gravityScale = _gravityDefault;
            }
        }
        if (LastGroundState = isGrounded) 
        {
            CheckDashingInToGround();
        }

    }
    void CheckDashingInToGround()
    {

        if (!_isWaitingForDashInTheGround)
        {
           _isWaitingForDashInTheGround = (!_hitUndashableGround && isGrounded && isDashingDown);
        }
        
        isDashingDown = _isWaitingForDashInTheGround ? isDashingDown : false;
        ParticleController.InsideGroundHandler();
        if (isGrounded && _inAirAfterJump)
        {
            // check if you hit the ground when you land AFTER jump dash
            if (!_inBossTrigger)
            {
                _canMove = true;
            }
            _inAirAfterJump = false;
            _stopMomentom = false;
        }
    }
    void CheckDashingOutTheGround()
    {
      
        if (_isWaitingForDashInTheGround)
        {
            if (!_isAtEdge)
            {
                _inTheGroundMovement = true;
            }
            StartTimer();
        }

    }
    private void StartTimer()
    {
        _waitTime += Time.deltaTime;
        if (_waitTime > DashFunctions.MaxTimeWait || InputGetter._pressedUp)
        {
            _inTheGroundMovement = false;
            _canMove = false;
            _dashJump = true;
            _isWaitingForDashInTheGround = false;
            isDashingUp = true;
            _waitTime = 0;
            _startY = transform.position.y;
            if (InputGetter._dir != 0)
            {
                _dashJumpWithDir = true;
            }
        }

    }
    void CheckEdge()
    {

        if (isGrounded)
        {
            //ray cast that start at small distance from the player depand on the diraction that he is facing
            float _rayCastSide = 0.7f;
            if (transform.localScale.x < 0)
            {
                _rayCastSide *= -1;
            }
            Vector2 _rayOrigin = new Vector2(transform.position.x + _rayCastSide, transform.position.y);
            _isAtEdge = !Physics2D.Raycast(_rayOrigin, new Vector2(InputGetter._dir, 0), RaycastCheckDisForEdge, _layersForCheckingGround);
            Debug.DrawLine(_rayOrigin, _rayOrigin * RaycastCheckDisForEdge, Color.red);
        }
        else
        {
            _isAtEdge = false;
        }
    }
    void CheckAbove()
    {
        if (isDashingUp)
        {
            _insideObj = Physics2D.Raycast(transform.position + new Vector3(0, _aboveRayCheckingStartPos, 0), Vector2.up, 0.1f, _canMoveThroughLayer);
        }
        if (_insideObj)
        {
            TeleportFromInsideObj();
        }


    }
    private void TeleportFromInsideObj()
    {
        //if you go up from the fround and you are inside an object - than rest your momentom and teleport to the last spot you dashed down to 
        transform.position = _safePos;
        _stopMomentom = true;
        _canMove = true;
        _rb.velocity = Vector2.zero;
        _insideObj = false;
    }
    void CheckForDashableGround()
    {
        // if you hit the ground in dash mode and the ground is not "dashable", than go to normal standing state
        _hitUndashableGround = Physics2D.OverlapBox(transform.position + new Vector3(0, _cubeOffset - 0.5f, 0), new Vector2(_cubeX, _cubeY), _cubeAngle, _layersForUndashableGround) != null;
    }
    void SpeedCheck()
    {
        //make sure you cant go to fast in general
        _rb.velocity = new Vector2(Mathf.Clamp(_rb.velocity.x, -_MaxHorizontalSpeed, _MaxHorizontalSpeed), _rb.velocity.y);
    }
    void JumpDashSpeedCheck()
    {
        //if you get a certain speed and height after jump dash - then stop the momentum
        if (isDashingUp && (transform.position.y - _startY > _maxDashPosY || _rb.velocity.y < -0.5))
        {
            _stopMomentom = true;
        }

    }
    void ParticleStateCheck()
    {
        _playWoodParticle = _hittingWoodSurfce && _isWaitingForDashInTheGround && !ParticleController._goingInWoodParticle.isPlaying; 
        _playGroundParticle =  _hittingGroundSurfce && _isWaitingForDashInTheGround && !ParticleController._goingInGroundParticle.isPlaying;
    }
    void GravityModifairOnRegularJump()
    {
        // makes the gravity stronger after normal jump
        if (_rb.velocity.y < PeakHeight && !isGrounded)
        {
            _rb.gravityScale = _gravityFallForce;
        }
    }
    public void InEndTrigger()
    {
        _canDashDown = false;
        _canShoot = false;
        _inEndTrigger = true;
        InputGetter._dir = -1f;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Boss_Enter_Trigger"))
        {
            _checkPointAfterBoss = true;
            _inBossTrigger = true;
            InputGetter._dir = 0;
        }
        if (collision.gameObject.CompareTag("Ability_Dash"))
        {
            _canDashDown = true;
        }
        if (collision.gameObject.CompareTag("Ability_Shoot"))
        {
            _canShoot = true;
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Wood_Surface"))
        {
            _hittingWoodSurfce = true;
            _hittingGroundSurfce = false;
        }

        if (collision.gameObject.CompareTag("Ground_Surface"))
        {
            _hittingWoodSurfce = false;
            _hittingGroundSurfce = true;
        }

        zone = collision.gameObject.GetComponent<SpecialZone>();
        if (zone != null)
        {
            IsInSpecialCameraZone[zone._zoneIndex] = true; // Set the corresponding zone to true
        }
        if (collision.gameObject.CompareTag("End_Trigger"))
        {
            InEndTrigger();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        zone = collision.gameObject.GetComponent<SpecialZone>();
        if (zone != null)
        {
            IsInSpecialCameraZone[zone._zoneIndex] = false;
        }
    }
    void DrawRaysForTesting()
{
    float _rayCastSide1 = 0.7f;
    if (transform.localScale.x < 0)
    {
        _rayCastSide1 *= -1;
    }
    float dir = Input.GetAxis("Horizontal");
    Debug.DrawRay(new Vector2(transform.position.x + _rayCastSide1, transform.position.y), new Vector2(dir, 0), Color.black);
    Debug.DrawRay(new Vector2(transform.position.x, transform.position.y + _aboveRayCheckingStartPos), Vector2.up, Color.yellow);

}
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;

        Gizmos.DrawWireCube(transform.position + new Vector3(0, _cubeOffset, 0), new Vector2(_cubeX, _cubeY));
    }
}


