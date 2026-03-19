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
    public GameObject leftmostBorder;
    public GameObject rightmostBorder;
    public GameObject bottommostBorder;
    public GameObject topmostBorder;
    [Range(0.1f, 1.0f)]
    [Tooltip("Incremental range to move")]
    public float deltaMovement = 0.1f;
    [Range(0.1f, 10.0f)]
    [Tooltip("Force to apply in direction")]
    public float forceToApply = 1.5f;
    Rigidbody2D playerRB;
    public UIDocument uiDocument;
    private Label scoreText;
    private float gameTimeInSeconds;
    private static Camera m_cameraMain = null;
    private static Camera m_cameraPlayer = null;
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
        if (m_cameraMain == null)
        {
            m_cameraMain = cameraMain;
        }
        else
        {
            cameraMain = m_cameraMain;
        }
        if (m_cameraPlayer == null)
        {
            m_cameraPlayer = cameraPlayer;

        }
        else
        {
            cameraPlayer = m_cameraPlayer;
        }
    }
    private void Update()
    {
        if (player != null)
        {
            bool isPressed;
            gameTimeInSeconds += Time.deltaTime;
            scoreText.text = "Time: " + TimeSpan.FromSeconds(gameTimeInSeconds).ToString(@"mm\:ss");
            /*
            isPressed = controls.Move.MoveUp.IsPressed();
            if (isPressed)
            {
                Debug.Log("MoveUp pressed!");
                //Vector3 pos = player.transform.position;
                Vector3 pos, borderPos;
                Quaternion rotation, borderRotation;
                player.transform.GetPositionAndRotation(out pos, out rotation);
                topmostBorder.transform.GetPositionAndRotation(out borderPos, out borderRotation);
                //Debug.Log($"pos: {pos}, borderPos: {borderPos}");
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
          */
            //Reload the game
            isPressed = controls.GamePlay.GameStart.IsPressed();
            if (isPressed)
            {
                Debug.Log("Start Selected!");
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                cameraPlayer = m_cameraPlayer;
                cameraMain = m_cameraMain;
                //Reset();
                return;
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
            if (cameraPlayer != null)
            {
                cameraPlayer.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, cameraPlayer.transform.position.z);
            }
        }

    }

    public void FixedUpdate()
    {
        bool isPressed = controls.Move.MoveLeft.IsPressed();//.Gameplay.GameStart.IsPressed();
        if (isPressed)
        {
            Debug.Log("MoveLeft pressed!");
            Vector3 pos, borderPos;
            Quaternion rotation, borderRotation;
            player.transform.GetPositionAndRotation(out pos, out rotation);
            leftmostBorder.transform.GetPositionAndRotation(out borderPos, out borderRotation);
            Debug.Log($"pos: {pos}, borderPos: {borderPos}");
            //Incremental Movements
            //playerRB.linearVelocityX = -forceToApply;
            //playerRB.linearVelocityX = -deltaMovement;
            //playerRB.AddForceX(-deltaMovement, ForceMode2D.Impulse);
            Vector2 newDir = new Vector2((-1.0f * forceToApply * Time.deltaTime), 0.0f).normalized;
            playerRB.AddForce(newDir, ForceMode2D.Force);
            Debug.Log($"PlayerDirection: {newDir}, Velocity: {playerRB.linearVelocityX}");
        }
        isPressed = controls.Move.MoveRight.IsPressed();
        if (isPressed)
        {
            Debug.Log("MoveRight pressed!");
            //playerRB.linearVelocityX = forceToApply;
            //playerRB.linearVelocityX = -deltaMovement;
            //playerRB.AddForceX(-deltaMovement, ForceMode2D.Impulse);
            Vector2 newDir = new Vector2((1.0f * forceToApply * Time.deltaTime), 0.0f).normalized;
            playerRB.AddForce(newDir, ForceMode2D.Force);
            Debug.Log($"PlayerManager:PlayerDirection: {newDir}, Velocity: {playerRB.linearVelocityX}");
        }
        isPressed = controls.Move.MoveUp.IsPressed();
        if (isPressed)
        {
            Debug.Log("MoveUp pressed!");
            //playerRB.linearVelocityX = forceToApply;
            //playerRB.linearVelocityX = -deltaMovement;
            //playerRB.AddForceX(-deltaMovement, ForceMode2D.Impulse);
            Vector2 newDir = new Vector2(0.0f, (1.0f * forceToApply * Time.deltaTime)).normalized;
            playerRB.AddForce(newDir, ForceMode2D.Force);
            Debug.Log($"PlayerManager:PlayerDirection: {newDir}, Velocity: {playerRB.linearVelocityX}");
        }
        isPressed = controls.Move.MoveDown.IsPressed();
        if (isPressed)
        {
            Debug.Log("MoveDown pressed!");
            //playerRB.linearVelocityX = forceToApply;
            //playerRB.linearVelocityX = -deltaMovement;
            //playerRB.AddForceX(-deltaMovement, ForceMode2D.Impulse);
            Vector2 newDir = new Vector2(0.0f, (-1.0f * forceToApply * Time.deltaTime)).normalized;
            playerRB.AddForce(newDir, ForceMode2D.Force);
            Debug.Log($"PlayerManager:PlayerDirection: {newDir}, Velocity: {playerRB.linearVelocityX}");
        }

    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Collision: " + collision.ToString());
    }
}
