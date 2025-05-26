using Cinemachine;
using UnityEngine;

public class Camera_Control : MonoBehaviour
{
    public static Camera_Control instance;
    public GameObject _player;
    private PlayerStateChecker PlayerStateChecker;
    public CinemachineFramingTransposer _transposer;
    private CinemachineVirtualCamera _virtualCamera;
    public CinemachineBasicMultiChannelPerlin _cinemachineBasicMultiChannelPerlin;
    public CinemachineVirtualCamera[] _virtualSpecialCameras; // Array for multiple special cameras
    public float _screenOffsetLeft = 0.4f;
    public float _screenOffsetRight = 0.6f;
    private float _cameraShakeTime;
    public bool anyZoneActive = false;


    void Start()
    {
        instance = this;
        _player = GameObject.Find("Player");
        PlayerStateChecker = _player.GetComponent<PlayerStateChecker>();
        _virtualCamera = GetComponent<CinemachineVirtualCamera>(); 
        _transposer = _virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        //for (int i = 0; i < _virtualSpecialCameras.Length; i++)
        //{
        //    _virtualSpecialCameras[i].enabled = false;
        //}
    }

    void Update()
    {
        FlipCamera();
        SpecialZoneCamera();
        ShakeTimer();
        cinemachineBasicMultiChannelPerlinComponentGetter();
    }

    void FlipCamera()
    {
        if (!PlayerStateChecker._lookingRight)
        {
            _transposer.m_ScreenX = _screenOffsetRight;
        }
        else
        {
            _transposer.m_ScreenX = _screenOffsetLeft;
        }
    }
    void SpecialZoneCamera()
    {
        anyZoneActive = false;
        for (int i = 0; i < PlayerStateChecker._inSpacialCameraZones.Length; i++)
        {
            if (PlayerStateChecker._inSpacialCameraZones[i])
            {
                ActivateCamera(i);
                anyZoneActive = true;
                break; 
            }
        }

        if (!anyZoneActive)
        {
            for (int i = 0; i < _virtualSpecialCameras.Length; i++)
            {
                _virtualSpecialCameras[i].enabled = false;
            }
            _virtualCamera.enabled = true; 
        }
    }
    void ActivateCamera(int index)
    {
        _virtualSpecialCameras[index].enabled = true;
    }

    public void CameraShake(float intensity, float duration)
    {
        _cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = intensity; // the intensity of the shake
        _cameraShakeTime = duration; // how much time is the shake happening
    }
    private void cinemachineBasicMultiChannelPerlinComponentGetter()
    {
        // no metter what camera is enabled - get the componnet of the camera shake
        if (_virtualCamera.enabled)
        {
            _cinemachineBasicMultiChannelPerlin = _virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        }
        else
        {
            for (int i = 0; i < _virtualSpecialCameras.Length; i++)
            {
                if (_virtualSpecialCameras[i].enabled)
                {
                    _cinemachineBasicMultiChannelPerlin = _virtualSpecialCameras[i].GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
                }
            }
        }


    }
    private void ShakeTimer()
    {
        
        if (_cameraShakeTime > 0)
        {
            _cameraShakeTime -= Time.deltaTime;
            if (_cameraShakeTime <= 0)
            {
                //Timer Over 
                _cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = 0f;
            }
        }
    }




}
