using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using System.Collections;

public class MainMenuManager : MonoBehaviour
{
    /* ----------  Inspector ---------- */
    [Header("Menu – Boutons")]
    public Button startButton;
    public Button settingsButton;
    public Button quitButton;

    [Header("Menu – Panels")]
    public GameObject mainMenuPanel;
    public GameObject settingsPanel;

    [Header("Transition")]
    [Tooltip("Durée du fondu du menu (secondes)")]
    public float fadeTime = 1f;
    public CanvasGroup canvasGroup;

    [Header("Cinematic")]
    public VideoClip introVideo;
    public RawImage videoDisplay;             // ← laissé public pour que SettingsPanel y accède
    public bool playIntroBeforeGameStart = true;
    public bool canSkipVideo = true;
    [Range(0f, 1f)] public float defaultVideoVolume = 0.75f;

    [Header("Nom de la scène de jeu")]
    public string gameSceneName = "TestSceneBlocksAndTunels";

    /* ----------  privés ---------- */
    VideoPlayer videoPlayer;
    AudioSource videoAudioSource;
    RenderTexture videoRenderTexture;
    bool isPlayingVideo;

    /* ----------  Unity ---------- */
    void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }

        startButton?.onClick.AddListener(StartGame);
        settingsButton?.onClick.AddListener(OpenSettings);
        quitButton?.onClick.AddListener(QuitGame);

        settingsPanel?.SetActive(false);
        mainMenuPanel?.SetActive(true);
        videoDisplay?.gameObject.SetActive(false);
    }

    void Update()
    {
        if (isPlayingVideo && canSkipVideo &&
            (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Space)))
        {
            SkipVideo();
        }
    }

    /* ----------  Menu principal ---------- */
    public void StartGame()
    {
        if (playIntroBeforeGameStart && introVideo != null)
            StartCoroutine(PlayIntroSequence());
        else
            StartCoroutine(FadeAndLoadScene(gameSceneName));
    }

    public void OpenSettings()
    {
        if (settingsPanel == null) return;

        settingsPanel.SetActive(true);
        mainMenuPanel?.SetActive(false);

        // Cache tout sauf le panneau Settings
        foreach (Transform child in transform)
            if (child.gameObject != settingsPanel && !child.IsChildOf(settingsPanel.transform))
                child.gameObject.SetActive(false);
    }

    public void CloseSettings()
    {
        if (settingsPanel == null) return;

        settingsPanel.SetActive(false);
        mainMenuPanel?.SetActive(true);

        // Ré-active tous les enfants SAUF settingsPanel et videoDisplay
        foreach (Transform child in transform)
        {
            if (child == settingsPanel.transform ||
                child.IsChildOf(settingsPanel.transform) ||
                child.gameObject == videoDisplay?.gameObject)
                continue;

            child.gameObject.SetActive(true);
        }

        // Assure que videoDisplay reste bien désactivé
        if (videoDisplay != null)
        {
            videoDisplay.texture = null;
            videoDisplay.gameObject.SetActive(false);
        }

        ToggleMenuButtons(true);
    }


    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    /* ----------  Intro vidéo ---------- */
    IEnumerator PlayIntroSequence()
    {
        yield return StartCoroutine(FadeCanvas(1f, 0f));
        HideAllUI();

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }

        SetupVideoPlayer();
        videoPlayer.Prepare();
        while (!videoPlayer.isPrepared) yield return null;

        videoDisplay.gameObject.SetActive(true);
        videoPlayer.Play();
        isPlayingVideo = true;

        while (isPlayingVideo) yield return null;

        SceneManager.LoadScene(gameSceneName);
    }

    void SetupVideoPlayer()
    {
        if (videoPlayer != null) Destroy(videoPlayer);
        if (videoAudioSource != null) Destroy(videoAudioSource);
        if (videoRenderTexture != null) { videoRenderTexture.Release(); Destroy(videoRenderTexture); }

        videoPlayer = gameObject.AddComponent<VideoPlayer>();
        videoAudioSource = gameObject.AddComponent<AudioSource>();

        videoPlayer.source = VideoSource.VideoClip;
        videoPlayer.clip = introVideo;
        videoPlayer.playOnAwake = false;
        videoPlayer.isLooping = false;

        videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
        videoPlayer.SetTargetAudioSource(0, videoAudioSource);
        videoPlayer.EnableAudioTrack(0, true);
        videoPlayer.SetDirectAudioVolume(0, defaultVideoVolume);

        int w = Mathf.Max(Screen.width, 1280);
        int h = Mathf.Max(Screen.height, 720);
        videoRenderTexture = new RenderTexture(w, h, 24);
        videoRenderTexture.Create();

        videoPlayer.renderMode = VideoRenderMode.RenderTexture;
        videoPlayer.targetTexture = videoRenderTexture;

        videoDisplay.texture = videoRenderTexture;
        videoDisplay.color = Color.white;

        videoAudioSource.playOnAwake = false;
        videoAudioSource.volume = defaultVideoVolume;

        videoPlayer.loopPointReached += _ => isPlayingVideo = false;
    }

    void SkipVideo()
    {
        if (videoPlayer != null && videoPlayer.isPlaying)
        {
            videoPlayer.Stop();
            isPlayingVideo = false;
        }
    }

    /* ----------  Helpers ---------- */
    IEnumerator FadeAndLoadScene(string scene)
    {
        yield return StartCoroutine(FadeCanvas(1f, 0f));
        SceneManager.LoadScene(scene);
    }

    IEnumerator FadeCanvas(float from, float to)
    {
        if (canvasGroup == null) yield break;

        float t = 0f;
        while (t < fadeTime)
        {
            canvasGroup.alpha = Mathf.Lerp(from, to, t / fadeTime);
            t += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = to;
    }

    void HideAllUI()
    {
        mainMenuPanel?.SetActive(false);
        settingsPanel?.SetActive(false);

        // Désactive aussi le RawImage de la vidéo
        if (videoDisplay != null)
        {
            videoDisplay.texture = null;
            videoDisplay.gameObject.SetActive(false);
        }

        ToggleMenuButtons(false);
    }

    void ToggleMenuButtons(bool state)
    {
        startButton?.gameObject.SetActive(state);
        settingsButton?.gameObject.SetActive(state);
        quitButton?.gameObject.SetActive(state);
    }

    /* ----------  Accès public pour SettingsPanel ---------- */
    public bool VideoPlayerExists() => videoPlayer != null;

    public void SetVideoPlayerVolume(float volume)
    {
        videoPlayer?.SetDirectAudioVolume(0, volume);
    }
}
