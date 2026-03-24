using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public GameObject player;
    [Range(0.1f, 5.0f)]
    public float enemySpeed = .25f;
    public static EnemyManager Instance;
    Rigidbody2D playerRB;
    Rigidbody2D enemyRB;
    public void Awake()
    {
        //Debug.Log("EnemyManager gameObject.name: " + gameObject.name);
        if (Instance != null)
        {
            return;
        }
        else
        {
            Instance = this;
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerRB = player.GetComponent<Rigidbody2D>();
        enemyRB = Instance.GetComponent<Rigidbody2D>();

        //MoveEnemy();
    }
    public void MoveEnemy()
    {
        //Get location of enemy and player
        Vector2 playerPos = player.transform.position;
        Vector2 enemyPos = transform.position;
        Vector2 dir = calculateMoveVector(playerPos, enemyPos);

        //Calculate vector
        Debug.Log($"playerPos:{playerPos}");
        Debug.Log($"enemyPos:{enemyPos}");
        Debug.Log($"Direction:{dir}");

        //Move towards player at speed and delta time
        //AddForce
        //Instance.enemyRB.AddForce((enemySpeed * Time.fixedDeltaTime * dir), ForceMode2D.Force);
        //Instance.enemyRB.linearVelocity = (enemySpeed * Time.fixedDeltaTime * dir);
        Instance.enemyRB.linearVelocity = (enemySpeed * dir);
    }
    private Vector2 calculateMoveVector(Vector2 pos, Vector2 pos1)
    {
        return (pos - pos1).normalized;
    }
    // Update is called once per frame
    void Update()
    {

    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log($"Enemy collision! Main Object: {collision.gameObject.name} Other Object: {collision.otherRigidbody.name}");
    }
    private void FixedUpdate()
    {
        MoveEnemy();
    }
}
