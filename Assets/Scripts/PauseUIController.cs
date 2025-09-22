using UnityEngine;
using UnityEngine.UIElements;
using static PlayerController;
using UnityEngine.SceneManagement;

public class PauseUIController : MonoBehaviour
{
    [SerializeField] private VisualTreeAsset pauseUIAsset;
    private VisualElement root;
    private VisualElement pauseUIInstance;
    public bool isPaused = false;

    private Label scoreLabel;
    private Label highScoreLabel;

    private Button menuButton;
    private Button resumeButton;
    private Button restartButton;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isPaused = false;
        var uiDoc = GetComponent<UIDocument>();
        if (uiDoc == null)
            uiDoc = gameObject.AddComponent<UIDocument>();
        root = uiDoc.rootVisualElement;
        root.Clear();
    }

    public void Pause()
    {
        isPaused = true;
        Time.timeScale = 0f;
        ShowPauseUI();
    }

    public void UnPause()
    {
        Time.timeScale = 1f;
        var hudController = FindFirstObjectByType<HUDController>();
        if (hudController != null)
            hudController.ShowHUD();
        HidePauseUI();
        isPaused = false;
    }

    private void Replay()
    {
        isPaused = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void ReturnToMenu()
    {
        isPaused = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene("StartMenu");
    }

    private void ShowPauseUI()
    {
        if (pauseUIAsset == null)
        {
            Debug.LogError("Pause UI Asset not assigned!");
            return;
        }

        if (pauseUIInstance == null)
        {
            pauseUIInstance = pauseUIAsset.Instantiate();
            root.Add(pauseUIInstance);
            menuButton = root.Q<Button>("menu-button");
            resumeButton = root.Q<Button>("resume-button");
            restartButton = root.Q<Button>("restart-button");
            scoreLabel = root.Q<Label>("current-score");
            highScoreLabel = root.Q<Label>("high-score");

            // Add event listeners
            if (menuButton != null)
                menuButton.clicked += ReturnToMenu;
            if (resumeButton != null)
                resumeButton.clicked += UnPause;
            if (restartButton != null)
                restartButton.clicked += Replay;

            // Set scores after labels are assigned
            if (scoreLabel != null)
                scoreLabel.text = currentScore.ToString();
            if (highScoreLabel != null)
                highScoreLabel.text = "HS " + PlayerPrefs.GetFloat("highScore", 0).ToString();
        }
    }


    private void HidePauseUI()
    {
        if (pauseUIInstance != null)
        {
            pauseUIInstance.RemoveFromHierarchy();
            pauseUIInstance = null;
        }
    }
}
