using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundControllerWithoutAnimator : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSourceWithOutAnimations;
    public AudioClip[] _inTheGroundSounds;
    public AudioClip _shotSoundInTheAir;
    public AudioClip _waterSplesh;
    public AudioClip _metalClick;
    public AudioClip _mechanicPickup;

    private void Awake()
    {
        _audioSourceWithOutAnimations = GetComponent<AudioSource>();
    }

    public void GettingHitSound()
    {
        _audioSourceWithOutAnimations.PlayOneShot(_waterSplesh, 0.8f);
    }

    public void GettingHitFromBossSound()
    {
        _audioSourceWithOutAnimations.PlayOneShot(_metalClick, 1f);
    }

    public void ShootInTheAirSound()
    {
        _audioSourceWithOutAnimations.PlayOneShot(_shotSoundInTheAir, 0.7f);
    }

    public void HealthPickupSound()
    {
        _audioSourceWithOutAnimations.PlayOneShot(_mechanicPickup, 0.7f);
    }
    public void PlayRandomSoundInGround()
    {
        int randomIndex = Random.Range(0, _inTheGroundSounds.Length);
        _audioSourceWithOutAnimations.PlayOneShot(_inTheGroundSounds[randomIndex], 1f);
    }
    public void StopPlayingSounds()
    {
        if (_audioSourceWithOutAnimations.isPlaying)
        {
            _audioSourceWithOutAnimations.Stop();
        }
    }


}
