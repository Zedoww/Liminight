using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Vitesses")]
    public float walkSpeed = 5f;
    public float sprintSpeed = 10f;    // Vitesse en courant (touche Alt)
    public float crouchSpeed = 2.5f;   // Vitesse en s'accroupissant (touche Shift)
    public float jumpForce = 5f;
    public float gravity = 9.81f;

    [Header("Hauteurs et Centers")]
    public float standHeight = 2f;      // Hauteur du CharacterController debout
    public float crouchHeight = 1f;     // Hauteur du CharacterController accroupi
    public Vector3 standCenter = new Vector3(0f, 1f, 0f);   // Centre capsule debout
    public Vector3 crouchCenter = new Vector3(0f, 0.5f, 0f); // Centre capsule accroupi

    [Header("Transition Accroupi/Debout")]
    [Tooltip("Durée (en secondes) pour la transition accroupi ↔ debout.")]
    public float crouchTransitionTime = 0.3f;

    [Header("Caméra et sensibilité")]
    public float mouseSensitivity = 100f;
    public Camera playerCamera;

    // Composants
    private CharacterController characterController;

    // Déplacements
    private Vector3 moveDirection = Vector3.zero;

    // États du joueur
    private bool isCrouching = false;   // Sert pour la vitesse (crouchSpeed)
    private bool isSprinting = false;

    // Gestion de la transition
    private bool isTransitioning = false;   // Vrai quand on est en train de modifier la hauteur
    private float transitionStartTime;      // Moment où a débuté la transition
    private float fromHeight;               // Hauteur de départ pour la transition
    private float toHeight;                 // Hauteur d'arrivée (crouch ou stand)
    private Vector3 fromCenter;             // Center de départ
    private Vector3 toCenter;               // Center d'arrivée

    void Start()
    {
        characterController = GetComponent<CharacterController>();

        // Verrouille le curseur
        Cursor.lockState = CursorLockMode.Locked;

        // Caméra par défaut si non assignée
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }

        // Place le joueur un peu au-dessus du sol
        Vector3 startPos = transform.position;
        startPos.y = 1f;
        transform.position = startPos;

        // Configuration initiale : debout
        characterController.height = standHeight;
        characterController.center = standCenter;
    }

    void Update()
    {
        HandleCrouchInput();
        HandleSprint();
        UpdateCrouchTransition(); // Anime la hauteur si une transition est en cours
        MovePlayer();
        RotateView();
    }

    /// <summary>
    /// Détecte appui/relâche de la touche Shift pour lancer une transition (accroupi ↔ debout).
    /// </summary>
    void HandleCrouchInput()
    {
        // Si on appuie sur Shift alors qu'on n'est pas accroupi -> transition vers accroupi
        if (Input.GetKeyDown(KeyCode.C) && !isCrouching)
        {
            isCrouching = true;
            StartCrouchTransition(true);  // Vers accroupi
        }
        // Si on relâche Shift alors qu'on est accroupi -> transition vers debout
        else if (Input.GetKeyUp(KeyCode.C) && isCrouching)
        {
            isCrouching = false;
            StartCrouchTransition(false); // Vers debout
        }
    }

    /// <summary>
    /// Définit isSprinting en fonction de la touche Alt (et du fait qu'on ne soit pas accroupi).
    /// </summary>
    void HandleSprint()
    {
        isSprinting = Input.GetKey(KeyCode.LeftShift) && !isCrouching;
    }

    /// <summary>
    /// Initialise les paramètres de la transition (temps, from->to) pour la hauteur et le center.
    /// </summary>
    /// <param name="goingDown">true si on descend (accroupi), false si on se relève</param>
    void StartCrouchTransition(bool goingDown)
    {
        isTransitioning = true;
        transitionStartTime = Time.time;

        fromHeight = characterController.height;
        fromCenter = characterController.center;

        toHeight = goingDown ? crouchHeight : standHeight;
        toCenter = goingDown ? crouchCenter : standCenter;
    }

    /// <summary>
    /// Effectue la transition (Lerp) si isTransitioning est vrai, en prenant en compte crouchTransitionTime.
    /// </summary>
    void UpdateCrouchTransition()
    {
        if (!isTransitioning) return;

        float elapsed = Time.time - transitionStartTime;
        float t = Mathf.Clamp01(elapsed / crouchTransitionTime);

        // Interpolation fluide
        float newHeight = Mathf.Lerp(fromHeight, toHeight, t);
        Vector3 newCenter = Vector3.Lerp(fromCenter, toCenter, t);

        characterController.height = newHeight;
        characterController.center = newCenter;

        // Transition terminée ?
        if (t >= 1f)
        {
            isTransitioning = false;
        }
    }

    /// <summary>
    /// Gère le déplacement (Walk, Crouch, Sprint) et le saut.
    /// </summary>
    void MovePlayer()
    {
        // Si on est au sol
        if (characterController.isGrounded)
        {
            // Petite force vers le bas pour rester collé
            moveDirection.y = -0.5f;

            // Saut
            if (Input.GetButtonDown("Jump"))
            {
                moveDirection.y = jumpForce;
            }
        }

        // Récupération des inputs de déplacement
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        Vector3 move = transform.right * moveHorizontal + transform.forward * moveVertical;

        // Choix de la vitesse
        float currentSpeed = walkSpeed;
        if (isCrouching)
        {
            currentSpeed = crouchSpeed;
        }
        else if (isSprinting)
        {
            currentSpeed = sprintSpeed;
        }

        // Applique la vitesse sur XZ
        moveDirection.x = move.x * currentSpeed;
        moveDirection.z = move.z * currentSpeed;

        // Gravité
        moveDirection.y -= gravity * Time.deltaTime;

        // Déplacement final via CharacterController
        characterController.Move(moveDirection * Time.deltaTime);
    }

    /// <summary>
    /// Gère la rotation de la vue : horizontal = joueur, vertical = caméra.
    /// </summary>
    void RotateView()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Rotation horizontale
        transform.Rotate(Vector3.up * mouseX);

        // Rotation verticale de la caméra
        Vector3 camRotation = playerCamera.transform.localEulerAngles;
        float newRotationX = camRotation.x - mouseY;
        if (newRotationX > 180f) newRotationX -= 360f;
        newRotationX = Mathf.Clamp(newRotationX, -90f, 90f);

        playerCamera.transform.localEulerAngles = new Vector3(newRotationX, 0f, 0f);
    }
}