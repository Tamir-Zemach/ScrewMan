using System.Collections;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class DashFunctions : MonoBehaviour
{

    int _playerLayer;
    int _groundLayerIndex;
    int _platformLayerIndex;
    int _camerasTriggerIndex;

    [SerializeField] private float _afterDashSpeed = 0.3f;

    private Rigidbody2D _rb;
    [SerializeField] private GameObject _playerGfx;

    public float _maxTimeWait = 1.3f;
    [SerializeField] private float _jumpForce = 30f;
    [SerializeField] private float _jumpDash = 3f;
    [SerializeField] private float _dashDownForce = 20f;

    private PlayerStateChecker PlayerStateChecker;
    private ParticleController ParticleController;
    private SoundControllerWithoutAnimator SoundControllerWithoutAnimator;
    private PlayerMovement PlayerMovement;
    private InputGetter InputGetter;
    private void Awake()
    {
        PlayerStateChecker = GetComponent<PlayerStateChecker>();
        ParticleController = GetComponent<ParticleController>();
        InputGetter = GetComponent<InputGetter>();
        PlayerMovement = GetComponent<PlayerMovement>();
        SoundControllerWithoutAnimator = GetComponent<SoundControllerWithoutAnimator>();
        _playerLayer = gameObject.layer;
        _groundLayerIndex = LayerMask.NameToLayer("Ground");
        _platformLayerIndex = LayerMask.NameToLayer("Platform");
        _camerasTriggerIndex = LayerMask.NameToLayer("Special Camera Trigger");
        _rb = GetComponent<Rigidbody2D>();
    }


    public void InTheGround()
    {
        
        IgnoreCollisionOtherThanGround();
        PlayerMovement.ResetVelocity();
        PlayerStateChecker.isDashingDown = false;
        _playerGfx.SetActive(false);
        SoundControllerWithoutAnimator.PlayRandomSoundInGround();
    }
    public void DashJumpFromGround()
    {
        ParticleController.GoingOutTheGroundHandler();
        _playerGfx.SetActive(true);
        _rb.AddForce(Vector2.up * _jumpForce * _jumpDash, ForceMode2D.Impulse);
        if (PlayerStateChecker._dashJumpWithDir)
        {
            DashForwardFromGround();
            PlayerStateChecker._dashJumpWithDir = false;
        }
        SoundControllerWithoutAnimator.StopPlayingSounds();
        InputGetter._pressedUp = false;
        CollideWithEveryThing();
    }
    private void IgnoreCollisionOtherThanGround()
    {
        //checks all of the layers in the scene and if the layer is not ground or bojects - than ignore it 
        for (int i = 0; i < 32; i++)
        {
            if (i != _groundLayerIndex && i != _platformLayerIndex && i != _camerasTriggerIndex)
            {
                Physics2D.IgnoreLayerCollision(_playerLayer, i, true);
            }
            else
            {
                Physics2D.IgnoreLayerCollision(_playerLayer, i, false);
            }

        }

    }


    public void StopMomentum()
    {
        _rb.velocity = new Vector2(_rb.velocity.x * _afterDashSpeed, _rb.velocity.y* _afterDashSpeed);
         PlayerStateChecker.isDashingUp = false;
         PlayerStateChecker._inAirAfterJump = true;
    }
    void CollideWithEveryThing()
    {
        //go back to collide with every layer
        for (int i = 0; i < 32; i++)
        {
            Physics2D.IgnoreLayerCollision(_playerLayer, i, false);
        }

    }


    void DashForwardFromGround()
    {
        // Apply force based on character's facing direction
        float direction = Mathf.Sign(transform.localScale.x);
        _rb.AddForce(Vector2.right * direction * _jumpForce * _jumpDash, ForceMode2D.Impulse);
    }
    public void DashTowardsGround()
    { 
       StartCoroutine(WaitInAirBeforeDashing());
    }

    IEnumerator WaitInAirBeforeDashing()
    {
        // freeze your movement before dash
        _rb.velocity = Vector2.zero;
        PlayerStateChecker._freezeBeforeDash = true;
        PlayerStateChecker.isDashingDown = true;
        PlayerStateChecker._canMove = false;
        _rb.gravityScale = 0;
        //wait for half of a second before going to the ground
        yield return new WaitForSeconds(0.2f);
        //gives the force towrds to ground
        ParticleController.SmokeDownParticleSpawn();
        _rb.gravityScale = PlayerStateChecker._gravityDefault;
        PlayerStateChecker._freezeBeforeDash = false;
        PlayerStateChecker._canMove = true;
        _rb.AddForce(Vector2.down * _dashDownForce, ForceMode2D.Impulse);
        _rb.velocity = new Vector2(0, _rb.velocity.y);
        InputGetter._pressedDown = false;
    }


}




