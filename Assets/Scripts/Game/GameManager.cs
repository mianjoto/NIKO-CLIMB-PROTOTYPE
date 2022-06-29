using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;
using System.Collections;

public class GameManager : MonoBehaviour
{
    #region GAME
    Camera mainCamera;
    public GameObject Player;
    string playerName = "Timothy";
    #endregion
    
    #region KEYBINDS
    public KeyCode interactKey = KeyCode.E;
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode runKey = KeyCode.LeftShift;
    public KeyCode blockKey = KeyCode.Mouse1;
    public KeyCode pauseKey = KeyCode.Escape;
    #endregion

    #region SCRIPTS
    PauseMenu _pauseMenu;
    LevelManager _lm;
    CameraMovement _cm;
    PlayerHealthManager _playerHealthManager;
    #endregion

    #region DAMAGE NUMBERS
    public float fireTickDamage = 5f;
    public float punchDamage = 60f;
    public float enemyBulletDamage = 20f;
    public float maxPlayerHealth = 100;
    #endregion

    #region FIRE
    [SerializeField] GameObject _firePrefab;
    GameObject _fireInstance;
    float _fireStartingYOffset = -40;
    public float fireClimbSpeed = 100f;
    #endregion

    #region LEVEL 
    int startingLevel = 1;
    public float floorHeight = 15f;   
    public int currentLevel;
    #endregion

    #region UI
    [SerializeField] private Text ScoreUI;
    public int numberOfShields = 3;
    #endregion
    
    #region DEATH
    public bool playerIsDead;
    Vector2 _playerPosOnDeath;
    float musicFadeDuration = 1f;
    [SerializeField] GameObject _playerGhostPrefab;
    float fireFadedAlpha = 0.15f;
    float fireFadeDuration = 1;
    float newCameraOrthoSize = 5f;
    float cameraZoomDuration = 1f;
    float cameraTranslateDuration = 1f;
    bool playerWantsMusicOn;
    #endregion

    void Awake()
    {
        Application.targetFrameRate = 60;
        _pauseMenu = FindObjectOfType<PauseMenu>();
        _lm = FindObjectOfType<LevelManager>();
        _playerHealthManager = Player.GetComponent<PlayerHealthManager>();
        _cm = FindObjectOfType<CameraMovement>();
        mainCamera = Camera.main;

        Player.SetActive(true);
        Player.name = playerName;

        ScoreUI = GameObject.FindGameObjectWithTag("Score").GetComponent<Text>();


        // Initialize values
        currentLevel = startingLevel;
    }

    void Start()
    {
        ResetTweens();
        ScoreUI.text = currentLevel.ToString();
        _playerHealthManager.OnPlayerDeath += OnPlayerDeath_EndGame;
        Play();
    }

    private void ResetTweens() {
        DOTween.RestartAll(true);
    }

    public void Play()
    {  
        // Initialize audio
        if (AudioManager.Instance.isDucked)
            AudioManager.Instance.ReverseDuckAudio(musicFadeDuration);

        // Initialize player
        InitializePlayer();
        playerIsDead = false;
        PauseMenu.isPaused = false;

        // Initialize fire
        _fireInstance = Instantiate(_firePrefab, new Vector2(0, 
        _fireStartingYOffset), Quaternion.identity);

        // Build level
        currentLevel = startingLevel;
        _lm.GenerateFirstLevel(currentLevel);
    }

    void InitializePlayer()
    {
        // Enable all player components
        Player.GetComponent<PlayerMovement>().enabled = true;
        Player.GetComponent<PlayerCombat>().enabled = true;
        Player.GetComponent<Animator>().enabled = true;
    }

    private void OnPlayerDeath_EndGame(object sender, EventArgs e) {
        // Unsubscribe from the event
        _playerHealthManager.OnPlayerDeath -= OnPlayerDeath_EndGame;
        _playerPosOnDeath = Player.transform.position;
        playerIsDead = true;

        // Fade the music
        AudioManager.Instance.DuckAudio(duration: musicFadeDuration);

        // Fade the fire
        _fireInstance.GetComponent<SpriteRenderer>().DOFade(0.3f, fireFadeDuration);

        // Disable player from moving and stop animations
        Player.transform.position = _playerPosOnDeath;
        Player.GetComponent<PlayerMovement>().enabled = false;
        Player.GetComponent<PlayerCombat>().enabled = false;
        Player.GetComponent<Animator>().enabled = false;
        Destroy(_playerHealthManager.GetComponent<HealthManager>().Instance);

        // Zoom and move camera to the player
        mainCamera.DOOrthoSize(newCameraOrthoSize, cameraZoomDuration);
        mainCamera.transform.DOMove(Player.transform.position, cameraTranslateDuration);

        // Start death animation
        StartCoroutine(deathAnimation());
    }

    IEnumerator deathAnimation()
    {
        yield return new WaitForSeconds(2);
        
        // Destroy health bar and hide player
        Player.SetActive(false);
        

        // Spawn a ghost where the player was
        Instantiate(_playerGhostPrefab, _playerPosOnDeath, Quaternion.identity);

        // Display Game Over menu after 1 second
        yield return new WaitForSeconds(2);
        _pauseMenu.GameOver();
    }

    
    void Update()
    {
    }

    public void AscendFloor()
    {
        currentLevel++;
        _lm.DestroyPreviousLevel();
        _lm.TeleportPlayer();
        _lm.GenerateNewLevel(currentLevel);
        IncreaseScore();
    }

    private void IncreaseScore() {
        ScoreUI.text = currentLevel.ToString();
    }



}
