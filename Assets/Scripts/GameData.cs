using UnityEngine;
public enum GameLevels
{
    BEGINNER,
    INTERMEDIATE,
    EXPERT,
    QUITTER
}
public class GameData : MonoBehaviour
{
    public static GameData Instance;
    private static int m_highScore = 0;
    private static GameLevels m_level = GameLevels.BEGINNER;
    public int HighScore
    {
        get { return m_highScore; }
        set { m_highScore = value; }
    }
    public GameLevels Level
    {
        get => m_level;
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
   
}
