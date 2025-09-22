using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private VisualTreeAsset gameOverUIAsset;
    [SerializeField] private GameObject gameSoundtrackGameObject;
    private AudioSource gameSoundtrack;

    private bool isGameOver = false;
    private VisualElement root;
    private Button replayButton;
    private Button menuButton;
    private Label highScoreLabel;

    void Awake()
    {
        Application.targetFrameRate = 60; // or 30 for lower devices
        Screen.sleepTimeout = SleepTimeout.NeverSleep; // Prevent screen dimming
    }
    void Start()
    {
        if (player == null)
            player = GameObject.FindWithTag("Player").transform;

        var uiDoc = GetComponent<UIDocument>();
        if (uiDoc == null)
            uiDoc = gameObject.AddComponent<UIDocument>();
        if (gameSoundtrackGameObject != null)
            gameSoundtrack = gameSoundtrackGameObject.GetComponent<AudioSource>();

        root = uiDoc.rootVisualElement;
        root.Clear();
        
        Time.timeScale = 1f;
    }

    void Update()
    {
        if (isGameOver) return;
        if (player.position.y < PlatformGenerator.firstPlatformY - 0.5f)
        {
            GameOver();
        }
    }

    public void GameOver()
    {
        isGameOver = true;
        ShowGameOverUI();
        Time.timeScale = 0f;
        gameSoundtrack.Stop();
    }

    private void ShowGameOverUI()
    {
        var hudController = FindFirstObjectByType<HUDController>();
        if (hudController != null)
            hudController.HideHUD();
        if (gameOverUIAsset == null)
        {
            Debug.LogError("GameOver UI Asset not assigned!");
            return;
        }
        var gameOverUI = gameOverUIAsset.Instantiate();
        root.Add(gameOverUI);

        replayButton = root.Q<Button>("replay-button");
        if (replayButton != null)
            replayButton.clicked += Replay;

        menuButton = root.Q<Button>("menu-button");
        if (menuButton != null)
            menuButton.clicked += ReturnToMenu;

        highScoreLabel = root.Q<Label>("high-score");
        if (highScoreLabel != null)
            highScoreLabel.text = "HIGH SCORE " + PlayerPrefs.GetFloat("highScore", 0).ToString();
    }

    private void Replay()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void ReturnToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("StartMenu");
    }
}