using System;
using UnityEditor;
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
    public GameObject gameWorld;
    public bool autoBackToEditor = false;
    private Label gameOverText;
    [Range(0.1f, 1.0f)]
    [Tooltip("Incremental range to move")]
    public float deltaMovement = 0.1f;
    [Range(0.1f, 10.0f)]
    [Tooltip("Force to apply in direction")]
    public float forceToApply = 1.5f;
    [Range(0.1f, 2.0f)]
    [Tooltip("Force to apply in direction using Impulse")]
    public float forceToApplyImpulse = 0.5f;
    Rigidbody2D playerRB;
    public UIDocument uiDocument;
    private Label timeText;
    private Label scoreText;
    private float gameTimeInSeconds;
    private static Camera m_cameraMain = null;
    private static Camera m_cameraPlayer = null;
    public Camera cameraMain;
    public Camera cameraPlayer;
    private bool cameraMainOn = true;
    GameObject[] startingLocations;
    private int currentStartingLocation = 0;
    public String[] tagsToTurnOnForSwitch;
    private string tagToFind = "StartLocation";
    [Tooltip("Object to hit and get points.")]
    public GameObject pointObject;
    private int CurrentScore = 0;
    private VisualElement root;
    public GameObject eyes;
    public float eyeMoveAmount = 0.03f;
    public bool usePulseAcceleration = false;
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
        timeText = uiDocument.rootVisualElement.Q<Label>("TimeText");
        scoreText = uiDocument.rootVisualElement.Q<Label>("ScoreText");
        //Find all starting locations
        startingLocations = GameObject.FindGameObjectsWithTag(tagToFind);
        if (startingLocations.Length > 0)
        {
            int randomStart = UnityEngine.Random.Range(0, startingLocations.Length - 1);
            // set player location to first start location
            Debug.Log($"Setting starting position to: {startingLocations[randomStart].name}");
            player.transform.SetPositionAndRotation(startingLocations[randomStart].transform.position, Quaternion.identity);
        }
        else
        {
            Debug.LogError("No starting locations exist!");
            return;
        }
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
        setObjectsVisible(false);
        root = uiDocument.rootVisualElement;
    }
    public void setObjectsVisible(bool visible)
    {
        //Switch objects with tags off - to be turned on later with object hit
        if (tagsToTurnOnForSwitch.Length > 0)
        {
            for (int i = 0; i < tagsToTurnOnForSwitch.Length; i++)
            {
                //Get the objects with tag
                GameObject[] ObjectsToStitchOff = GameObject.FindGameObjectsWithTag(tagsToTurnOnForSwitch[i]);
                for (int j = 0; j < ObjectsToStitchOff.Length; j++)
                {
                    //Turn off
                    SpriteRenderer sr = ObjectsToStitchOff[j].GetComponent<SpriteRenderer>();
                    sr.enabled = visible;
                }
            }
        }
        if (visible)
        {
            CreatePointObjects();

        }

    }
    private void Update()
    {
        bool isPressed;
        if (player != null)
        {
            gameTimeInSeconds += Time.deltaTime;
            timeText.text = "Time: " + TimeSpan.FromSeconds(gameTimeInSeconds).ToString(@"mm\:ss");
            //switch starting locations
            isPressed = controls.GamePlay.Location.WasPressedThisFrame();
            if (isPressed)
            {
                currentStartingLocation++;
                if (currentStartingLocation >= startingLocations.Length)
                {
                    currentStartingLocation = 0;
                }
                player.transform.SetPositionAndRotation(startingLocations[currentStartingLocation].transform.position, Quaternion.identity);
            }
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
        isPressed = controls.GamePlay.GameEnd.IsPressed();
        if (isPressed)
        {
            Debug.Log("Game End Selected!");
            EndGame();
        }

    }

    public void FixedUpdate()
    {
        bool isPressed;
        isPressed = controls.Move.MoveLeft.IsPressed();//.Gameplay.GameStart.IsPressed();
        if (isPressed)
        {
            Debug.Log("MoveLeft pressed!");
            Vector3 pos, borderPos;
            Quaternion rotation, borderRotation;
            player.transform.GetPositionAndRotation(out pos, out rotation);
            leftmostBorder.transform.GetPositionAndRotation(out borderPos, out borderRotation);
            Debug.Log($"pos: {pos}, borderPos: {borderPos}");
            Vector2 newDir;
            if (usePulseAcceleration)
            {
                newDir = new Vector2((-1.0f * forceToApplyImpulse * Time.fixedDeltaTime), 0.0f).normalized;
                playerRB.AddForce(newDir, ForceMode2D.Impulse);

            }
            else
            {
                newDir = new Vector2((-1.0f * forceToApply * Time.fixedDeltaTime), 0.0f).normalized;
                playerRB.AddForce(newDir, ForceMode2D.Force);

            }
            Debug.Log($"PlayerDirection: {newDir}, Velocity: {playerRB.linearVelocityX}");
            //move eyes
        }
        /*
        if (controls.Move.MoveLeft.WasPressedThisFrame())
        {
            //eyes.transform.position + 
            eyes.transform.position = new Vector3(-eyeMoveAmount, 0);

        }

        if (controls.Move.MoveRight.WasPressedThisFrame())
        {
            //eyes.transform.position + 
            eyes.transform.position = new Vector3(eyeMoveAmount, 0);

        }
        */
        isPressed = controls.Move.MoveRight.IsPressed();
        if (isPressed)
        {
            Debug.Log("MoveRight pressed!");
            //playerRB.linearVelocityX = forceToApply;
            //playerRB.linearVelocityX = -deltaMovement;
            //playerRB.AddForceX(-deltaMovement, ForceMode2D.Impulse);
            Vector2 newDir;
            if (usePulseAcceleration)
            {
                newDir = new Vector2((1.0f * forceToApplyImpulse * Time.fixedDeltaTime), 0.0f).normalized;
                playerRB.AddForce(newDir, ForceMode2D.Impulse);
            }
            else
            {
                newDir = new Vector2((1.0f * forceToApply * Time.fixedDeltaTime), 0.0f).normalized;
                playerRB.AddForce(newDir, ForceMode2D.Force);

            }
            Debug.Log($"PlayerManager:PlayerDirection: {newDir}, Velocity: {playerRB.linearVelocityX}");
        }
        isPressed = controls.Move.MoveUp.IsPressed();
        if (isPressed)
        {
            Debug.Log("MoveUp pressed!");
            //playerRB.linearVelocityX = forceToApply;
            //playerRB.linearVelocityX = -deltaMovement;
            //playerRB.AddForceX(-deltaMovement, ForceMode2D.Impulse);
            Vector2 newDir;
            if (usePulseAcceleration)
            {
                newDir = new Vector2(0.0f, (1.0f * forceToApplyImpulse * Time.fixedDeltaTime)).normalized;
                playerRB.AddForce(newDir, ForceMode2D.Impulse);
            }
            else
            {
                newDir = new Vector2(0.0f, (1.0f * forceToApply * Time.fixedDeltaTime)).normalized;
                playerRB.AddForce(newDir, ForceMode2D.Force);
 
            }
            Debug.Log($"PlayerManager:PlayerDirection: {newDir}, Velocity: {playerRB.linearVelocityX}");
        }
        isPressed = controls.Move.MoveDown.IsPressed();
        if (isPressed)
        {
            Debug.Log("MoveDown pressed!");
            //playerRB.linearVelocityX = forceToApply;
            //playerRB.linearVelocityX = -deltaMovement;
            //playerRB.AddForceX(-deltaMovement, ForceMode2D.Impulse);
            Vector2 newDir;
            if (usePulseAcceleration)
            {
                newDir = new Vector2(0.0f, (-1.0f * forceToApplyImpulse * Time.fixedDeltaTime)).normalized;
                playerRB.AddForce(newDir, ForceMode2D.Impulse);
            }
            else
            {
                newDir = new Vector2(0.0f, (-1.0f * forceToApply * Time.fixedDeltaTime)).normalized;
                playerRB.AddForce(newDir, ForceMode2D.Force);

            }
            Debug.Log($"PlayerManager:PlayerDirection: {newDir}, Velocity: {playerRB.linearVelocityX}");
        }

    }
    private void CreatePointObjects()
    {
        //Instantiate point objects in gamespace
        float x, y;
        float incrementX = 1.0f;
        float incrementY = -1.50f;
        float startX = -14.0f, startY = 30.75f;
        float endX = 13.0f, endY = 5.25f;

        for (x = startX; x <= endX; x = x + incrementX)
        {
            for (y = startY; y >= endY; y = y + incrementY)
            {
                //x = -14.75f; y = 19.25f;
                Vector3 poScale = pointObject.transform.localScale;
                Vector2 pos = new Vector2(x, y);
                Vector2 boxSize = new Vector2(poScale.x, poScale.y);
                Debug.Log($"Trying to instantiate at {pos}");
                if (!Physics2D.OverlapBox(pos, boxSize, 0.0f))
                {
                    Instantiate(pointObject, pos, Quaternion.identity);
                    Debug.Log("Object spawned in empty space!");
                }
                else
                {
                    Debug.Log("Space occupied, cannot spawn.");
                }
            }
        }
    }
    public void IncrementScore(int score = 1)
    {
        CurrentScore += score;
        UpdateScoreText();
    }
    public void UpdateScoreText()
    {
        scoreText.text = $"Score:  {CurrentScore}";
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Collision: " + collision.ToString());
    }
    public void EndGame()
    {
        // Turn off GameWorld

        gameWorld.SetActive(false);

        // Display GameOverText

        gameOverText = uiDocument.rootVisualElement.Q<Label>("GameOverText");

        gameOverText.visible = true;

        // GameOverText
        if (autoBackToEditor) { EditorApplication.isPlaying = false; }
#if UNITY_EDITOR
        if (autoBackToEditor)

        {
            EditorApplication.isPlaying = false;
        }
        // If the game is a standalone build, quit the application
#else
            Application.Quit();
#endif
        Debug.Log("Game Over!");
    }
}
