using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class MenuManager : MonoBehaviour
{
    private static UIDocument uiDocument;
    private static VisualElement root;
    private static MenuControl controls; // Reference to the generated class
    private Label titleLbl;
    private Button beginnerBtn, intermediateBtn, expertBtn, exitBtn;
    public bool autoBackToEditor = true;
    public void Awake()
    {
        uiDocument = GetComponent<UIDocument>();
        root = uiDocument.rootVisualElement;
        controls = new MenuControl();
        //controls.MenuButton.ButtonAny.performed += OnClickButton;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        titleLbl = root.Q<Label>("titleBtn");
        if (titleLbl != null)
        {
            Debug.Log($"titleLbl.text: {titleLbl.text}");
            titleLbl.text = Application.productName;
            Debug.Log($"titleLbl.text: {titleLbl.text}");


        }
        else
        {
            Debug.LogError("titleLbl is null");
        }
        //Set up functions for buttons
        beginnerBtn = root.Q<Button>("beginnerBtn");

    }
    void OnEnable()
    {
        controls.Enable(); // Actions must be enabled
        beginnerBtn = root.Q<Button>("beginnerBtn"); // Use the name you set in UI Builder
        if (beginnerBtn != null)
        {
            beginnerBtn.clicked += (() =>OnAnyClickButton(beginnerBtn.name));
        }
        intermediateBtn = root.Q<Button>("intermediateBtn"); // Use the name you set in UI Builder
        if (intermediateBtn != null)
        {
            intermediateBtn.clicked += (() => OnAnyClickButton(intermediateBtn.name));
        }
        expertBtn = root.Q<Button>("expertBtn"); // Use the name you set in UI Builder
        if (expertBtn != null)
        {
            expertBtn.clicked += (() => OnAnyClickButton(expertBtn.name));
        }
        exitBtn = root.Q<Button>("exitBtn"); // Use the name you set in UI Builder
        if (exitBtn != null)
        {
            exitBtn.clicked += (() => OnAnyClickButton(exitBtn.name));
            exitBtn.clicked += OnExitClickButton;
        }
    }
    void OnDisable()
    {
        controls.Disable(); // Actions should be disabled when not in use
    }
    public void OnAnyClickButton(string buttonName)
    {
        if (Application.isPlaying)
        {
              Debug.Log("Button Name: " + buttonName);
        }
    }
    public void OnExitClickButton()
    {
        if (Application.isPlaying)
        {
#if UNITY_EDITOR
            //Time.timeScale = 0.0f;
            if (autoBackToEditor)
            {
                EditorApplication.isPlaying = false;
            }
            // If the game is a standalone build, quit the application
#else
            Application.Quit();
#endif
        }
    }
}
