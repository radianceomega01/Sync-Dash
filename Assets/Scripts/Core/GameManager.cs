using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

/// <summary>
/// Central game state manager for Sync Dash (Android).
/// - Tracks score (distance + orbs)
/// - Gradually increases game speed
/// - Handles game states (Menu, Playing, Paused, GameOver)
/// - Exposes UnityEvents for UI and other systems to subscribe to
/// - Provides methods for OrbCollected and PlayerHit (called by Orb/Obstacle)
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Speed / Scoring")]
    [Tooltip("Base player forward speed (units/sec).")]
    public float baseSpeed = 6f;

    [Tooltip("Multiplier applied on top of baseSpeed (starts at 1).")]
    public float speedMultiplier = 1f;

    [Tooltip("How much the multiplier grows per second while playing.")]
    public float speedIncreasePerSecond = 0.01f;

    [Tooltip("Points awarded per orb collected.")]
    public int orbPoints = 10;

    [Tooltip("Points multiplier applied to distance -> int score.")]
    public float distancePointFactor = 1f; // e.g., every unit distance = 1 point

    [Header("Score")]
    [SerializeField] // ReadOnly is optional attribute from many editors; ignore if not present
    private float distanceAccum = 0f;
    [SerializeField]
    private int orbScore = 0;

    [Header("Gameplay")]
    [Tooltip("If true, distance accumulates even when player is not moving (helpful for debugging).")]
    public bool accumulateDistance = true;

    [Tooltip("Maximum speed multiplier (cap).")]
    public float maxSpeedMultiplier = 3f;

    [Header("Events")]
    public UnityEvent<int> OnScoreChanged; // int = total score
    public UnityEvent OnGameOver;          // triggered when player hits obstacle
    public UnityEvent OnGamePaused;
    public UnityEvent OnGameResumed;
    public UnityEvent<float> OnSpeedChanged; // current speed

    public int HighScore { get; private set; } = 0;

    // Player speed accessor
    public float CurrentPlayerSpeed => baseSpeed * speedMultiplier;

    // Optional: expose for UI debug
    public int TotalScore => Mathf.FloorToInt(distanceAccum * distancePointFactor) + orbScore;

    private PhysicsScene physicsA;
    private PhysicsScene physicsB;

    private void Awake()
    {
        // singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // keep across scenes if needed
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Initialize
        LoadHighScore();
        if (OnScoreChanged == null) OnScoreChanged = new UnityEvent<int>();
        if (OnGameOver == null) OnGameOver = new UnityEvent();
        if (OnGamePaused == null) OnGamePaused = new UnityEvent();
        if (OnGameResumed == null) OnGameResumed = new UnityEvent();
        if (OnSpeedChanged == null) OnSpeedChanged = new UnityEvent<float>();
    }

    private void Start()
    {
        // Broadcast initial values (useful if UI queries)
        OnScoreChanged.Invoke(TotalScore);
        OnSpeedChanged.Invoke(CurrentPlayerSpeed);
    }

    private void Update()
    {

        // Increase speed multiplier over time (frame-rate independent)
        speedMultiplier += speedIncreasePerSecond * Time.deltaTime;
        speedMultiplier = Mathf.Min(speedMultiplier, maxSpeedMultiplier);

        // Accumulate distance as score driver
        if (accumulateDistance)
        {
            // distanceAccum grows by the player's current forward velocity * dt
            distanceAccum += CurrentPlayerSpeed * Time.deltaTime;
        }

        // Notify subscribers about updated score & speed occasionally (every frame keeps it simple)
        OnScoreChanged.Invoke(TotalScore);
        OnSpeedChanged.Invoke(CurrentPlayerSpeed);
    }

    // private void FixedUpdate()
    // {
    //     if (physicsA.IsValid()) physicsA.Simulate(Time.fixedDeltaTime);
    //     if (physicsB.IsValid()) physicsB.Simulate(Time.fixedDeltaTime);
    // }

    #region Public control API (UI / Scene)
    /// <summary>
    /// Called by UI to start the game from the menu.
    /// </summary>
    public void StartGame()
    {
        distanceAccum = 0f;
        orbScore = 0;
        speedMultiplier = 1f;

        SceneManager.LoadScene("GamePlay", LoadSceneMode.Single);

        OnScoreChanged.Invoke(TotalScore);
        OnSpeedChanged.Invoke(CurrentPlayerSpeed);
    }

    /// <summary>
    /// Pause the gameplay (freezes time scale).
    /// </summary>
    public void PauseGame()
    {
        Time.timeScale = 0f;
        OnGamePaused.Invoke();
    }

    /// <summary>
    /// Resume from pause.
    /// </summary>
    public void ResumeGame()
    {
        Time.timeScale = 1f;
        OnGameResumed.Invoke();
    }

    /// <summary>
    /// Restart current scene / return to menu depending on design.
    /// </summary>
    public void Restart()
    {
        Time.timeScale = 1f;
        // Optionally reload the scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// Return to main menu scene (if you use separate scenes).
    /// </summary>
    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        // SceneManager.LoadScene("MainMenu");
    }

    #endregion

    #region Score / Event Callbacks (called from other gameplay scripts)

    /// <summary>
    /// Called by Orb when collected.
    /// </summary>
    /// <param name="orbId">optional id</param>
    public void OrbCollected(int orbId = -1)
    {
        orbScore += orbPoints;

        // Optional: play SFX or particle via other systems
        OnScoreChanged.Invoke(TotalScore);
    }

    /// <summary>
    /// Called when the player collides with obstacle.
    /// Triggers game over flow.
    /// </summary>
    /// <param name="obstacleId"></param>
    public void PlayerHit(int obstacleId = -1)
    {
        Time.timeScale = 1f; // ensure not paused

        // Final score handling
        int finalScore = TotalScore;
        if (finalScore > HighScore)
        {
            HighScore = finalScore;
            SaveHighScore();
        }

        // Broadcast game over for UI / analytics
        OnGameOver.Invoke();

        // Optionally, show GameOver UI or transition to GameOver scene
        // SceneManager.LoadScene("GameOver");
    }

    #endregion

    #region Persistence (High Score)
    private const string PREF_HIGH_SCORE = "SyncDash_HighScore_v1";

    private void SaveHighScore()
    {
        PlayerPrefs.SetInt(PREF_HIGH_SCORE, HighScore);
        PlayerPrefs.Save();
    }

    private void LoadHighScore()
    {
        HighScore = PlayerPrefs.GetInt(PREF_HIGH_SCORE, 0);
    }
    #endregion
}