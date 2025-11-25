using System;
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

    public enum GameState { Menu, Playing, Paused, GameOver }

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

    // Internal state
    public GameState CurrentState { get; private set; } = GameState.Menu;
    public int HighScore { get; private set; } = 0;

    // Player speed accessor
    public float CurrentPlayerSpeed => baseSpeed * speedMultiplier;

    // Optional: expose for UI debug
    public int TotalScore => Mathf.FloorToInt(distanceAccum * distancePointFactor) + orbScore;

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
        // Start at Menu. UI should call StartGame() to begin.
        CurrentState = GameState.Menu;
        // Broadcast initial values (useful if UI queries)
        OnScoreChanged.Invoke(TotalScore);
        OnSpeedChanged.Invoke(CurrentPlayerSpeed);
    }

    private void Update()
    {
        if (CurrentState != GameState.Playing) return;

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

    #region Public control API (UI / Scene)
    /// <summary>
    /// Called by UI to start the game from the menu.
    /// </summary>
    public void StartGame()
    {
        // Reset state & scores
        distanceAccum = 0f;
        orbScore = 0;
        speedMultiplier = 1f;
        CurrentState = GameState.Playing;

        // If your scenes are separated, load the Game scene here (optional)
        SceneManager.LoadScene("GamePlayPlayer", LoadSceneMode.Additive);
        SceneManager.LoadScene("GamePlayClone", LoadSceneMode.Additive);

        OnScoreChanged.Invoke(TotalScore);
        OnSpeedChanged.Invoke(CurrentPlayerSpeed);
    }

    public void OnExitGame()
    {
        Application.Quit();
    }

    /// <summary>
    /// Pause the gameplay (freezes time scale).
    /// </summary>
    public void PauseGame()
    {
        if (CurrentState != GameState.Playing) return;
        CurrentState = GameState.Paused;
        Time.timeScale = 0f;
        OnGamePaused.Invoke();
    }

    /// <summary>
    /// Resume from pause.
    /// </summary>
    public void ResumeGame()
    {
        if (CurrentState != GameState.Paused) return;
        CurrentState = GameState.Playing;
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
        CurrentState = GameState.Menu;
    }

    #endregion

    #region Score / Event Callbacks (called from other gameplay scripts)

    /// <summary>
    /// Called by Orb when collected.
    /// </summary>
    /// <param name="orbId">optional id</param>
    public void OrbCollected(int orbId = -1)
    {
        if (CurrentState != GameState.Playing) return;

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
        if (CurrentState != GameState.Playing) return;

        CurrentState = GameState.GameOver;
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