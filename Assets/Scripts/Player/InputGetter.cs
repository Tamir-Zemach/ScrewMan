using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputGetter : MonoBehaviour
{
    private PlayerStateChecker PlayerStateChecker;
    private GameManager GameManager;
    public bool _pressedJump;
    public bool _pressedDown;
    public bool _pressedUp;
    public bool _pressedF;
    public float _dir;
    private void Awake()
    {
        PlayerStateChecker = GetComponent<PlayerStateChecker>();
        GameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }

    void Update()
    {
        if (GameManager.HasGameStarted)
        {
            GetInput();
        }

    }


    // regular functions
    void GetInput()
    {
        if (!PlayerStateChecker._inBossTrigger)
        {
            if (!PlayerStateChecker._inEndTrigger)
            {
                _dir = Input.GetAxis("Horizontal");
            }

            if (Input.GetButtonDown("Jump"))
            {
                if (PlayerStateChecker.isGrounded && !PlayerStateChecker._isWaitingForDashInTheGround)
                {
                    _pressedJump = true;
                }
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                if (PlayerStateChecker._canDashDown && !PlayerStateChecker._isWaitingForDashInTheGround && !PlayerStateChecker.isGrounded)
                {
                    _pressedDown = true;
                }
            }
        
            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.Space))
            {
               if (PlayerStateChecker._isWaitingForDashInTheGround)
              {
                  _pressedUp = true;
              }
            }
            if (Input.GetKeyDown(KeyCode.F) && !PlayerStateChecker._isWaitingForDashInTheGround && !PlayerStateChecker._isShooting)
            {
                 _pressedF = true;
            }
        }
    }

    }

