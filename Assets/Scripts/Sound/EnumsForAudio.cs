using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnumsForAudio : MonoBehaviour
{

    public enum SoundTypeForEnemy 
    {  
        Alert,
        Spit, 
        Boss_Shot,
        GuardEnemyDying
    }
    public enum SoundTypeForPlayer 
    { 
        Whoosh1,
        Big_Steam,
        Small_Air_Burst, 
        Big_Air_Burst,
        PickUpMusic,
        waterSplesh,
        metalClick, 
        mechanicPickup,
        shotSoundInTheAir 
    }
    public enum SoundTypeForEnviorment 
    { 
        gearWheels, 
        DrillIn, 
        Achivement 
    }

}
