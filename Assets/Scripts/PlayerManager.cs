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
            /*
             DontDestroyOnLoad only works for root GameObjects or components on root GameObjects.
            UnityEngine.Object:DontDestroyOnLoad (UnityEngine.Object)
            PlayerManager:Awake () (at Assets/Scripts/PlayerManager.cs:31)
            */
 //           DontDestroyOnLoad(Instance); // same as GameObject

        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log($"Player collision! Main Object: {collision.gameObject.name} Other Object: {collision.otherRigidbody.name}");
        if (collision.gameObject.name == "DoorSwitchExit")
        {
            //Disable parent containing switch and door blocker
            //collision.gameObject.transform.parent.gameObject.SetActive(false);
            //Get the child particle system
            //collision.gameObject.
            //Set PS active
            //Destroy parent object after explosion
            float durationPS = 2.0f;
            foreach (Transform childTransform in collision.gameObject.transform)
            {
                GameObject childObject = childTransform.gameObject;
                ParticleSystem childPS = childObject.GetComponent<ParticleSystem>();
                var mainModule = childPS.main;
                durationPS = mainModule.duration;
                mainModule.playOnAwake = true;
                childObject.SetActive(true);
            }
            //ToDo:get time from particle system
            Destroy(collision.gameObject.transform.parent.gameObject, durationPS);
        }
        if (collision.gameObject.CompareTag("Key"))
        {
            Destroy(collision.gameObject.transform.parent.gameObject, 0.5f);

        }
        if (collision.gameObject.name.Contains("LightSwitch"))
        {
            MainManager.Instance.setObjectsVisible(true);
            Destroy(collision.gameObject, 0.5f);
        }
        if (collision.gameObject.name.Contains("WallPiece"))
        {
            //Destroy(collision.gameObject.transform.parent.gameObject, 0.5f);
            //Change the color
            //collision.gameObject
            SpriteRenderer renderer = collision.gameObject.GetComponent<SpriteRenderer>();
            if (renderer != null)
            {
                 renderer.color = Color.cornflowerBlue;
            }
        }
    }

}
