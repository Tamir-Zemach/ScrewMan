using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private InputGetter InputGetter;
    private PlayerStateChecker PlayerStateChecker;
    public Rigidbody2D _rb;

    public float _speed = 6.5f;
    public float _inGroundSpeed = 0.7f;
    public float _InAirSpeed = 120f;
    public float jumpForce = 30f;



    private void Awake()
    {
        InputGetter = GetComponent<InputGetter>();
        PlayerStateChecker = GetComponent<PlayerStateChecker>();
        _rb = gameObject.GetComponent<Rigidbody2D>();
    }

    public void GeneralMovement()
    {
        _rb.velocity = new Vector2(InputGetter._dir * _speed, _rb.velocity.y);

    }

    public void InAirMovement()
    {
        _rb.AddForce(new Vector2(InputGetter._dir * _InAirSpeed, 0));
    }

    public void inTheGroundMovement()
    {
        _rb.velocity = new Vector2(InputGetter._dir * _speed * _inGroundSpeed, 0);
        if (PlayerStateChecker._isAtEdge) 
        {
            ResetVelocity();    
        }

    }
    public void ResetVelocity(bool resetX = true, bool resetY = true)
    {
        _rb.velocity = new Vector2(
            resetX ? 0 : _rb.velocity.x,
            resetY ? 0 : _rb.velocity.y
        );
    }
    public void Jump()
    {
        _rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        InputGetter._pressedJump = false;
    }

    public void KnockBackFromDoor(float x, float y)
    {
        _rb.AddForce(new Vector2(x, y), ForceMode2D.Impulse);
    }

    private readonly Dictionary<string, float> triggerDelays = new Dictionary<string, float>
    {
        { "Start_Trigger", 0.7f },
        { "Boss_Enter_Trigger", 4.5f },
        { "Ability_Dash", 1.5f },
        { "Ability_Shoot", 1.5f }
    };
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (triggerDelays.TryGetValue(collision.gameObject.tag, out float delay))
        {
            StartCoroutine(CantMoveAfterTriggers(delay));
            if (collision.gameObject.CompareTag("Boss_Enter_Trigger"))
            {
                StartCoroutine(CantMoveAfterBossTrigger(delay));
            }
        }
        
    }
    IEnumerator CantMoveAfterTriggers(float time)
    {
        PlayerStateChecker._canMove = false;
        yield return new WaitForSeconds(time);
        PlayerStateChecker._canMove = true;
    }
    IEnumerator CantMoveAfterBossTrigger(float time)
    {
        yield return new WaitForSeconds(time);
        PlayerStateChecker._inBossTrigger = false;
    }




}

