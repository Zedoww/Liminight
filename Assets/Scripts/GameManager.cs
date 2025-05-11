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
        Debug.Log("GameManager: Initialisation du jeu");
        
        // S'assurer que les inputs sont activés au démarrage
        if (inputManager != null)
        {
            inputManager.EnableAllInputs();
            inputManager.FindAllInputComponents(); // Trouver tous les composants interactifs
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
} 