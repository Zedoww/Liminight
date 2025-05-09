using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoPlayerTest : MonoBehaviour
{
    public VideoClip videoClip;
    public RawImage display;
    public bool playOnStart = true;
    
    private VideoPlayer videoPlayer;
    private AudioSource audioSource;
    
    void Start()
    {
        // Create video player component
        videoPlayer = gameObject.AddComponent<VideoPlayer>();
        
        // Create audio source for sound
        audioSource = gameObject.AddComponent<AudioSource>();
        
        // Basic setup with maximum compatibility
        videoPlayer.source = VideoSource.VideoClip;
        videoPlayer.clip = videoClip;
        videoPlayer.playOnAwake = false;
        videoPlayer.waitForFirstFrame = true;
        videoPlayer.skipOnDrop = false; // Don't skip frames for smoother playback
        videoPlayer.isLooping = false;
        
        // Configure audio explicitly
        videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
        videoPlayer.SetTargetAudioSource(0, audioSource);
        videoPlayer.controlledAudioTrackCount = 1;
        audioSource.volume = 1.0f;
        
        // Set output to RawImage if available
        if (display != null)
        {
            // Make sure RawImage is visible and properly sized
            display.gameObject.SetActive(true);
            display.color = Color.white; // Full opacity
            
            // Create a render texture with correct aspect ratio
            int width = Mathf.Max(Screen.width, 1280);
            int height = Mathf.Max(Screen.height, 720);
            RenderTexture renderTexture = new RenderTexture(width, height, 24);
            renderTexture.Create();
            
            // Set up the videoPlayer output
            videoPlayer.renderMode = VideoRenderMode.RenderTexture;
            videoPlayer.targetTexture = renderTexture;
            
            // Assign texture to display
            display.texture = renderTexture;
            
            // Ensure RawImage is in front of everything
            Canvas canvas = display.GetComponentInParent<Canvas>();
            if (canvas != null)
            {
                canvas.sortingOrder = 999; // Put it on top of everything
                // Force screen space overlay mode
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            }
        }
        else
        {
            Debug.LogError("No RawImage assigned! Video won't be visible.");
            // Try to render on camera background as fallback
            videoPlayer.renderMode = VideoRenderMode.CameraFarPlane;
            videoPlayer.targetCamera = Camera.main;
        }
        
        // Register event handling when video is prepared
        videoPlayer.prepareCompleted += PrepareCompleted;
        
        // Log finished event
        videoPlayer.loopPointReached += (vp) => { Debug.Log("Video playback finished!"); };
        
        if (playOnStart)
        {
            // Prepare and then play
            videoPlayer.Prepare();
            Debug.Log("Video is preparing to play...");
        }
    }
    
    void PrepareCompleted(VideoPlayer vp)
    {
        Debug.Log("Video is prepared and ready to play!");
        vp.Play();
        Debug.Log("Video has started playing! Dimensions: " + vp.texture.width + "x" + vp.texture.height);
        
        // Additional check to ensure video is visible
        if (display != null && display.texture != null)
        {
            Debug.Log("RawImage has texture assigned: " + display.texture);
            Debug.Log("RawImage dimensions: " + display.rectTransform.rect.width + "x" + display.rectTransform.rect.height);
        }
    }
    
    void Update()
    {
        // Show playback progress
        if (videoPlayer.isPlaying)
        {
            // Log progress every second
            if (Time.frameCount % 60 == 0)
            {
                Debug.Log($"Video playback: {videoPlayer.time:F2}s / {videoPlayer.length:F2}s");
                
                // Check if texture is valid
                if (videoPlayer.texture != null)
                {
                    if (display != null && display.texture != videoPlayer.targetTexture)
                    {
                        Debug.Log("Fixing texture assignment");
                        display.texture = videoPlayer.targetTexture;
                    }
                }
            }
        }
        
        // Space to toggle play/pause
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
                Debug.Log("Video resumed");
            }
        }
    }
} 