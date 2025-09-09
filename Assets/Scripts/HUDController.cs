using UnityEngine;
using UnityEngine.UIElements;
using static PlayerController;
using UnityEngine.SceneManagement;

public class HUDController : MonoBehaviour
{

    [SerializeField] private Texture2D soundOnIcon;
    [SerializeField] private Texture2D soundOffIcon;


    private Label scoreLabel;
    private Label highScoreLabel;

    private Button soundButton;
    private Button reloadButton;

    [SerializeField] private GameObject soundtrack;
    private AudioSource audioSource;
    private GameManager gameManager;

    void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        scoreLabel = root.Q<Label>("score-label");
        highScoreLabel = root.Q<Label>("highscore-label");
        highScoreLabel.text = "HS " + PlayerPrefs.GetFloat("highScore", 0).ToString();
        soundButton = root.Q<Button>("sound-button");
        reloadButton = root.Q<Button>("reload-button");
        audioSource = soundtrack.GetComponent<AudioSource>();
        if (reloadButton != null)
            reloadButton.clicked += Reload;
        if (soundButton != null)
            soundButton.clicked += SoundConfig;
        gameManager = FindFirstObjectByType<GameManager>();
        // restore sound setting
        bool soundOn = PlayerPrefs.GetInt("soundOn", 1) == 1;
        audioSource.mute = !soundOn;
        UpdateSoundButtonIcon(!audioSource.mute);
    }

    private void OnDisable()
    {
        if (reloadButton != null)
            reloadButton.clicked -= Reload;
        if (soundButton != null)
            soundButton.clicked -= SoundConfig;
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

    private void Reload()
    {
        if (gameManager != null)
        {
            gameManager.GameOver();
            HideButtons();
        }
    }

    private void SoundConfig()
    {
        if (audioSource != null)
        {
            audioSource.mute = !audioSource.mute;
            PlayerPrefs.SetInt("soundOn", audioSource.mute ? 0 : 1);
            UpdateSoundButtonIcon(!audioSource.mute);
        }
    }
    private void UpdateSoundButtonIcon(bool soundOn)
    {
        if (soundButton != null)
        {
            soundButton.style.backgroundImage = new StyleBackground(soundOn ? soundOnIcon : soundOffIcon);
        }
    }

    public void HideButtons()
    {
        if (soundButton != null)
            soundButton.style.display = DisplayStyle.None;
        if (reloadButton != null)
            reloadButton.style.display = DisplayStyle.None;
    }
}
