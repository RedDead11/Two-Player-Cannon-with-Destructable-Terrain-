using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    public AudioSource musicSource;
    private Button muteButton;
    private bool isMuted;

    private void Awake()
    {
        // Singleton pattern to persist across scenes
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Subscribe to scene load event
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Retrieve the mute state from PlayerPrefs
        isMuted = PlayerPrefs.GetInt("IsMuted", 0) == 1;
        UpdateMuteState();
    }

    private void OnDestroy()
    {
        // Unsubscribe from scene load event
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Find and assign the mute button in the new scene
        muteButton = GameObject.Find("Mute Button")?.GetComponent<Button>();
        if (muteButton != null)
        {
            muteButton.onClick.RemoveAllListeners();
            muteButton.onClick.AddListener(ToggleMute);
            UpdateMuteState(); // Update button appearance if needed
        }
    }

    private void UpdateMuteState()
    {
        musicSource.mute = isMuted;
    }

    private void ToggleMute()
    {
        isMuted = !isMuted;
        UpdateMuteState();
        PlayerPrefs.SetInt("IsMuted", isMuted ? 1 : 0);
        PlayerPrefs.Save();
    }
}
