using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Text References")]
    public Text scoreText;
    public Text highScoreText;

    [Header("Panels")]
    public GameObject gamePanel;
    public GameObject gameOverPanel;
    public GameObject menuPanel;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void OnEnable()
    {
        // Subscribe to GameManager events
        GameManager.Instance.OnScoreChanged.AddListener(UpdateScore);
        GameManager.Instance.OnGameOver.AddListener(ShowGameOverScreen);
    }

    private void OnDisable()
    {
        // Unsubscribe to avoid memory leaks
        GameManager.Instance.OnScoreChanged.RemoveListener(UpdateScore);
        GameManager.Instance.OnGameOver.RemoveListener(ShowGameOverScreen);
    }

    private void Start()
    {
        ShowMenu();
    }

    // ---- UI Callbacks ----

    public void StartGameButton()
    {
        gamePanel.SetActive(true);
        menuPanel.SetActive(false);
        gameOverPanel.SetActive(false);

        GameManager.Instance.StartGame();
    }

    public void RestartButton()
    {
        GameManager.Instance.Restart();
    }

    public void MenuButton()
    {
        GameManager.Instance.GoToMainMenu();
        ShowMenu();
    }

    void ShowMenu()
    {
        menuPanel.SetActive(true);
        gameOverPanel.SetActive(false);
        gamePanel.SetActive(false);

        highScoreText.text = "HIGH SCORE: " + GameManager.Instance.HighScore;
    }

    void ShowGameOverScreen()
    {
        gamePanel.SetActive(false);
        gameOverPanel.SetActive(true);
        highScoreText.text = "HIGH SCORE: " + GameManager.Instance.HighScore;
    }

    // ---- Score UI ----
    void UpdateScore(int score)
    {
        scoreText.text = score.ToString();
    }
}