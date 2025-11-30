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

    [Header("Speed Increase Settings")]
    [Tooltip("Increase speed every X seconds.")]
    public float speedIncreaseInterval = 10f;

    [Tooltip("Amount added to speedMultiplier every interval.")]
    public float speedIncreaseAmount = 0.2f;

    [Tooltip("Maximum speed multiplier (cap).")]
    public float maxSpeedMultiplier = 2f;

    [Tooltip("Points awarded per orb collected.")]
    public int orbPoints = 10;

    [Header("Score")]
    [SerializeField]
    private float distanceAccum = 0f;
    [SerializeField]
    private int orbScore = 0;

    public event Action OnGameOver;          // triggered when player hits obstacle
    public event Action<float> OnSpeedChanged; // current speed

    public int HighScore { get; private set; } = 0;

    // Player speed accessor
    public float CurrentPlayerSpeed => baseSpeed * speedMultiplier;

    // Optional: expose for UI debug
    public int TotalScore => Mathf.FloorToInt(distanceAccum) + orbScore;
    private bool isGameOver = false;
    private Coroutine speedRoutine;

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
    }

    void Start()
    {
        Application.targetFrameRate = 60;
        speedRoutine = StartCoroutine(SpeedIncreaseRoutine());
    }

    private void Update()
    {
        // distanceAccum grows by the player's current forward velocity * dt
        if (!isGameOver)
        {
            distanceAccum += CurrentPlayerSpeed * Time.deltaTime;
        }

        OnSpeedChanged?.Invoke(CurrentPlayerSpeed);
    }

    #region Public control API (UI / Scene)
    /// <summary>
    /// Called by UI to start the game from the menu.
    /// </summary>
    public void StartGame()
    {
        ResetParams();

        SceneManager.LoadScene("GamePlay", LoadSceneMode.Single);
    }

    /// <summary>
    /// Restart current scene / return to menu depending on design.
    /// </summary>
    public void Restart()
    {
        ResetParams();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// Return to main menu scene (if you use separate scenes).
    /// </summary>
    public void GoToMainMenu()
    {
        ResetParams();
        SceneManager.LoadScene("MainMenu");
    }

    #endregion

    #region Score / Event Callbacks (called from other gameplay scripts)

    /// <summary>
    /// Called by Orb when collected.
    /// </summary>
    public void OrbCollected()
    {
        orbScore += orbPoints;
    }

    /// <summary>
    /// Called when the player collides with obstacle.
    /// Triggers game over flow.
    /// </summary>
    public void PlayerHit()
    {
        // Final score handling
        int finalScore = TotalScore;
        if (finalScore > HighScore)
        {
            HighScore = finalScore;
            SaveHighScore();
        }

        // Broadcast game over for UI / analytics
        isGameOver = true;
        OnGameOver?.Invoke();
    }

    #endregion

    #region Persistence (High Score)
    private const string PREF_HIGH_SCORE = "SyncDash_HighScore";

    private void SaveHighScore()
    {
        PlayerPrefs.SetInt(PREF_HIGH_SCORE, HighScore);
        PlayerPrefs.Save();
    }

    private void LoadHighScore()
    {
        HighScore = PlayerPrefs.GetInt(PREF_HIGH_SCORE, 0);
    }
    private IEnumerator SpeedIncreaseRoutine()
    {
        while (!isGameOver)
        {
            yield return new WaitForSeconds(speedIncreaseInterval);

            speedMultiplier += speedIncreaseAmount;
            speedMultiplier = Mathf.Min(speedMultiplier, maxSpeedMultiplier);

            OnSpeedChanged?.Invoke(CurrentPlayerSpeed);
        }
    }

    private void ResetParams()
    {
        distanceAccum = 0f;
        orbScore = 0;
        speedMultiplier = 1f;
        isGameOver = false;
    }
    #endregion
}