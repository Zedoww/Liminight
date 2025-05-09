using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class SimpleVideoTest : MonoBehaviour
{
    public VideoClip testVideoClip;
    public RawImage displayImage;
    public bool playOnStart = true;
    
    private VideoPlayer videoPlayer;
    private AudioSource audioSource;
    
    void Start()
    {
        if (testVideoClip == null || displayImage == null)
        {
            Debug.LogError("Missing video clip or display image!");
            return;
        }
        
        // Create components
        videoPlayer = gameObject.AddComponent<VideoPlayer>();
        audioSource = gameObject.AddComponent<AudioSource>();
        
        // Basic setup
        videoPlayer.clip = testVideoClip;
        videoPlayer.renderMode = VideoRenderMode.RenderTexture;
        videoPlayer.targetTexture = new RenderTexture(1920, 1080, 24);
        videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
        videoPlayer.SetTargetAudioSource(0, audioSource);
        
        // Connect to display
        displayImage.texture = videoPlayer.targetTexture;
        displayImage.color = Color.white;
        
        // Ensure visible
        displayImage.gameObject.SetActive(true);
        
        Debug.Log("Video player set up");
        
        if (playOnStart)
        {
            videoPlayer.Play();
            Debug.Log("Video started playing");
        }
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (videoPlayer.isPlaying)
            {
                videoPlayer.Pause();
                Debug.Log("Video paused");
            }
            else
            {
                videoPlayer.Play();
                Debug.Log("Video playing");
            }
        }
    }
} 