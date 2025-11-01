using System.Collections;
using UnityEngine;

namespace Player
{
    public class DashFunctions : MonoBehaviour
    {
        private int _playerLayer;
        private int _groundLayerIndex;
        private int _platformLayerIndex;
        private int _camerasTriggerIndex;


        [Tooltip("Multiplier applied to velocity after a dash to reduce momentum.")]
        [SerializeField] private float _afterDashSpeed = 0.3f;

        [Tooltip("Reference to the player's visual GameObject used during dash effects.")]
        [SerializeField] private GameObject _playerGfx;

        [Tooltip("Base force applied when jumping from the ground.")]
        [SerializeField] private float _jumpForce = 30f;

        [Tooltip("Multiplier for directional dash force when jumping.")]
        [SerializeField] private float _jumpDash = 3f;

        [Tooltip("Force applied when dashing downward toward the ground.")]
        [SerializeField] private float _dashDownForce = 20f;

        [Tooltip("Maximum time to wait in the air before initiating downward dash.")]
        public float MaxTimeWait = 1.3f;

        
        private PlayerStateChecker _playerStateChecker;
        private ParticleController _particleController;
        private SoundControllerWithoutAnimator _soundControllerWithoutAnimator;
        private PlayerMovement _playerMovement;
        private InputGetter _inputGetter;

        
        private Rigidbody2D _rb;
        
        public DashFunctions(int groundLayerIndex)
        {
            _groundLayerIndex = groundLayerIndex;
        }

        private void Awake()
        {
            _playerStateChecker = GetComponent<PlayerStateChecker>();
            _particleController = GetComponent<ParticleController>();
            _inputGetter = GetComponent<InputGetter>();
            _playerMovement = GetComponent<PlayerMovement>();
            _soundControllerWithoutAnimator = GetComponent<SoundControllerWithoutAnimator>();
            _playerLayer = gameObject.layer;
            _groundLayerIndex = LayerMask.NameToLayer("Ground");
            _platformLayerIndex = LayerMask.NameToLayer("Platform");
            _camerasTriggerIndex = LayerMask.NameToLayer("Special Camera Trigger");
            _rb = GetComponent<Rigidbody2D>();
        }


        public void InTheGroundMovement()
        {
            IgnoreCollisionOtherThanGround();
            _playerMovement.ResetVelocity();
            _playerStateChecker.isDashingDown = false;
            _playerGfx.SetActive(false);
            _soundControllerWithoutAnimator.PlayRandomSoundInGround();
        }
        public void DashJumpFromGround()
        {
            _particleController.GoingOutTheGroundHandler();
            _playerGfx.SetActive(true);
            _rb.AddForce(Vector2.up * _jumpForce * _jumpDash, ForceMode2D.Impulse);
            if (_playerStateChecker._dashJumpWithDir)
            {
                DashForwardFromGround();
                _playerStateChecker._dashJumpWithDir = false;
            }
            _soundControllerWithoutAnimator.StopPlayingSounds();
            _inputGetter._pressedUp = false;
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
            _playerStateChecker.isDashingUp = false;
            _playerStateChecker._inAirAfterJump = true;
        }

        private void CollideWithEveryThing()
        {
            //go back to collide with every layer
            for (int i = 0; i < 32; i++)
            {
                Physics2D.IgnoreLayerCollision(_playerLayer, i, false);
            }

        }

        private void DashForwardFromGround()
        {
            // Apply force based on character's facing direction
            var direction = Mathf.Sign(transform.localScale.x);
            _rb.AddForce(Vector2.right * direction * _jumpForce * _jumpDash, ForceMode2D.Impulse);
        }
        public void DashTowardsGround()
        { 
            StartCoroutine(WaitInAirBeforeDashing());
        }

        private IEnumerator WaitInAirBeforeDashing()
        {
 
            FreezeMovement();
            
            //wait for half of a second before going to the ground
            yield return new WaitForSeconds(0.2f);
            
            DashDown();
        }

        
        private void FreezeMovement()
        {
            _rb.velocity = Vector2.zero;
            _playerStateChecker._freezeBeforeDash = true;
            _playerStateChecker.isDashingDown = true;
            _playerStateChecker._canMove = false;
            _rb.gravityScale = 0;
        }
        
        private void DashDown()
        {
            _particleController.SmokeDownParticleSpawn();
            _rb.gravityScale = _playerStateChecker._gravityDefault;
            _playerStateChecker._freezeBeforeDash = false;
            _playerStateChecker._canMove = true;
            _rb.AddForce(Vector2.down * _dashDownForce, ForceMode2D.Impulse);
            _rb.velocity = new Vector2(0, _rb.velocity.y);
            _inputGetter._pressedDown = false;
        }
    }
}




