using UnityEditor.Experimental.GraphView;
using UnityEngine;

public enum SoundType
{
    POINT,
    CRICKETS,
    DOGBARK,
    PLAYERWALK,
    PLAYERRUN,
    ENDGAME
}
[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
    [Tooltip("Make the element num ber the same as the enum SoundType length.")]
    [SerializeField] private AudioClip[] soundList;
    public static SoundManager instance;
    private AudioSource audioSource;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            return;
        }
    }

    public static void PlaySound(SoundType soundType)
    {
        Debug.Log($"SoundType: {(int)soundType}");
        if (instance != null)
        {
            if (instance.audioSource != null)
            {
                instance.audioSource.PlayOneShot(instance.soundList[(int)soundType], instance.audioSource.volume);
            }
            else
            {
                Debug.LogError("instance.audioSource instance is null");
            }
        }
        else
        {
            Debug.LogError("SoundManager instance is null");
        }
    }
    public static void PlaySound(SoundType soundType, float volume)
    {
        Debug.Log($"SoundType: {(int)soundType}");
        if (instance != null)
        {
            if (instance.audioSource != null)
            {
                instance.audioSource.PlayOneShot(instance.soundList[(int)soundType], volume);
            }
            else
            {
                Debug.LogError("instance.audioSource instance is null");
            }
        }
        else
        {
            Debug.LogError("SoundManager instance is null");
        }
    }
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

}
