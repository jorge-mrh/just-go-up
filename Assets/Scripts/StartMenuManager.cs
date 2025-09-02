using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class StartMenuManager : MonoBehaviour
{
    private Button startButton;
    private Button quitButton;
    private Label highScoreLabel;

    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        startButton = root.Q<Button>("start-button");
        quitButton = root.Q<Button>("quit-button");
        highScoreLabel = root.Q<Label>("high-score");

        if (startButton != null)
            startButton.clicked += StartGame;

        if (quitButton != null)
            quitButton.clicked += QuitGame;
            
        if (highScoreLabel != null)
            highScoreLabel.text = "HIGH SCORE " + PlayerPrefs.GetFloat("highScore", 0).ToString();
    }

    private void StartGame()
    {
        SceneManager.LoadScene("MainScene");
    }

    private void QuitGame()
    {
        Application.Quit();
    }
}
