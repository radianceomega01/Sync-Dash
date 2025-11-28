using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{

    [Header("Text References")]
    [SerializeField] private Text scoreText;
    [SerializeField] private Text highScoreText;

    [Header("Panels")]
    [SerializeField] private GameObject gameOverPanel;

    private void Awake()
    {
        
    }

    private void OnEnable()
    {
        // Subscribe to GameManager events
        GameManager.Instance.OnGameOver += ShowGameOverScreen;
    }

    private void OnDisable()
    {
        // Unsubscribe to avoid memory leaks
        GameManager.Instance.OnGameOver -= ShowGameOverScreen;
    }

    private void Start()
    {
        ShowMenu();
    }

    // ---- UI Callbacks ----

    public void StartGameButton()
    {
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
        SceneManager.LoadScene("MainMenu");
    }

    void ShowGameOverScreen()
    {
        gameOverPanel.SetActive(true);
        highScoreText.text = "HIGH SCORE: " + GameManager.Instance.HighScore;
    }

    // ---- Score UI ----
    void UpdateScore(int score)
    {
        scoreText.text = score.ToString();
    }
}