using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
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
    private Rigidbody2D playerRB;
    public UIDocument uiDocument;
    private Label timeText;
    private Label scoreText;
    private Label highScoreText01;
    private float gameTimeInSeconds;
    private static Camera m_cameraMain = null;
    private static Camera m_cameraPlayer = null;
    public Camera cameraMain;
    public float minCameraMainSize = 1.0f;
    public float maxCameraMainSize = 25.0f;
    public float cameraMainZoomIncrement = 1.0f;
    public Camera cameraPlayer;
    public float minCameraPlayerSize = 1.0f;
    public float maxCameraPlayerSize = 25.0f;
    public float cameraPlayerZoomIncrement = 1.0f;
    private bool cameraMainOn = true;
    GameObject[] startingLocations;
    private int currentStartingLocation = 0;
    public String[] tagsToTurnOnForSwitch;
    private readonly string startingLocationTag = "StartLocation";
    [Tooltip("Object to hit and get points.")]
    public GameObject pointObject;
    private int CurrentScore = 0;
    private int HighScore = 0;
    private VisualElement root;
    public GameObject eyes;
    public float eyeMoveAmount = 0.03f;
    public bool usePulseAcceleration = false;
    [Range(.1f, 2.0f)]
    public float pointObjectIncrementX = 1.0f;
    [Range(-2.0f, -0.1f)]
    public float pointObjectIncrementY = -1.0f;
    public string looseText = "Sorry about your skills!\nPlease try again to improve\nGame Over!";
    public string winText = "Great Job!\nPlease play again soon!\nGame Over!";

    public GameObject[] pointObjects;
    private int numberOfPointObjects = 0;
    [Tooltip("Player Locator GameObject")]
    public GameObject playerLocators;
    public GameObject[] pointObjectsBoundingBoxes;
    private string highScoreKey;
    public GameLevels gameLevel = GameLevels.BEGINNER;
    public GameObject[] beginnerGameObjectsToDisable;
    public GameObject[] intermediateGameObjectsToDisable;
    public PlayerControl PlayerControlsShared { get { return controls; } }
    private Button mainMenuButton;

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
            //CreatePointObjects();
            return;
        }
        else
        {
            Instance = this;
            controls = new PlayerControl();
            controls.Camera.ZoomMouse.performed += OnScroll;
            //DontDestroyOnLoad(Instance); // same as GameObject

        }
        SetObjectsVisible(false);
        root = uiDocument.rootVisualElement;
        CreatePointObjects();
    }
    void Start()
    {
        if (GameData.Instance == null)
        {
            Debug.LogError("GameData not defined. Enter thru Menu!");
        }
        else
        {
            gameLevel = GameData.Instance.Level;
            //Debug.Log($"Game Level: {gameLevel}");
        }

        //Disable challenges based on level
        switch (gameLevel)
        {
            case GameLevels.BEGINNER:
                {
                    //Disable BEGINNER items
                    for (int i = 0; i < beginnerGameObjectsToDisable.Length; i++)
                    {
                        beginnerGameObjectsToDisable[i].SetActive(false);
                    }
                    break;
                }
            case GameLevels.INTERMEDIATE:
                {
                    //intermediateGameObjectsToDisable
                    for (int i = 0; i < intermediateGameObjectsToDisable.Length; i++)
                    {
                        intermediateGameObjectsToDisable[i].SetActive(false);
                    }
                    break;
                }
            default:
                {
                    //everything enabled
                    break;
                }
        }

        playerRB = player.GetComponent<Rigidbody2D>();
        timeText = uiDocument.rootVisualElement.Q<Label>("TimeText");
        scoreText = uiDocument.rootVisualElement.Q<Label>("ScoreText");
        highScoreText01 = uiDocument.rootVisualElement.Q<Label>("HighScoreText01");
        SetObjectsVisible(false);
        MovePlayerToNewStartingLocation();
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
        highScoreKey = Application.productName + "_" + HighScore;
        GetPlayerPrefs();
        SetHighScoreText();
        mainMenuButton = root.Q<Button>("MainMenuButton"); // Use the name you set in UI Builder
        if (mainMenuButton != null)
        {
            if (Instance != null)
            {
                mainMenuButton.clicked += GoToMainMenu;
            }
            else
            {
                Debug.LogError("Instance is null");
            }

        }

    }
    public void MovePlayerToNewStartingLocation()
    {
        //Find all starting locations
        startingLocations = GameObject.FindGameObjectsWithTag(startingLocationTag);
        if (startingLocations.Length > 0)
        {
            int randomStart = UnityEngine.Random.Range(0, startingLocations.Length - 1);
            // set player location to first start location
            //Debug.Log($"Setting starting position to: {startingLocations[randomStart].name}");
            player.transform.SetPositionAndRotation(startingLocations[randomStart].transform.position, Quaternion.identity);
        }
        else
        {
            //Debug.LogError("No starting locations exist!");
            return;
        }

    }
    public void SetObjectsVisible(bool visible)
    {
        //Switch objects with tags off - to be turned on later with object hit
        if (tagsToTurnOnForSwitch.Length > 0)
        {
            for (int i = 0; i < tagsToTurnOnForSwitch.Length; i++)
            {
                //Get the objects with tag
                GameObject[] ObjectsToSwitchOff = GameObject.FindGameObjectsWithTag(tagsToTurnOnForSwitch[i]);
                for (int j = 0; j < ObjectsToSwitchOff.Length; j++)
                {
                    //Turn off
                    SpriteRenderer sr = ObjectsToSwitchOff[j].GetComponent<SpriteRenderer>();
                    sr.enabled = visible;
                }
            }
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
            isPressed = controls.GamePlay.Talk.WasPressedThisFrame();
            if (isPressed)
            {
                SoundManager.PlaySound(SoundType.CRICKETS);
            }

            //Reload the game
            isPressed = controls.GamePlay.GameStart.IsPressed();
            if (isPressed)
            {
                RestartGame();
                return;
            }
            isPressed = controls.Camera.CameraButton.WasPressedThisFrame();
            if (isPressed)
            {
                //Debug.Log("Camera Toggle is pressed!");
                if (cameraMainOn)
                {
                    //Set the transform of camera above player
                    cameraPlayer.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, cameraPlayer.transform.position.z);
                    cameraMainOn = false;
                    cameraMain.gameObject.SetActive(false);
                    cameraPlayer.gameObject.SetActive(true);
                    playerLocators.SetActive(false);
                }
                else
                {
                    cameraMainOn = true;
                    cameraPlayer.gameObject.SetActive(false);
                    cameraMain.gameObject.SetActive(true);
                    playerLocators.SetActive(true);

                }
            }
            if (cameraPlayer != null)
            {
                cameraPlayer.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, cameraPlayer.transform.position.z);
            }
        }
        isPressed = controls.GamePlay.GameEnd.WasPressedThisFrame();
        if (isPressed)
        {
            //Debug.Log("Game End Selected!");
            EndGame();
        }
    }

    public void FixedUpdate()
    {
        bool isPressed;
        isPressed = controls.Move.MoveLeft.IsPressed();//.Gameplay.GameStart.IsPressed();
        if (isPressed)
        {
            //Debug.Log("MoveLeft pressed!");
            player.transform.GetPositionAndRotation(out _, out _);
            leftmostBorder.transform.GetPositionAndRotation(out _, out _);
            //Debug.Log($"pos: {pos}, borderPos: {borderPos}");
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
            //Debug.Log($"PlayerDirection: {newDir}, Velocity: {playerRB.linearVelocityX}");
            //move eyes
        }
        isPressed = controls.Move.MoveRight.IsPressed();
        if (isPressed)
        {
            //Debug.Log("MoveRight pressed!");
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
            //Debug.Log($"PlayerManager:PlayerDirection: {newDir}, Velocity: {playerRB.linearVelocityX}");
        }
        isPressed = controls.Move.MoveUp.IsPressed();
        if (isPressed)
        {
            //Debug.Log("MoveUp pressed!");
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
            //Debug.Log($"PlayerManager:PlayerDirection: {newDir}, Velocity: {playerRB.linearVelocityX}");
        }
        isPressed = controls.Move.MoveDown.IsPressed();
        if (isPressed)
        {
            //Debug.Log("MoveDown pressed!");
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
            //Debug.Log($"PlayerManager:PlayerDirection: {newDir}, Velocity: {playerRB.linearVelocityX}");
        }
        isPressed = controls.Camera.ZoomIn.WasPressedThisFrame();
        if (isPressed)
        {
            ZoomCamera(true);
        }
        isPressed = controls.Camera.ZoomOut.WasPressedThisFrame();
        if (isPressed)
        {
            ZoomCamera(false);
        }
    }
    public void RestartGame()
    {
        //Debug.Log("Start Selected!");
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    }
    public void GoToMainMenu()
    {
        //Debug.Log("Main Menu Selected!");
        SceneManager.LoadScene(0);

    }
    private void ZoomCamera(bool zoomIn = true)
    {
        float minSize;
        float maxSize;
        float zoomIncrement;
        Camera currentCamera;
        if (cameraMainOn)
        {
            currentCamera = cameraMain;
            minSize = minCameraMainSize;
            maxSize = maxCameraMainSize;
            zoomIncrement = cameraMainZoomIncrement;
        }
        else
        {
            currentCamera = cameraPlayer;
            minSize = minCameraPlayerSize;
            maxSize = maxCameraPlayerSize;
            zoomIncrement = cameraPlayerZoomIncrement;
        }
        // Get the current camera size
        float cameraSize = currentCamera.orthographicSize;

        // Adjust size
        if (zoomIn)
        {
            cameraSize -= zoomIncrement;
        }
        else
        {
            cameraSize += zoomIncrement;
        }


        // Clamp the FOV so it doesn't go too far
        cameraSize = Mathf.Clamp(cameraSize, minSize, maxSize);

        //currentCamera.fieldOfView = fov;
        currentCamera.orthographicSize = cameraSize;
    }
    public void OnScroll(InputAction.CallbackContext context)
    {
        Vector2 scrollValue = context.ReadValue<Vector2>();
        // Positive for Up, Negative for Down
        //Debug.Log($"scrollValue.y:{scrollValue.y}");
        if (scrollValue.y != 0)
        {
            if (scrollValue.y > 0)
            {
                ZoomCamera(true);
            }
            else
            {
                ZoomCamera(false);
            }
        }
    }

    public void PauseGame(bool pause = true)
    {
        if (pause)
        {
            Time.timeScale = 0.0f;

        }
        else
        {
            Time.timeScale = 1.0f;

        }
    }
    public void CreatePointObjects()
    {
        //Instantiate point objects in gamespace
        float x, y;
        int maxArraySize;
        float incrementX = pointObjectIncrementX;
        float incrementY = pointObjectIncrementY;
        //ToDo: Remove hard coding for point object box
        for (int i = 0; i < pointObjectsBoundingBoxes.Length; i++)
        {
            //Find max and min of box
            float startX, startY;
            float endX, endY;
            //Calculate
            startX = pointObjectsBoundingBoxes[i].transform.position.x -
                (pointObjectsBoundingBoxes[i].transform.localScale.x / 2.0f);
            startY = pointObjectsBoundingBoxes[i].transform.position.y +
                (pointObjectsBoundingBoxes[i].transform.localScale.y / 2.0f);
            endX = pointObjectsBoundingBoxes[i].transform.position.x +
                 (pointObjectsBoundingBoxes[i].transform.localScale.x / 2.0f);
            endY = pointObjectsBoundingBoxes[i].transform.position.y -
                 (pointObjectsBoundingBoxes[i].transform.localScale.y / 2.0f);
            //Destroy bounding box so logic does not think something is there
            Destroy(pointObjectsBoundingBoxes[i]);
            Vector3 poScale = pointObject.transform.localScale;
            Vector2 boxSize = new(poScale.x, poScale.y);
            //Debug.Log($"GameWorld.transform:{gameWorld.transform}");
            //Allocate array size based on maximum points
            //Debug.Log($"Point Object Bounding Box: [{startX}, {startY}, {endX}, {endY}]");
            maxArraySize = Math.Abs((int)Math.Ceiling(((endX - startX) / incrementX) * ((endY - startY) / incrementY)));
            if (i == 0)
            {
                pointObjects = new GameObject[maxArraySize];
            }
            else
            {
                //Resize array based on actual results
                maxArraySize += numberOfPointObjects;
                Array.Resize(ref pointObjects, maxArraySize);

            }
            //Resize array based on actual results.
            bool success = false;
            for (x = startX; x <= endX; x += incrementX)
            {
                for (y = startY; y >= endY; y += incrementY)
                {
                    Vector2 pos = new(x, y);
                    //Debug.Log($"Trying to instantiate at {pos}");
                    if (!Physics2D.OverlapBox(pos, boxSize, 0.0f))
                    {
                        GameObject newGo;
                        newGo = Instantiate(pointObject, pos, Quaternion.identity, gameWorld.transform);
                        pointObjects[numberOfPointObjects] = newGo;
                        numberOfPointObjects++;
                        success = true;
                        //Debug.Log("Object spawned in empty space!");
                    }
                    else
                    {
                        //Debug.Log("Space occupied, cannot spawn.");
                    }
                }
                if (success)
                {
                     success = false;
                }
            }

        }
        //Resize array based on actual results
        Array.Resize(ref pointObjects, numberOfPointObjects);
    }
    public void SetPointObjectsActive(bool active)
    {
        //GameObject[] pointObjects = GameObject.FindGameObjectsWithTag("PointObject");
        for (int i = 0; i < numberOfPointObjects; i++)
        {
            pointObjects[i].SetActive(active);
        }
    }
    private void SavePlayerPrefs()
    {
        // Save high score and other scores
        //SpectreTrials
        PlayerPrefs.SetInt(highScoreKey, HighScore);
        PlayerPrefs.Save();
    }
    private void GetPlayerPrefs()
    {
        HighScore = PlayerPrefs.GetInt(highScoreKey, 0);

    }
    public void IncrementScore(int score = 1)
    {
        CurrentScore += score;
        if (CurrentScore >= HighScore) HighScore = CurrentScore;
        UpdateScoreText();
    }
    public void UpdateScoreText()
    {
        scoreText.text = $"Score:  {CurrentScore}";
        SetHighScoreText();
    }
    private void SetHighScoreText()
    {
        highScoreText01.text = $"High Score: {HighScore}";

    }
    public void EndGame()
    {
        SavePlayerPrefs();
        int numberOfPointObjectsLeft = GameObject.FindGameObjectsWithTag("PointObject").Length;
        // Display GameOverText
        //If points left then loose, else win

        gameOverText = uiDocument.rootVisualElement.Q<Label>("GameOverText");
        if (numberOfPointObjectsLeft > 0)
        {

            gameOverText.text = looseText;
            SoundManager.PlaySound(SoundType.ENDGAMELOOSE);

        }
        else
        {
            gameOverText.text = winText;
            SoundManager.PlaySound(SoundType.ENDGAMEWIN);

        }
        // Turn off GameWorld
        gameWorld.SetActive(false);
        // GameOverText

        gameOverText.visible = true;

#if UNITY_EDITOR
        Time.timeScale = 0.0f;
        if (autoBackToEditor)
        {
            EditorApplication.isPlaying = false;
        }
        // If the game is a standalone build, quit the application
#else
            Application.Quit();
#endif
        //Debug.Log("Game Over!");
    }
}
