
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject[] _checkpoints;
    public GameObject _player;
    public Animator _playerAnimator;
    public Player_Controller _playercontroller;
    public UIHeartsToggle UIHeartsToggle;
    public PlayerStateChecker PlayerStateChecker; 
    public HealthManager HealthManager;
    public PlayerMovement playerMovement;
    public bool _hasGameStarted;
    public bool _gameOver = false;

    [SerializeField] private Button _startGameButton;
    [SerializeField] private Button _continueButton;
    [SerializeField] private GameObject[] _gameObjectToUnactive;
    [SerializeField] private GameObject[] _gameObjectToActiveAfterStartButtonIsClicked;
    [SerializeField] private GameObject[] _gameObjectToAnActiveAfterStartButtonIsClicked;
    [SerializeField] private GameObject _startmenuUI;
    [SerializeField] private GameObject _inGameUI;
    [SerializeField] private GameObject _gameOverMenuUI;
    [SerializeField] private GameObject[] _heartsUI;
    [SerializeField] private GameObject _canvasForAchivement;
    public AudioClip _gameOverSound;


    //bool for making sure function playing only once
    private bool _showedMenuOnce;
    public bool _checkGameOverOnce = false;
    public bool _checkCountineOnce = false;
    public bool _checkStartOnce = false;
    private bool _checkCoroutineForUiHeartsOnce = false;
    /// ////////////////////////////////////////////////////

    public float _delayAfterClickForStartButton;

    private AudioSource _audioSource;
    [SerializeField] private AudioSource _startMenuAudioSource;
    public float _fadeInDuration = 5.0f; // Duration of the fade in seconds
    public float _fadeOutDuration = 2f; 
    public float _maxVolumeForFadeIn = 0.3f;
    public float _maxVolumeForFadeOut = 0.6f;

    // Start is called before the first frame update
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        GameStartUp();
        if (PlayerStateChecker._checkPointAfterBoss)
        {
            _hasGameStarted = true;
        }
    }

    void GameStartUp()
    {
        _startmenuUI.SetActive(true);
        for (int i = 0; i < _gameObjectToUnactive.Length; i++)
        {
            _gameObjectToUnactive[i].SetActive(false);
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (!_checkStartOnce)
        {
            _startGameButton.onClick.AddListener(StartGame);
            _checkStartOnce = true;
        }

        if (!_checkCountineOnce)
        {
            _continueButton.onClick.AddListener(CountinueGame);
            _checkCountineOnce = true;
        }
        if (!_checkGameOverOnce)
        {
            GameOver();
        }
        //AdminForTesting();
        UpdateHeartsUI();
        Achivement();

    }

    void UpdateHeartsUI()
    {

        for (int i = 0; i < _heartsUI.Length; i++)
        {
            Heart _heartScript = _heartsUI[i].gameObject.GetComponent<Heart>();
            if (i < HealthManager._lifeindex)
            {
                if (UIHeartsToggle._finishedAnimation)
                {
                    _heartsUI[i].SetActive(true);
                }
                _heartScript._isActiveHeart = true;

            }
            else 
            {
                _heartScript._isActiveHeart = false;
            }
        }
    }
    //void AdminForTesting()
    //{
    //    for (int i = 0; i < _checkpoints.Length; i++)
    //    {
    //        if (Input.GetKeyDown((KeyCode)((int)KeyCode.Alpha1 + i)))
    //        {
    //            _player.transform.position = _checkpoints[i].transform.position;
    //            PlayerStateChecker._canDashDown = true;
    //            PlayerStateChecker._canShoot = true;
    //        }
    //    }

    //    if (Input.GetKeyDown(KeyCode.Alpha9))
    //    {
    //        HealthManager._lifeindex = 1;
    //    }
    //}

    void StartGame()
    {
        StartCoroutine(FadeOut(_startMenuAudioSource, _fadeOutDuration));
        _hasGameStarted = true;
        Invoke("OnStartButtonClick", _delayAfterClickForStartButton);
    }

    void OnStartButtonClick()
    {

        for (int i = 0; i < _gameObjectToActiveAfterStartButtonIsClicked.Length; i++)
        {
            _gameObjectToActiveAfterStartButtonIsClicked[i].SetActive(true);
        }
        for (int i = 0; i < _gameObjectToAnActiveAfterStartButtonIsClicked.Length; i++)
        {
            _gameObjectToAnActiveAfterStartButtonIsClicked[i].SetActive(false);
        }
        StartCoroutine(FadeIn(_audioSource, _fadeInDuration));
    }

    void GameOver()
    {

        if (HealthManager._lifeindex<= 0)
        {
            _audioSource.PlayOneShot(_gameOverSound, 0.8f);
            _checkCoroutineForUiHeartsOnce = false;
            _checkGameOverOnce = true;
            PlayerStateChecker._canGetHit = false;
            _playerAnimator.SetBool("GameOver", true);
            _gameOver = true;
            _hasGameStarted = false;
            if (!_showedMenuOnce)
            {
                StartCoroutine(GameOverScreenDelay());
            }
        }
    }
    IEnumerator GameOverScreenDelay()
    {
        
        yield return new WaitForSeconds(1);
        _gameOverMenuUI.SetActive(true);
        _inGameUI.SetActive(false);
        _showedMenuOnce = true;
        UIHeartsToggle._finishedAnimation = false;
    }
    void CountinueGame()
    {
        Invoke("OnCountinueButtonClick", _delayAfterClickForStartButton);
    }

    void OnCountinueButtonClick()
    {
        if (!PlayerStateChecker._checkPointAfterBoss)
        {
            HealthManager._lifeindex = 5;
            _showedMenuOnce = false;
            _gameOverMenuUI.SetActive(false);
            _playerAnimator.SetBool("GameOver", false);
            _inGameUI.SetActive(true);
            _player.transform.position = _playercontroller._checkpointPos;
            playerMovement._rb.velocity = Vector2.zero;

            if (!_checkCoroutineForUiHeartsOnce)
            {
                StartCoroutine(WaitUntilPlayerCanMove());
                StartCoroutine(UIHeartsToggle.TurnOnHealth());
                _checkCoroutineForUiHeartsOnce = true;
            }   
        }
        else
        {
            SceneManager.LoadScene("Boss");
        }



    }
    IEnumerator WaitUntilPlayerCanMove()
    {
       
        yield return new WaitForSeconds(1.5f);

        _gameOver = false;
        _hasGameStarted = true;
        PlayerStateChecker._canGetHit = true;
        _checkGameOverOnce = false;
        _checkCountineOnce = false;
    }

    IEnumerator FadeIn(AudioSource source, float duration)
    {
        float currentTime = 0f;
        source.volume = 0f;
        source.Play();

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            source.volume = Mathf.Lerp(0f, _maxVolumeForFadeIn, currentTime / duration);
            yield return null;
        }
    }
    IEnumerator FadeOut(AudioSource source, float duration)
    {
        float currentTime = 0f;
        source.volume = 0f;

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            source.volume = Mathf.Lerp(_maxVolumeForFadeOut, 0, currentTime / duration);
            yield return null;
        }
    }
    void Achivement()
    {
        if (HealthManager._lifeindex == 10)
        {
            _canvasForAchivement.SetActive(true);
        }
    }
}

