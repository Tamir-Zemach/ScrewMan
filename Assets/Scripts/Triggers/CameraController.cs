using System.Linq;
using Cinemachine;
using UnityEngine;

namespace Triggers
{
    public class CameraController : MonoBehaviour
    {
        public static CameraController Instance;
        
        [Header("Player & Camera References")]
        [Tooltip("Reference to the player GameObject.")]
        [SerializeField] private GameObject _player;
        
        [Tooltip("Main virtual camera used when no special zones are active.")]
        [SerializeField] private CinemachineVirtualCamera _defaultVirtualCamera;
        
        [Tooltip("Array of special zone virtual cameras.")]
        [SerializeField] private CinemachineVirtualCamera[] _specialZoneCameras;
        
        [Header("Camera Behavior Settings")]
        [Tooltip("Screen offset when player is facing left.")]
        [SerializeField] private float _screenOffsetLeft = 0.4f;
        
        [Tooltip("Screen offset when player is facing right.")]
        [SerializeField] private float _screenOffsetRight = 0.6f;

        [Tooltip("Flag indicating if any special zone is currently active.")]
        public bool IsZoneActive = false;

        private PlayerStateChecker _playerStateChecker;
        private CinemachineFramingTransposer _transposer;
        private CinemachineBasicMultiChannelPerlin _perlinNoise;
        private float _shakeDuration;

        private void Awake()
        {
            Instance = this;

            if (_player == null)
            {
                _player = GameObject.Find("Player");
            }
            _playerStateChecker = _player.GetComponent<PlayerStateChecker>();
            _transposer = _defaultVirtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        }

        private void Update()
        {
            UpdateCameraDirection();
            HandleSpecialZoneCameras();
            UpdateShakeTimer();
            UpdatePerlinComponent();
        }

        private void UpdateCameraDirection()
        {
           _transposer.m_ScreenX = _playerStateChecker.IsLookingRight ? _screenOffsetLeft : _screenOffsetRight;
        }

        private void HandleSpecialZoneCameras()
        {
            IsZoneActive = false;

            for (int i = 0; i < _playerStateChecker.IsInSpecialCameraZone.Length; i++)
            {
                if (!_playerStateChecker.IsInSpecialCameraZone[i]) continue;

                ActivateSpecialCamera(i);
                IsZoneActive = true;
                break;
            }

            if (IsZoneActive) return;

            foreach (var cam in _specialZoneCameras)
            {
                cam.enabled = false;
            }

            _defaultVirtualCamera.enabled = true;
        }

        private void ActivateSpecialCamera(int index)
        {
            foreach (var cam in _specialZoneCameras)
            {
                cam.enabled = false;
            }

            _specialZoneCameras[index].enabled = true;
            _defaultVirtualCamera.enabled = false;
        }

        public void TriggerCameraShake(float intensity, float duration)
        {
            _shakeDuration = duration;
            if (_perlinNoise != null)
            {
                _perlinNoise.m_AmplitudeGain = intensity;
            }
        }

        private void UpdateShakeTimer()
        {
            if (_shakeDuration <= 0) return;

            _shakeDuration -= Time.deltaTime;

            if (_shakeDuration <= 0 && _perlinNoise != null)
            {
                _perlinNoise.m_AmplitudeGain = 0f;
            }
        }

        private void UpdatePerlinComponent()
        {
            var activeCamera = GetActiveCamera();
            if (activeCamera != null)
            {
                _perlinNoise = activeCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            }
        }

        private CinemachineVirtualCamera GetActiveCamera()
        {
            return _defaultVirtualCamera.enabled ? _defaultVirtualCamera : _specialZoneCameras.FirstOrDefault(cam => cam.enabled);
        }
    }
}