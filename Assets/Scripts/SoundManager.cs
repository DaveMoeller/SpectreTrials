using UnityEditor.Experimental.GraphView;
using UnityEngine;
using System;
public enum SoundType
{
    POINT,
    CRICKETS,
    DOGBARK,
    PLAYERWALK,
    PLAYERRUN,
    ENDGAME,
    ENDGAMEWIN,
    ENDGAMELOOSE,
    ENEMY
}
[RequireComponent(typeof(AudioSource)), ExecuteInEditMode]
public class SoundManager : MonoBehaviour
{
    [Tooltip("Make the element num ber the same as the enum SoundType length.")]
    [SerializeField] private SoundList[] soundList;
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
                instance.audioSource.PlayOneShot(instance.soundList[(int)soundType].sound, instance.soundList[(int)soundType].volume);
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
                instance.audioSource.PlayOneShot(instance.soundList[(int)soundType].sound, instance.soundList[(int)soundType].volume);
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
#if UNITY_EDITOR
    private void OnEnable()
    {
        string[] names = Enum.GetNames(typeof(SoundType));
        Debug.Log($"names= {names}, length={names.Length}");
        Array.Resize(ref soundList, names.Length);
        for (int i = 0; i < soundList.Length; i++)
        {
            Debug.Log($"names[{i}]= {names[i]}");
            soundList[i].name = names[i];
        }
    }

#endif

}

[Serializable]
public struct SoundList
{
    [HideInInspector] public string name;
    [SerializeField] public AudioClip sound;
    [SerializeField,Range(0.0f,1.0f)] public float volume;
}