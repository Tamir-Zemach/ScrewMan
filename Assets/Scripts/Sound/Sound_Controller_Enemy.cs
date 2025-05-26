using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Sound_Controller_Enemy;

public class Sound_Controller_Enemy : MonoBehaviour
{
    public AudioSource _audioSource;
    public AudioClip[] enemySoundEffects;
    public AudioClip[] playerSoundEffects;
    public AudioClip[] enviormentSoundEffects;
    public AudioClip[] _inTheGroundSounds;

    public enum SoundTypeForEnemy { Alert, Spit, Boss_Shot, GuardEnemyDying }
    public enum SoundTypeForPlayer { Whoosh1, Big_Steam, Small_Air_Burst, Big_Air_Burst, PickUpMusic, }
    public enum SoundTypeForEnviorment { gearWheels, DrillIn, Achivement }

    public void StopPlayingClip()
    {
        if (_audioSource != null)
        {
            _audioSource.Stop();
        }
    }

    public void PlayEnemySound(SoundTypeForEnemy soundTypeForEnemy)
    {
        _audioSource.PlayOneShot(enemySoundEffects[(int)soundTypeForEnemy], GetVolumeForEnemy(soundTypeForEnemy));
    }

    public void PlayPlayerSound(SoundTypeForPlayer soundTypeForPlayer)
    {
        _audioSource.PlayOneShot(playerSoundEffects[(int)soundTypeForPlayer], GetVolumeForPlayer(soundTypeForPlayer));
    }
    public void PlayEnviormentSound(SoundTypeForEnviorment soundTypeForEnviorment)
    {
        _audioSource.PlayOneShot(enviormentSoundEffects[(int)soundTypeForEnviorment], GetVolumeForEnviorment(soundTypeForEnviorment));
    }

    private float GetVolumeForEnemy(SoundTypeForEnemy soundTypeForEnemy)
    {
        switch (soundTypeForEnemy)
        {
            case SoundTypeForEnemy.Alert:
                return 0.3f;
            case SoundTypeForEnemy.Spit:
                return 1.0f;
            case SoundTypeForEnemy.Boss_Shot:
                return 1.0f;
            case SoundTypeForEnemy.GuardEnemyDying:
                return 1.0f;
            default:
                return 1.0f;

        }
    }
    
    private float GetVolumeForPlayer(SoundTypeForPlayer soundTypeForPlayer)
    {
        switch (soundTypeForPlayer)
        {
            case SoundTypeForPlayer.Whoosh1:
                return 0.4f;
            case SoundTypeForPlayer.Big_Steam:
                return 0.1f;
            case SoundTypeForPlayer.Small_Air_Burst:
                return 0.7f;
            case SoundTypeForPlayer.Big_Air_Burst:
                return 0.4f;
            case SoundTypeForPlayer.PickUpMusic:
                return 1f;
            default:
                return 1.0f;
        }
    }
    private float GetVolumeForEnviorment(SoundTypeForEnviorment soundTypeForEnviorment)
    {
        switch (soundTypeForEnviorment)
        {
            case SoundTypeForEnviorment.gearWheels:
                return 1f;
            case SoundTypeForEnviorment.DrillIn:
                return 0.1f;
            case SoundTypeForEnviorment.Achivement:
                return 1f;
            default:
                return 1.0f;
        }
    }



    public void PlayRandomSoundInGround()
    {
        int inTheGroundSoundsIndex = Random.Range(0, _inTheGroundSounds.Length);
        float randomVolumeIndex = Random.Range(0.4f, 0.6f);
        _audioSource.PlayOneShot(_inTheGroundSounds[inTheGroundSoundsIndex], randomVolumeIndex);
    }
}
