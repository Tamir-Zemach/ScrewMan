using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    private Animator _animator;
    private PlayerStateChecker playerStateChecker;
    private InputGetter InputGetter;
    private GameManager GameManager;

    private bool _fallingForAnim;
    [SerializeField] private float _minVelosityForFallingAnim = -1f;
    [SerializeField] private float _idleAnimSpeed = 1.6f;

    private void Awake()
    {
        GameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        playerStateChecker = GetComponent<PlayerStateChecker>();
        InputGetter = GetComponent<InputGetter>();
        _animator = GetComponentInChildren<Animator>();
    }
    private void Update()
    {

        if (GameManager._hasGameStarted)
        {
            AnimatorParameterSetter();
        }
    }
    void AnimatorParameterSetter()
    {
        _fallingForAnim = !playerStateChecker.isGrounded && playerStateChecker._rb.velocity.y < _minVelosityForFallingAnim;
        _animator.SetBool("isShooting", playerStateChecker._isShooting);
        _animator.SetBool("IsGrounded", playerStateChecker.isGrounded);
        _animator.SetBool("Falling", _fallingForAnim);
        _animator.SetFloat("Speed", Mathf.Abs(InputGetter._dir));
        _animator.SetFloat("IdleAnimSpeed", _idleAnimSpeed);
        _animator.SetBool("IsDashingDown", playerStateChecker.isDashingDown);
        _animator.SetBool("IsDashingUp", playerStateChecker.isDashingUp);
        _animator.SetBool("IsWaitingForDashInTheGround", playerStateChecker._isWaitingForDashInTheGround);
        _animator.SetBool("CanWalkAfterShot", playerStateChecker._canMove);
        _animator.SetBool("PlayGettingSeeingBossAnim", playerStateChecker._inBossTrigger);


    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ability_Dash"))
        {
            _animator.SetTrigger("PlayGettingDashAnim");
        }
        if (collision.gameObject.CompareTag("Ability_Shoot"))
        {
            _animator.SetTrigger("PlayGettingShotAnim");
        }
    }

}
