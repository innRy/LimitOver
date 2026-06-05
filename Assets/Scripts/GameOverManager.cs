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
    //public Button goalToNextButton;

    [Header("player")]
    public Transform player;
    public Transform startPosition;

    void Start()
    {
        // ボタンにクリック処理を登録
        startButton.onClick.AddListener(OnGameStart);

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
        // プレイヤーをスタート位置に移動
        player.position = startPosition.position;
        player.rotation = startPosition.rotation; // 向きもリセットしたい場合

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
        gameStartPanel.SetActive(true);
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


}