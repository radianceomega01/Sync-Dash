using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class UIManager : MonoBehaviour
{

    [Header("Text References")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI highScoreText;

    [Header("Panels")]
    [SerializeField] private GameObject gameOverPanel;

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

    public void RestartButtonClicked()
    {
        gameOverPanel.SetActive(false);
        GameManager.Instance.Restart();
    }

    public void MenuButtonClicked()
    {
        GameManager.Instance.GoToMainMenu();
    }

    void ShowGameOverScreen()
    {
        highScoreText.text = string.Join("", "High Score: ", GameManager.Instance.HighScore);
        gameOverPanel.SetActive(true);
    }

    void FixedUpdate()
    {
        UpdateScore(GameManager.Instance.TotalScore);
    }

    // ---- Score UI ----
    void UpdateScore(int score)
    {
        scoreText.text = score.ToString();
    }
}