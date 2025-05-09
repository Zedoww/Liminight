using UnityEngine;
using System;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Vitesses")]
    public float walkSpeed = 5f;
    public float sprintSpeed = 10f;
    public float crouchSpeed = 2.5f;
    public float jumpForce = 5f;
    public float gravity = 9.81f;

    [Header("Hauteur & Crouch")]
    public float standHeight = 2f;
    public float crouchHeight = 1f;
    public Vector3 standCenter = new Vector3(0f, 1f, 0f);
    public Vector3 crouchCenter = new Vector3(0f, 0.5f, 0f);
    public float crouchTransitionTime = 0.3f;

    [Header("Caméra")]
    public float mouseSensitivity = 100f;
    public Camera playerCamera;

    [Header("Stamina")]
    public float maxStamina = 1f;
    public float staminaRegenRate = 0.25f;  // à l'arrêt
    public float staminaRegenWhileWalking = 0.1f;   // en marchant
    public float staminaUseRate = 1f / 6f; // sprint = 6s
    [Tooltip("Seuil de vitesse minimum pour consommer de la stamina en sprintant")]
    public float staminaUseSpeedThreshold = 0.5f;
    [HideInInspector] public float currentStamina = 1f;
    [HideInInspector] public bool isExhausted;

    [Header("Essoufflement")]
    [Tooltip("Multiplicateur de vitesse quand le joueur est essoufflé")]
    public float exhaustedSpeedMultiplier = 0.7f;
    [Tooltip("Durée du ralentissement après avoir été essoufflé (en secondes)")]
    public float exhaustedSlowdownDuration = 5f;

    // UI
    [Header("UI")]
    [SerializeField] private StaminaUI staminaUI;
    public Action<float> onStaminaChanged;

    // Composants
    private CharacterController characterController;

    // États
    private bool isCrouching = false;
    public bool isSprinting = false;
    private float verticalVel = 0f;
    private float exhaustedStartTime = -999f;
    private bool wasExhaustedLastFrame = false;
    private float currentSpeedMultiplier = 1f;

    // Crouch lerp
    private bool isTransitioning;
    private float transitionStartTime;
    private float fromHeight, toHeight;
    private Vector3 fromCenter, toCenter;

    void Start()
    {
        characterController = GetComponent<CharacterController>();

        // UI hookup
        if (staminaUI != null)
            onStaminaChanged += staminaUI.SetStamina;

        // Curseur
        Cursor.lockState = CursorLockMode.Locked;
        if (playerCamera == null) playerCamera = Camera.main;

        // Position Y initiale
        Vector3 p = transform.position; p.y = 1f;
        transform.position = p;

        // Crouch initial
        characterController.height = standHeight;
        characterController.center = standCenter;
    }

    void Update()
    {
        HandleCrouchInput();
        HandleStamina();
        UpdateExhaustedEffect();
        UpdateCrouchTransition();
        MovePlayer();
        RotateView();
    }

    // ───── CROUCH ─────
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

    void StartCrouchTransition(bool down)
    {
        isTransitioning = true;
        transitionStartTime = Time.time;
        fromHeight = characterController.height;
        fromCenter = characterController.center;
        toHeight = down ? crouchHeight : standHeight;
        toCenter = down ? crouchCenter : standCenter;
    }

    void UpdateCrouchTransition()
    {
        if (!isTransitioning) return;
        float t = Mathf.Clamp01((Time.time - transitionStartTime) / crouchTransitionTime);
        characterController.height = Mathf.Lerp(fromHeight, toHeight, t);
        characterController.center = Vector3.Lerp(fromCenter, toCenter, t);
        if (t >= 1f) isTransitioning = false;
    }

    // ───── STAMINA & SPRINT ─────
    void HandleStamina()
    {
        // Vérifier si le joueur se déplace suffisamment pour consommer de la stamina
        Vector3 horizontalVelocity = characterController.velocity;
        horizontalVelocity.y = 0f;
        float horizontalSpeed = horizontalVelocity.magnitude;
        
        bool wantSprint = Input.GetKey(KeyCode.LeftShift) && !isCrouching;
        bool isMovingEnough = horizontalSpeed > staminaUseSpeedThreshold;
        
        if (wantSprint && !isExhausted && isMovingEnough)
        {
            isSprinting = true;
            currentStamina -= staminaUseRate * Time.deltaTime;
            if (currentStamina <= 0f)
            {
                currentStamina = 0f;
                isExhausted = true;
                exhaustedStartTime = Time.time; // Enregistrer le moment où le joueur devient essoufflé
                isSprinting = false;
            }
        }
        else
        {
            isSprinting = false;
            float regen = characterController.velocity.magnitude > 0.1f
                        ? staminaRegenWhileWalking
                        : staminaRegenRate;
            currentStamina += regen * Time.deltaTime;
            if (currentStamina >= maxStamina)
            {
                currentStamina = maxStamina;
                if (isExhausted)
                {
                isExhausted = false;
                }
            }
        }

        onStaminaChanged?.Invoke(currentStamina / maxStamina);
    }

    // ───── ESSOUFFLEMENT VITESSE ─────
    void UpdateExhaustedEffect()
    {
        // Détecter quand le joueur devient essoufflé
        if (isExhausted && !wasExhaustedLastFrame)
        {
            exhaustedStartTime = Time.time;
        }

        // Calculer le multiplicateur de vitesse
        if (isExhausted || Time.time < exhaustedStartTime + exhaustedSlowdownDuration)
        {
            float timeSinceExhausted = Time.time - exhaustedStartTime;
            
            if (isExhausted)
            {
                // Ralentissement maximal tant que le joueur est essoufflé
                currentSpeedMultiplier = exhaustedSpeedMultiplier;
            }
            else if (timeSinceExhausted < exhaustedSlowdownDuration)
            {
                // Transition progressive vers la vitesse normale
                float t = timeSinceExhausted / exhaustedSlowdownDuration;
                currentSpeedMultiplier = Mathf.Lerp(exhaustedSpeedMultiplier, 1f, t);
            }
            else
            {
                currentSpeedMultiplier = 1f;
            }
        }
        else
        {
            currentSpeedMultiplier = 1f;
        }

        wasExhaustedLastFrame = isExhausted;
    }

    // ───── MOUVEMENT ─────
    void MovePlayer()
    {
        if (characterController.isGrounded)
        {
            verticalVel = -2f;
            if (Input.GetButtonDown("Jump"))
                verticalVel = jumpForce;
        }
        else verticalVel -= gravity * Time.deltaTime;

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 dir = transform.right * h + transform.forward * v;

        float speed = walkSpeed;
        if (isCrouching) speed = crouchSpeed;
        else if (isSprinting) speed = sprintSpeed;

        // Appliquer le multiplicateur de vitesse dû à l'essoufflement
        speed *= currentSpeedMultiplier;

        Vector3 vel = dir * speed;
        vel.y = verticalVel;
        characterController.Move(vel * Time.deltaTime);
    }

    // ───── CAMÉRA ─────
    void RotateView()
    {
        float mx = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float my = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        transform.Rotate(Vector3.up * mx);

        Vector3 c = playerCamera.transform.localEulerAngles;
        float nx = c.x - my;
        if (nx > 180f) nx -= 360f;
        nx = Mathf.Clamp(nx, -90f, 90f);
        playerCamera.transform.localEulerAngles = new Vector3(nx, 0f, 0f);
    }
}
