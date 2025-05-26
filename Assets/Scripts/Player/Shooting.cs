using System.Collections;
using UnityEngine;

public class Shooting : MonoBehaviour
{

    private PlayerStateChecker PlayerStateChecker;
    private ParticleController ParticleController;
    private SoundControllerWithoutAnimator SoundControllerWithoutAnimator;


    public GameObject[] _bulletPrefabs;
    public float _bulletOffsetY = 2.1f;
    public float _bulletOffsetX = 0.1f;
    [SerializeField] private float _shootingDelayTime = 0.3f;

    private void Awake()
    {
        PlayerStateChecker = GetComponent<PlayerStateChecker>(); 
        ParticleController = GetComponent<ParticleController>();  
        SoundControllerWithoutAnimator = GetComponent<SoundControllerWithoutAnimator>();
    }

    public void Shoot()
    {
        if (!PlayerStateChecker._inBossTrigger & PlayerStateChecker._canShoot && !PlayerStateChecker.isDashingUp)
        {
            StartCoroutine(ShootWithDelay());
        }
    }
    IEnumerator ShootWithDelay()
    {
        if (PlayerStateChecker.isGrounded)
        {
            SetMovementState(false);
            PlayerStateChecker._rb.velocity = new Vector2(0, PlayerStateChecker._rb.velocity.y);
        }
        else
        {
            SoundControllerWithoutAnimator.ShootInTheAirSound();
        }
        PlayerStateChecker._isShooting = true;
        yield return new WaitForSeconds(0.2f);
        ParticleController.ShotSmokeParticleSpawn();
        SetMovementState(true);
        int _randomBulletPrefabIndex = Random.Range(0, _bulletPrefabs.Length);
        Instantiate(_bulletPrefabs[_randomBulletPrefabIndex], transform.position + new Vector3(_bulletOffsetX, _bulletOffsetY, 0), _bulletPrefabs[_randomBulletPrefabIndex].transform.rotation);
        yield return new WaitForSeconds(_shootingDelayTime);
        PlayerStateChecker._isShooting = false;

    }

    void SetMovementState(bool state)
    {
        //PlayerStateChecker._canMoveAtAll = state;
        PlayerStateChecker._canMove = state;
    }

}
