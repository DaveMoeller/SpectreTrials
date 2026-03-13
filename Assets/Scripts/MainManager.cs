using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MainManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public static MainManager Instance;
    private static PlayerControl controls; // Reference to the generated class
    public GameObject player;
    bool m_GameOver = false;
    bool m_Started = false;
    public GameObject leftmostBorder;
    public GameObject rightmostBorder;
    public GameObject bottommostBorder;
    public GameObject topmostBorder;
    [Range(0.1f, 1f)]
    [Tooltip("Incremental range to move")]
    public float deltaMovement = 0.1f;
    Rigidbody2D playerRB;
    public UIDocument uiDocument;
    private Label scoreText;
    private float gameTimeInSeconds;
    public Camera cameraMain;
    public Camera cameraPlayer;
    private bool cameraMainOn = true;
    public PlayerControl PlayerControlsShared { get { return controls; } }
    void OnEnable()
    {
        controls.Enable(); // Actions must be enabled
    }

    void OnDisable()
    {
        controls.Disable(); // Actions should be disabled when not in use
    }
    public void Awake()
    {
        //Debug.Log("MainManager gameObject.name: " + gameObject.name);
        if (Instance != null)
        {
            return;
        }
        else
        {
            Instance = this;
            controls = new PlayerControl();
            DontDestroyOnLoad(Instance); // same as GameObject

        }
    }
    void Start()
    {
        playerRB = player.GetComponent<Rigidbody2D>();
        scoreText = uiDocument.rootVisualElement.Q<Label>("TimeText");
    }
    private void Update()
    {
        gameTimeInSeconds += Time.deltaTime;
        scoreText.text = "Time: " + TimeSpan.FromSeconds(gameTimeInSeconds).ToString(@"mm\:ss");
        if (!m_Started)
        {
            // Update is called once per frame
            bool isPressed = controls.Move.MoveLeft.IsPressed();//.Gameplay.GameStart.IsPressed();
            if (isPressed)
            {
                Debug.Log("MoveLeft pressed!");
                Vector3 pos, borderPos;
                Quaternion rotation, borderRotation;
                player.transform.GetPositionAndRotation(out pos, out rotation);
                leftmostBorder.transform.GetPositionAndRotation(out borderPos, out borderRotation);
                Debug.Log($"pos: {pos}, borderPos: {borderPos}");

                pos.x -= deltaMovement;
                if (pos.x < borderPos.x)
                {
                    pos.x = borderPos.x;
                    //leftmostBorder.transform.
                }
                player.transform.position = pos;
                //playerRB.linearVelocityX = -deltaMovement;
                playerRB.linearVelocityX = -deltaMovement;
                //playerRB.AddForceX(-deltaMovement, ForceMode2D.Impulse);
                //Vector2 newDir = new Vector2(1.0f + (1.0f * (-deltaMovement)), 0.0f);
                //playerRB.AddForce(newDir);
            }
            isPressed = controls.Move.MoveRight.IsPressed();
            if (isPressed)
            {
                Debug.Log("MoveRight pressed!");
                Vector3 pos = player.transform.position;
                pos.x += deltaMovement;
                if (pos.x > rightmostBorder.transform.position.x)
                {
                    pos.x = rightmostBorder.transform.position.x;
                }
                player.transform.position = pos;
                //playerRB.linearVelocityX = deltaMovement;
                //playerRB.AddForceX(deltaMovement, ForceMode2D.Impulse);
            }
            isPressed = controls.Move.MoveUp.IsPressed();//.Gameplay.GameStart.IsPressed();
            if (isPressed)
            {
                Debug.Log("MoveUp pressed!");
                //Vector3 pos = player.transform.position;
                Vector3 pos, borderPos;
                Quaternion rotation, borderRotation;
                player.transform.GetPositionAndRotation(out pos, out rotation);
                topmostBorder.transform.GetPositionAndRotation(out borderPos, out borderRotation);
                Debug.Log($"pos: {pos}, borderPos: {borderPos}");
                pos.y += deltaMovement;
                if (pos.y > borderPos.y)
                {
                    pos.y = borderPos.y;
                }
                player.transform.position = pos;
            }
            isPressed = controls.Move.MoveDown.IsPressed();
            if (isPressed)
            {
                Debug.Log("MoveDown pressed!");
                Vector3 pos, borderPos;
                Quaternion rotation, borderRotation;
                player.transform.GetPositionAndRotation(out pos, out rotation);
                bottommostBorder.transform.GetPositionAndRotation(out borderPos, out borderRotation);
                Debug.Log($"pos: {pos}, borderPos: {borderPos}");
                //Vector3 pos = player.transform.position;
                pos.y -= deltaMovement;
                if (pos.y < borderPos.y)
                {
                    pos.y = borderPos.y;
                }
                player.transform.position = pos;
            }
            //Reload the game
            isPressed = controls.GamePlay.GameStart.IsPressed();
            if (isPressed)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                //Reset();
            }
            isPressed = controls.Camera.CameraButton.WasPressedThisFrame();
            if (isPressed)
            {
                Debug.Log("Camera Toggle is pressed!");
                if (cameraMainOn)
                {
                    //Set the transform of camera above player
                    cameraPlayer.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, cameraPlayer.transform.position.z);
                    cameraMainOn = false;
                    cameraMain.gameObject.SetActive(false);
                    cameraPlayer.gameObject.SetActive(true);
                }
                else
                {
                    cameraMainOn = true;
                    cameraPlayer.gameObject.SetActive(false);
                    cameraMain.gameObject.SetActive(true);

                }
            }
            cameraPlayer.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, cameraPlayer.transform.position.z);

        }
        else if (m_GameOver)
        {
            //Direct read from keyboard
            //bool isPressed = Keyboard.current[Key.Space].isPressed;
            //bool isPressed = controls.Gameplay.GameStart.IsPressed();
            //if (isPressed)
            //{
            //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            //Reset();
            //}
        }
    }

}
