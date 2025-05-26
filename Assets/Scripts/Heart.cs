using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heart : MonoBehaviour
{
    public bool _playedAddedLifeAnim;
    public bool _isActiveHeart;
    private Player_Controller _playerController;
    private Animation _animation;
    public AnimationClip _addLifeAnim;
    public AnimationClip _getHurtAnim;
    public GameObject _heartPivot;
    public Vector3 _heartPivotStartPos;
    Quaternion _heartPivotStartRot;
    public Vector3 _screwStartPos;
    public GameObject _screw;
    // Start is called before the first frame update
    void Start()
    {
        _playerController = GameObject.Find("Player").GetComponent<Player_Controller>();
        _animation = gameObject.GetComponent<Animation>();
        _animation.AddClip(_getHurtAnim, _getHurtAnim.name);
        _animation.AddClip(_addLifeAnim, _addLifeAnim.name);
        _screwStartPos = _screw.transform.position;
        _heartPivotStartPos = _heartPivot.transform.position;
        _heartPivotStartRot = _heartPivot.transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        HandleAnimations();
    }

    void HandleAnimations()
    {
        if (_isActiveHeart && !_playedAddedLifeAnim)
        {
            PlayAnimation(_addLifeAnim.name);
            _playedAddedLifeAnim = true;
        }
        
        if (!_isActiveHeart && _playedAddedLifeAnim)
        {
            PlayAnimation(_getHurtAnim.name);
        }

    }

    void PlayAnimation(string animationName)
    {
        _animation.Play(animationName);
    }

    public void TurnOffAfterANIM()
    {
        gameObject.SetActive(false);
    }

}

