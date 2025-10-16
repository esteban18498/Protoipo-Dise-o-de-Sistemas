using UnityEngine;

public sealed class AudioBootstrapper : MonoBehaviour
{
    [SerializeField] private AudioManager audioManagerPrefab;

    void Awake()
    {
        if (AudioManager.Instance == null)
        {
            var audioManager = Instantiate(audioManagerPrefab);
            audioManager.name = "AudioManager"; // keep scene clean
            DontDestroyOnLoad(audioManager.gameObject);
        }
        // else: one already exists (e.g., returned from gameplay), do nothing
    }
}
