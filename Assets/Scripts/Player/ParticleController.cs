using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleController : MonoBehaviour
{
    private InputGetter InputGetter;
    private PlayerStateChecker PlayerStateChecker;
    private GameManager GameManager;

    public ParticleSystem _goingInWoodParticle;
    public ParticleSystem _goingOutWoodParticle;
    public ParticleSystem _goingInGroundParticle;
    public ParticleSystem _goingOutGroundParticle;
    public ParticleSystem _smokeUpParticle;
    public ParticleSystem _smokeDownParticle;
    public ParticleSystem _shotSmokeParticle;

    public float ParticleRot = -60;
    private float _rotationZInTheGround;
    private float _rotationZOutTheGround;

    public bool _alreadyPlayedSmokeParticle;



    private void Awake()
    {
        InputGetter = GetComponent<InputGetter>();  
        PlayerStateChecker = GetComponent<PlayerStateChecker>();
        GameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();

    }
    private void Update()
    {
        if (GameManager.HasGameStarted)
        {
            InGroundParticleRotation();
        }
    }

    public void GoingOutWoodParticleSpawn()
    {
        _goingOutWoodParticle.transform.position = transform.position;

        OutTheGroundParticleRotation();

        _goingOutWoodParticle.transform.eulerAngles = new Vector3(transform.rotation.x, transform.rotation.y, _rotationZOutTheGround);

        Instantiate(_goingOutWoodParticle);
    }
    public void GoingOutGroundParticleSpawn()
    {
        _goingOutGroundParticle.transform.position = transform.position;

        OutTheGroundParticleRotation();

        _goingOutGroundParticle.transform.eulerAngles = new Vector3(transform.rotation.x, transform.rotation.y, _rotationZOutTheGround);

        Instantiate(_goingOutGroundParticle);
    }
    public void InTheGroundParticleClear()
    {
        _alreadyPlayedSmokeParticle = false;
        _goingInWoodParticle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        _goingInGroundParticle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }
    public void PlayWoodPaticleInsideGround()
    {
        _goingInGroundParticle.Stop();
        _goingInWoodParticle.Play();
        PlayerStateChecker._playWoodParticle = false;
    }
    public void PlayGroundPaticleInsideGround()
    {
        _goingInGroundParticle.Play();
        _goingInWoodParticle.Stop();
        PlayerStateChecker._playGroundParticle = false;
    }
    public void InsideGroundHandler()
    {
        if (PlayerStateChecker._playWoodParticle)
        {
            PlayWoodPaticleInsideGround();
        }
        else if (PlayerStateChecker._playGroundParticle)
        {
           PlayGroundPaticleInsideGround();
        }
    }

    public void GoingOutTheGroundHandler()
    {
        InTheGroundParticleClear();
        _smokeUpParticle.Play();
        if (PlayerStateChecker._hittingWoodSurfce)
        {
           GoingOutWoodParticleSpawn();
        }
        else if (PlayerStateChecker._hittingGroundSurfce)
        {
           GoingOutGroundParticleSpawn();
        }
    }
    void InGroundParticleRotation()
    {
        _rotationZInTheGround = PlayerStateChecker._rb.velocity.x < 0 ? -45 : PlayerStateChecker._rb.velocity.x > 0 ? 45 : 0;

        _goingInWoodParticle.transform.eulerAngles = new Vector3(transform.rotation.x, transform.rotation.y, _rotationZInTheGround);
        _goingInGroundParticle.transform.eulerAngles = new Vector3(transform.rotation.x, transform.rotation.y, _rotationZInTheGround);
        _smokeUpParticle.transform.eulerAngles = new Vector3(transform.rotation.x, transform.rotation.y, _rotationZInTheGround);
    }
    
    void OutTheGroundParticleRotation()
    {
        _rotationZOutTheGround = InputGetter._dir != 0
         ? (transform.localScale.x < 0 ? -130 : ParticleRot) : -90;
    }
    public void ShotSmokeParticleSpawn()
    {
        _shotSmokeParticle.transform.position = transform.position + new Vector3(0, 1.83f, 0);
        _shotSmokeParticle.transform.eulerAngles = new Vector3(0, 0, transform.localScale.x < 0 ? -40 : 0);
        Instantiate(_shotSmokeParticle);
    }
    public void SmokeDownParticleSpawn()
    {
        if (!_alreadyPlayedSmokeParticle)
        {
            Instantiate(_smokeDownParticle, transform.position + new Vector3(0, 1.5f, 0), _smokeDownParticle.transform.rotation);
            _alreadyPlayedSmokeParticle = true;
        }
    }

}
