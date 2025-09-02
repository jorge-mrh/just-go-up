using UnityEngine;
using UnityEngine.UIElements;
using static PlayerController;

public class HUDController : MonoBehaviour
{
    private Label scoreLabel;
    private Label highScoreLabel;

    void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        scoreLabel = root.Q<Label>("score-label");
        highScoreLabel = root.Q<Label>("highscore-label");
        highScoreLabel.text = "HS " + PlayerPrefs.GetFloat("highScore", 0).ToString();
    }

    void Update()
    {
        if (currentScore > PlayerPrefs.GetFloat("highScore", 0))
        {
            PlayerPrefs.SetFloat("highScore", currentScore);
            highScoreLabel.text = "HS " + PlayerPrefs.GetFloat("highScore", 0).ToString();
        }
        scoreLabel.text = currentScore.ToString();
        highScoreLabel.text = "HS " + PlayerPrefs.GetFloat("highScore", 0).ToString();
    }
}
