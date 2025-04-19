using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Vitesses")]
    public float walkSpeed = 5f;
    public float sprintSpeed = 10f;    // Vitesse en courant (touche LeftShift)
    public float crouchSpeed = 2.5f;   // Vitesse en s'accroupissant (touche C)
    public float jumpForce = 5f;
    public float gravity = 9.81f;

    [Header("Hauteurs et Centers")]
    public float standHeight = 2f;      // Hauteur du CharacterController debout
    public float crouchHeight = 1f;     // Hauteur du CharacterController accroupi
    public Vector3 standCenter = new Vector3(0f, 1f, 0f);
    public Vector3 crouchCenter = new Vector3(0f, 0.5f, 0f);

    [Header("Transition Accroupi/Debout")]
    [Tooltip("Durée (en secondes) pour la transition accroupi ↔ debout.")]
    public float crouchTransitionTime = 0.3f;

    [Header("Caméra et sensibilité")]
    public float mouseSensitivity = 100f;
    public Camera playerCamera;

    // Composants
    private CharacterController characterController;

    // États du joueur
    private bool isCrouching = false;
    private bool isSprinting = false;

    // Pour le saut et la gravité
    private float verticalVelocity = 0f;

    // Gestion de la transition accroupi/debout
    private bool isTransitioning = false;
    private float transitionStartTime;
    private float fromHeight, toHeight;
    private Vector3 fromCenter, toCenter;

    void Start()
    {
        characterController = GetComponent<CharacterController>();

        // Verrouille le curseur
        Cursor.lockState = CursorLockMode.Locked;

        // Caméra par défaut si non assignée
        if (playerCamera == null)
            playerCamera = Camera.main;

        // Position initiale
        Vector3 startPos = transform.position;
        startPos.y = 1f;
        transform.position = startPos;

        // Hauteur et centre initiaux
        characterController.height = standHeight;
        characterController.center = standCenter;
    }

    void Update()
    {
        HandleCrouchInput();
        HandleSprint();
        UpdateCrouchTransition();
        MovePlayer();
        RotateView();
    }

    void HandleCrouchInput()
    {
        if (Input.GetKeyDown(KeyCode.C) && !isCrouching)
        {
            isCrouching = true;
            StartCrouchTransition(true);
        }
        else if (Input.GetKeyUp(KeyCode.C) && isCrouching)
        {
            isCrouching = false;
            StartCrouchTransition(false);
        }
    }

    void HandleSprint()
    {
        // Courir avec LeftShift, sauf si accroupi
        isSprinting = Input.GetKey(KeyCode.LeftShift) && !isCrouching;
    }

    void StartCrouchTransition(bool goingDown)
    {
        isTransitioning = true;
        transitionStartTime = Time.time;
        fromHeight = characterController.height;
        fromCenter = characterController.center;
        toHeight = goingDown ? crouchHeight : standHeight;
        toCenter = goingDown ? crouchCenter : standCenter;
    }

    void UpdateCrouchTransition()
    {
        if (!isTransitioning) return;

        float elapsed = Time.time - transitionStartTime;
        float t = Mathf.Clamp01(elapsed / crouchTransitionTime);

        characterController.height = Mathf.Lerp(fromHeight, toHeight, t);
        characterController.center = Vector3.Lerp(fromCenter, toCenter, t);

        if (t >= 1f)
            isTransitioning = false;
    }

    void MovePlayer()
    {
        // Gestion de la gravité et du saut
        if (characterController.isGrounded)
        {
            // Assure le contact avec le sol
            verticalVelocity = -2f;

            // Saut
            if (Input.GetButtonDown("Jump"))
            {
                verticalVelocity = jumpForce;
            }
        }
        else
        {
            // Gravité quand on est en l'air
            verticalVelocity -= gravity * Time.deltaTime;
        }

        // Déplacement horizontal
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 move = transform.right * h + transform.forward * v;

        // Choix de la vitesse
        float speed = walkSpeed;
        if (isCrouching) speed = crouchSpeed;
        else if (isSprinting) speed = sprintSpeed;

        // On construit le vecteur final
        Vector3 velocity = move * speed;
        velocity.y = verticalVelocity;

        // On déplace le CharacterController
        characterController.Move(velocity * Time.deltaTime);
    }

    void RotateView()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Rotation du joueur
        transform.Rotate(Vector3.up * mouseX);

        // Rotation verticale de la caméra
        Vector3 camRot = playerCamera.transform.localEulerAngles;
        float newX = camRot.x - mouseY;
        if (newX > 180f) newX -= 360f;
        newX = Mathf.Clamp(newX, -90f, 90f);
        playerCamera.transform.localEulerAngles = new Vector3(newX, 0f, 0f);
    }
}