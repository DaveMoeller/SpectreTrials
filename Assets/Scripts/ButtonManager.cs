using UnityEngine;
using UnityEngine.UIElements;

public class ButtonManager : MonoBehaviour
{
    private bool gamePaused = false;
    private Button playButton;
    private Button pauseButton;
    private Button quitButton;
    void OnEnable()
    {
        // Get the UIDocument component
        if (!TryGetComponent<UIDocument>(out var uiDocument)) return;

        // Get the root visual element
        var root = uiDocument.rootVisualElement;

        // Query for the button by name and register the click event
        playButton = root.Q<Button>("PlayBtn"); // Use the name you set in UI Builder
        if (playButton != null)
        {
            playButton.clicked += OnPlayButtonClicked;
        }
        // Query for the button by name and register the click event
        pauseButton = root.Q<Button>("PauseBtn"); // Use the name you set in UI Builder
        if (pauseButton != null)
        {
            pauseButton.clicked += OnPauseButtonClicked;
        }
        // Query for the button by name and register the click event
        quitButton = root.Q<Button>("QuitBtn"); // Use the name you set in UI Builder
        if (quitButton != null)
        {
            quitButton.clicked += OnQuitButtonClicked;
        }
    }
    void OnDisable()
    {
        if (playButton != null)
        {
            playButton.clicked -= OnPlayButtonClicked;
        }
        if (pauseButton != null)
        {
            pauseButton.clicked -= OnPauseButtonClicked;
        }
        if (quitButton != null)
        {
            quitButton.clicked -= OnQuitButtonClicked;
        }
    }

    public void OnPlayButtonClicked()
    {
        //Debug.Log("Play Button Clicked!");
        playButton.SetEnabled(false);
        // Add your custom logic here (e.g., load a new scene, open a panel, etc.)
        MainManager.Instance.RestartGame();
    }
    public void OnPauseButtonClicked()
    {
        //Debug.Log("Pause Button Clicked!");
        if (gamePaused)
        {
            gamePaused = false;
            pauseButton.text = "Pause";

        }
        else
        {
            gamePaused = true;
            playButton.SetEnabled(true);
            pauseButton.text = "Resume";
        }
        MainManager.Instance.PauseGame(gamePaused);
        // Add your custom logic here (e.g., load a new scene, open a panel, etc.)
    }
    public void OnQuitButtonClicked()
    {
        //Debug.Log("Quit Button Clicked!");
        MainManager.Instance.EndGame();
        // Add your custom logic here (e.g., load a new scene, open a panel, etc.)
    }
}