using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Debug.Log($"Player collision! Main Object: {collision.gameObject.name} Other Object: {collision.otherRigidbody.name}");
        if (collision.gameObject.name == "DoorSwitchExit")
        {
            //Disable parent containing switch and door blocker
            //collision.gameObject.transform.parent.gameObject.SetActive(false);
            //Get the child particle system
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
            //Remove container
            Destroy(collision.gameObject.transform.parent.gameObject, 0.5f);


        }
        if (collision.gameObject.CompareTag("Enemy"))
        {
            //
            //Debug.Log($"Collision with {collision.gameObject.name} Object!");
            //Update player location to starting location
            MainManager.Instance.MovePlayerToNewStartingLocation();

        }

        if (collision.gameObject.CompareTag("GuardDog"))
        {
            SoundManager.PlaySound(SoundType.DOGBARK);

            Destroy(collision.gameObject, 1.0f);
            MainManager.Instance.EndGame();

        }
        if (collision.gameObject.CompareTag("PointObject"))
        {
            MainManager.Instance.IncrementScore(1);
            ParticleSystem ps = collision.gameObject.GetComponent<ParticleSystem>();
            var mainModule = ps.main;
            SpriteRenderer sr = collision.gameObject.GetComponent<SpriteRenderer>();
            float durationPS = mainModule.duration;
            sr.enabled = false;
            BoxCollider2D collider = collision.gameObject.GetComponent<BoxCollider2D>();
            collider.enabled = false;
            ps.Play();
            Destroy(collision.gameObject, durationPS);

        }
        if (collision.gameObject.name.Contains("LightSwitch"))
        {
            MainManager.Instance.SetObjectsVisible(true);
            MainManager.Instance.SetPointObjectsActive(true);
            Destroy(collision.gameObject, 0.1f);
        }
        if (collision.gameObject.name.Contains("WallPiece"))
        {
            //Change the color
            //collision.gameObject
            if (collision.gameObject.TryGetComponent<SpriteRenderer>(out var renderer))
            {
                renderer.color = Color.cornflowerBlue;
                renderer.enabled = true;
            }
            else
            {
                Debug.LogError("renderer is null!");
            }
        }
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("PointObject"))
        {
            MainManager.Instance.IncrementScore(1);
            ParticleSystem ps = collision.gameObject.GetComponent<ParticleSystem>();
            var mainModule = ps.main;
            SpriteRenderer sr = collision.gameObject.GetComponent<SpriteRenderer>();
            float durationPS = mainModule.duration;
            sr.enabled = false;
            BoxCollider2D collider = collision.gameObject.GetComponent<BoxCollider2D>();
            collider.enabled = false;
            ps.Play();
            //SoundManager.PlaySound(MainManager.Instance.pointSound, MainManager.Instance.pointVolume);
            SoundManager.PlaySound(SoundType.POINT);
            Destroy(collision.gameObject, durationPS);

        }
        if (collision.gameObject.CompareTag("Enemy"))
        {
            //Debug.Log("Collision with Enemy!");
            //Update player location to starting location
            MainManager.Instance.MovePlayerToNewStartingLocation();

        }
    }
}
