using UnityEngine;

/// <summary>
/// Gestionnaire central d'entrées qui peut être activé/désactivé selon les besoins
/// pour empêcher les interactions avec le jeu lorsque les menus sont ouverts.
/// </summary>
public class InputManager : MonoBehaviour
{
    private static InputManager _instance;
    public static InputManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<InputManager>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("InputManager");
                    _instance = go.AddComponent<InputManager>();
                    DontDestroyOnLoad(go);
                }
            }
            return _instance;
        }
    }

    // État des entrées
    private bool _inputEnabled = true;

    // Références aux scripts qui utilisent des entrées
    private PlayerController[] playerControllers;
    private InteractBehavior[] interactBehaviors;
    private DoorOpener[] doorOpeners;
    private CardReader[] cardReaders;

    // Propriété publique pour activer/désactiver toutes les entrées utilisateur
    public bool InputEnabled
    {
        get => _inputEnabled;
        set
        {
            if (_inputEnabled != value)
            {
                _inputEnabled = value;
                UpdateAllInputStates();
            }
        }
    }

    private void Awake()
    {
        // Singleton pattern
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        // Configuration initiale
        _inputEnabled = true;
        
        // Trouver toutes les références nécessaires
        FindAllInputComponents();
    }
    
    public void FindAllInputComponents()
    {
        // Trouver tous les composants qui gèrent des entrées utilisateur
        playerControllers = FindObjectsByType<PlayerController>(FindObjectsSortMode.None);
        interactBehaviors = FindObjectsByType<InteractBehavior>(FindObjectsSortMode.None);
        doorOpeners = FindObjectsByType<DoorOpener>(FindObjectsSortMode.None);
        cardReaders = FindObjectsByType<CardReader>(FindObjectsSortMode.None);
    }

    // Méthode appelée lorsque l'état des entrées change
    private void UpdateAllInputStates()
    {
        // Mettre à jour tous les composants trouvés
        if (playerControllers != null)
        {
            foreach (var controller in playerControllers)
            {
                if (controller != null)
                    controller.enabled = _inputEnabled;
            }
        }
        
        if (interactBehaviors != null)
        {
            foreach (var behavior in interactBehaviors)
            {
                if (behavior != null)
                    behavior.enabled = _inputEnabled;
            }
        }
        
        if (doorOpeners != null)
        {
            foreach (var door in doorOpeners)
            {
                if (door != null)
                    door.EnableInput(_inputEnabled);
            }
        }
        
        if (cardReaders != null)
        {
            foreach (var reader in cardReaders)
            {
                if (reader != null)
                    reader.EnableInput(_inputEnabled);
            }
        }
    }
    
    // Méthodes publiques pour activer/désactiver les entrées
    public void DisableAllInputs()
    {
        InputEnabled = false;
    }
    
    public void EnableAllInputs()
    {
        InputEnabled = true;
    }
} 