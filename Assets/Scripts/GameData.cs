using UnityEngine;

public class GameData : MonoBehaviour
{
    public static GameData Instance;
    private static int m_highScore = 0;
    private static int m_level = 0;
    public int HighScore
    {
        get { return m_highScore; }
        set { m_highScore = value; }
    }
    public int Level
    {
        get { return m_level; }
        set { m_level = value; }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(Instance.gameObject);
        }

    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
