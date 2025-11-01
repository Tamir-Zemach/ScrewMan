
using Player;
using UnityEngine;


public class Player_Controller : MonoBehaviour
{
    private InputGetter InputGetter;
    private PlayerStateChecker PlayerStateChecker;
    private DashFunctions DashFunctions;
    private HealthManager HealthManager;
    private Shooting Shooting;
    private PlayerMovement PlayerMovement;
    private SoundControllerWithoutAnimator SoundControllerWithoutAnimator;


    private GameManager GameManager;
    public Vector3 _checkpointPos;

    private void Awake()
    {
        InputGetter = gameObject.GetComponent<InputGetter>();
        PlayerStateChecker = gameObject.GetComponent<PlayerStateChecker>();
        DashFunctions = gameObject.GetComponent<DashFunctions>();
        HealthManager = gameObject.GetComponent<HealthManager>();
        Shooting = gameObject.GetComponent<Shooting>();
        PlayerMovement = gameObject.GetComponent<PlayerMovement>();
        SoundControllerWithoutAnimator = GetComponent<SoundControllerWithoutAnimator>();
        GameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }

    void Update()
    {
        if (GameManager._hasGameStarted)
        {
            DashManager();
            ShootingManager();
        }

    }
    private void FixedUpdate()
    {
        if (GameManager._hasGameStarted)
        {
            Movement();
            InAirMovement();
            InTheGroundMovement();
        }

        if (GameManager._gameOver)
        {
            PlayerMovement.ResetVelocity();
        }
    }

    private void DashManager()
    {
        if (InputGetter._pressedDown && PlayerStateChecker._canDashDown && ! PlayerStateChecker.isGrounded)
        {
            DashFunctions.DashTowardsGround();
        }
        if (PlayerStateChecker._isWaitingForDashInTheGround && PlayerStateChecker.isDashingDown)
        {
            DashFunctions.InTheGroundMovement();
            PlayerStateChecker._safePos = transform.position;
        }
        if (PlayerStateChecker._dashJump)
        {
            DashFunctions.DashJumpFromGround();
            PlayerStateChecker._dashJump = false;
        }
        if (PlayerStateChecker._stopMomentom)
        {
            DashFunctions.StopMomentum();
            PlayerStateChecker._stopMomentom = false;   
        }

    }

    private void ShootingManager()
    {
        if (InputGetter._pressedF)
        {
            Shooting.Shoot();
            InputGetter._pressedF = false;
        }
    }

    private void Movement()
    {
        if (GameManager._gameOver || PlayerStateChecker._isWaitingForDashInTheGround ||
            PlayerStateChecker.isDashingUp || !PlayerStateChecker._canMove)
        {
            return; // Exit early if movement isn't allowed
        }

        if (PlayerStateChecker._canMove)
        {
            PlayerMovement.GeneralMovement();
        }

        if (InputGetter._pressedJump && PlayerStateChecker.isGrounded)
        {
            PlayerMovement.Jump();
            InputGetter._pressedJump = false;
        }


    }
    private void InAirMovement()
    {
        if (PlayerStateChecker._inAirAfterJump)
        {
            PlayerMovement.InAirMovement();
        }
    }
    private void InTheGroundMovement()
    {
        if (PlayerStateChecker._inTheGroundMovement)
        {
            PlayerMovement.inTheGroundMovement();
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Start_Trigger"))
        {
            Camera_Control.instance.CameraShake(2, 0.4f);
            PlayerMovement.KnockBackFromDoor(-40, 20);
        }
        if (collision.gameObject.CompareTag("Boss_Enter_Trigger"))
        {
            PlayerMovement.ResetVelocity(true, false);
            Camera_Control.instance.CameraShake(0.5f, 4f);
        }
        if (collision.gameObject.CompareTag("Ability_Dash"))
        {
            PlayerMovement.ResetVelocity(true, false);
            Destroy(collision.gameObject);
        }
        if (collision.gameObject.CompareTag("Ability_Shoot"))
        {
            PlayerMovement.ResetVelocity(true, false);
            Destroy(collision.gameObject);
        }
        if (collision.gameObject.CompareTag("Health_Pickup"))
        {
             SoundControllerWithoutAnimator.HealthPickupSound();
             Destroy(collision.gameObject);
        }
        if (collision.gameObject.CompareTag("Respawn"))
        {
            if (PlayerStateChecker._canGetHit)
            {
                HealthManager.GetHit();
            }
        }
        if (collision.gameObject.CompareTag("Checkpoint"))
        {
            _checkpointPos = collision.gameObject.transform.position;
            Destroy(collision.gameObject);
        }
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy_Bullet"))
        {
            SoundControllerWithoutAnimator.GettingHitSound();
            if (PlayerStateChecker._canGetHit)
            {
                HealthManager.GetHit();
            }
        }
        if (collision.gameObject.CompareTag("Boss_Bullet"))
        {
            SoundControllerWithoutAnimator.GettingHitFromBossSound();
            if (PlayerStateChecker._canGetHit)
            {
                HealthManager.GetHit();
            }
        }
    }
}



