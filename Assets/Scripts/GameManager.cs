using UnityEngine;

/// <summary>
/// Script principal qui initialise et gère les composants globaux du jeu.
/// </summary>
public class GameManager : MonoBehaviour
{
    // Singleton
    public static GameManager Instance { get; private set; }
    
    // Référence à l'InputManager
    private InputManager inputManager;
    // Référence à l'AudioManager
    private BackgroundAudioManager backgroundAudioManager;
    
    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        // Initialiser l'InputManager
        inputManager = InputManager.Instance;
    }
    
    private void Start()
    {
        // Initialisation du jeu
        Debug.Log("GameManager: Game initialization");
        
        // S'assurer que les inputs sont activés au démarrage
        if (inputManager != null)
        {
            inputManager.EnableAllInputs();
            inputManager.FindAllInputComponents(); // Trouver tous les composants interactifs
        }
        
        // Obtenir la référence à l'AudioManager s'il existe
        backgroundAudioManager = BackgroundAudioManager.Instance;
        if (backgroundAudioManager == null)
        {
            // Si l'AudioManager n'existe pas, créer un nouvel objet et l'initialiser
            GameObject audioManagerObj = new GameObject("BackgroundAudioManager");
            backgroundAudioManager = audioManagerObj.AddComponent<BackgroundAudioManager>();
        }
    }
    
    // Méthode utilisée pour la réinitialisation des états lors des changements de scène
    public void ResetGameState()
    {
        // Réactiver les inputs
        if (inputManager != null)
        {
            inputManager.EnableAllInputs();
            inputManager.FindAllInputComponents(); // Mettre à jour les références
        }
        
        // Débloquer le curseur et restaurer le timeScale
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1f;
    }
    
    /// <summary>
    /// Permet d'activer/désactiver la musique de fond
    /// </summary>
    public void ToggleBackgroundMusic()
    {
        if (backgroundAudioManager != null)
        {
            backgroundAudioManager.ToggleAudio();
        }
    }
    
    /// <summary>
    /// Change la musique de fond en fonction de l'index fourni
    /// </summary>
    public void ChangeBackgroundMusic(int trackIndex)
    {
        if (backgroundAudioManager != null)
        {
            if (trackIndex < 0)
            {
                backgroundAudioManager.PlayDefaultTrack();
            }
            else
            {
                backgroundAudioManager.PlayTrackByIndex(trackIndex);
            }
        }
    }
    
    /// <summary>
    /// Définit la durée des transitions (fade in/out) pour la musique de fond
    /// </summary>
    /// <param name="fadeInDuration">Durée du fade in en secondes</param>
    /// <param name="fadeOutDuration">Durée du fade out en secondes</param>
    public void SetMusicFadeDurations(float fadeInDuration, float fadeOutDuration)
    {
        if (backgroundAudioManager != null)
        {
            backgroundAudioManager.SetFadeDurations(fadeInDuration, fadeOutDuration);
        }
    }
    
    /// <summary>
    /// Définit le volume de la musique de fond
    /// </summary>
    /// <param name="volume">Volume entre 0 et 1</param>
    public void SetMusicVolume(float volume)
    {
        if (backgroundAudioManager != null)
        {
            backgroundAudioManager.SetVolume(volume);
        }
    }
} 