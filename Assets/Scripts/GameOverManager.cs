using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject gameStartPanel;
    public GameObject gameOverPanel;
    public GameObject goalPanel;
    public GameObject gamePlayPanel;

    [Header("Buttons")]
    public Button startButton;
    public Button continueYesButton;
    public Button continueNoButton;
    public Button goalToTitleButton;

    void Start()
    {        
        // ボタンにクリック処理を登録
        startButton.onClick.AddListener(OnGameStart);
        continueYesButton.onClick.AddListener(OnContinueYes);
        continueNoButton.onClick.AddListener(OnContinueNo);
        goalToTitleButton.onClick.AddListener(OnGoalToTitle);

        // 起動時はGameStart画面だけ表示
        gameStartPanel.SetActive(true);
        gamePlayPanel.SetActive(false);
        gameOverPanel.SetActive(false);
        goalPanel.SetActive(false);
        Time.timeScale = 0f; // ゲームを止めておく
    }

    // ===== GameStart =====
    public void OnGameStart()
    {
        gameStartPanel.SetActive(false);
        gamePlayPanel.SetActive(true);
        Time.timeScale = 1f; // ゲーム開始
    }

    // ===== GameOver =====
    public void ShowGameOver()
    {
        gameOverPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void OnContinueYes()
    {
        gameOverPanel.SetActive(false);
        gamePlayPanel.SetActive(true);
        Time.timeScale = 1f;
    }

    public void OnContinueNo()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // 同じシーンに戻るのでStartが表示される
    }

    // ===== Goal =====
    public void ShowGoal()
    {
        goalPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void OnGoalToTitle()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // 同上
    }
}