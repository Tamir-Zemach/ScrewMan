using System.Collections;
using UnityEngine;

public class HealthManager : MonoBehaviour
{
    public int _lifeindex = 5;
    private PlayerStateChecker PlayerStateChecker;
    public float _invisabiltyTimeDelay = 2;
    public float _afterKnockBackTime = 0.5f;
    public int _flashCount = 4;
    public float _hitColorTimeDelay = 0.2f;
    [SerializeField] private SpriteRenderer[] _playerSpriteRenderer;

    private void Awake()
    {
        PlayerStateChecker = GetComponent<PlayerStateChecker>();
        _playerSpriteRenderer = GetComponentsInChildren<SpriteRenderer>();
    }
    public void GetHit()
    {
        StartCoroutine(CantMoveAfterKnockBack());
        //Camera_Control.instance.CameraShake(_cameraShakeIntesity, _cameraShakeTime);
        StartCoroutine(InvisabilityAfterHit());
        _lifeindex--;
        StartCoroutine(ColorChangeWhenGetHit());
    }
    IEnumerator CantMoveAfterKnockBack()
    {
        //PlayerStateChecker._canMoveAtAll = false;
        PlayerStateChecker._canMove = false;
        yield return new WaitForSeconds(_afterKnockBackTime);
        //PlayerStateChecker._canMoveAtAll = true;
        PlayerStateChecker._canMove = true;
    }
    IEnumerator InvisabilityAfterHit()
    {
        PlayerStateChecker._canGetHit = false;
        yield return new WaitForSeconds(_invisabiltyTimeDelay);
        PlayerStateChecker._canGetHit = true;
    }
    IEnumerator ColorChangeWhenGetHit()
    {
        if (_lifeindex <= 1) yield break; // Exit early if life index is too low

        for (int i = 0; i < _flashCount; i++)
        {
            SetPlayerColor(new Color(1, 0.5f, 0.5f, 1)); // Red color
            yield return new WaitForSeconds(_hitColorTimeDelay);

            SetPlayerColor(Color.white);
            yield return new WaitForSeconds(_hitColorTimeDelay);
        }
    }

    void SetPlayerColor(Color color)
    {
        foreach (var _playerPart in _playerSpriteRenderer)
        {
            _playerPart.color = color;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Health_Pickup"))
        {
            _lifeindex++;
        }
    }

}
