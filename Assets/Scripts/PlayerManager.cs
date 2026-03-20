using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    private static PlayerControl controls; // Reference to the generated class
    public GameObject leftmostBorder;
    public GameObject rightmostBorder;
    public GameObject bottommostBorder;
    public GameObject topmostBorder;
    public float deltaMovement = 0.1f;
    Rigidbody2D playerRB;
    public float forceToApply = 1.5f;
    public static PlayerManager Instance;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerRB = GetComponent<Rigidbody2D>();

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

    // Update is called once per frame
    void Update()
    {

    }
    /*
    public void FixedUpdate()
    {
        bool isPressed = false;
        isPressed = controls.Move.MoveLeft.IsPressed();//.Gameplay.GameStart.IsPressed();
        if (isPressed)
        {
            Debug.Log("PlayerManager: MoveLeft pressed!");
            Vector3 pos, borderPos;
            Quaternion rotation, borderRotation;
            transform.GetPositionAndRotation(out pos, out rotation);
            leftmostBorder.transform.GetPositionAndRotation(out borderPos, out borderRotation);
            Debug.Log($"pos: {pos}, borderPos: {borderPos}");
            //Incremental Movements
            if (false)
            {
                pos.x -= deltaMovement;
                if (pos.x < borderPos.x)
                {
                    pos.x = borderPos.x;
                }
                transform.position = pos;

            }
            else
            {

                playerRB.linearVelocityX = -forceToApply;
                //playerRB.linearVelocityX = -deltaMovement;
                //playerRB.AddForceX(-deltaMovement, ForceMode2D.Impulse);
                Vector2 newDir = new Vector2((-1.0f * forceToApply * Time.deltaTime), 0.0f).normalized;
                playerRB.AddForce(newDir, ForceMode2D.Force);
                Debug.Log($"PlayerManager:PlayerDirection: {newDir}, Velocity: {playerRB.linearVelocityX}");
            }
        }
        isPressed = controls.Move.MoveRight.IsPressed();
        if (isPressed)
        {
            playerRB.linearVelocityX = forceToApply;
            //playerRB.linearVelocityX = -deltaMovement;
            //playerRB.AddForceX(-deltaMovement, ForceMode2D.Impulse);
            Vector2 newDir = new Vector2((1.0f * forceToApply * Time.deltaTime), 0.0f).normalized;
            playerRB.AddForce(newDir, ForceMode2D.Force);
            Debug.Log($"PlayerManager:PlayerDirection: {newDir}, Velocity: {playerRB.linearVelocityX}");
        }

    }*/
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log($"Player collision! Main Object: {collision.gameObject.name} Other Object: {collision.otherRigidbody.name}");
        if(collision.gameObject.name == "DoorSwitchExit")
        {
            //Disable parent containing switch and door blocker
            //collision.gameObject.transform.parent.gameObject.SetActive(false);
            //Get the child particle system
            //collision.gameObject.
            //Set PS active
            //Destroy parent object after explosion
            foreach (Transform childTransform in collision.gameObject.transform)
            {
                GameObject childObjectPS = childTransform.gameObject;
                //Debug.Log("Child name: " + childObject.name, childObject);
                // You can now perform actions on the child GameObject
                childObjectPS.SetActive(true);
            }
            //ToDo:get time from particle system
            Destroy(collision.gameObject.transform.parent.gameObject, 4.0f);
        }
    }
}
