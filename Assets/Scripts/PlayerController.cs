using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float walkSpeed = 5f;
    public float sprintSpeed = 10f;
    public float jumpForce = 5f;
    public float gravity = 9.81f;
    public float mouseSensitivity = 100f;

    private CharacterController characterController;
    private Vector3 moveDirection = Vector3.zero;
    private bool isSprinting = false;

    // R�f�rence � la cam�ra
    public Camera playerCamera;

    void Start()
    {
        characterController = GetComponent<CharacterController>();

        // Verrouiller le curseur au centre de l'�cran
        Cursor.lockState = CursorLockMode.Locked;

        // S'assurer que la cam�ra est assign�e
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }
    }

    void Update()
    {
        MovePlayer();
        RotateView();
    }

    void MovePlayer()
    {
        // V�rifier si le joueur est au sol
        bool isGrounded = characterController.isGrounded;

        // R�initialiser la v�locit� verticale lorsqu'au sol
        if (isGrounded)
        {
            moveDirection.y = -0.5f; // Une petite force vers le bas pour assurer le contact avec le sol
        }

        // Gestion du saut - ind�pendant de tout mouvement horizontal
        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            moveDirection.y = jumpForce;
        }

        // Get input for movement
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        // Determine the movement direction
        Vector3 move = transform.right * moveHorizontal + transform.forward * moveVertical;

        // Check if the player is sprinting
        isSprinting = Input.GetKey(KeyCode.LeftShift);

        // Set the movement speed
        float speed = isSprinting ? sprintSpeed : walkSpeed;

        // Conserver la v�locit� verticale (saut/gravit�) et appliquer le mouvement horizontal
        moveDirection.x = move.x * speed;
        moveDirection.z = move.z * speed;

        // Appliquer la gravit�
        moveDirection.y -= gravity * Time.deltaTime;

        // Move the player
        characterController.Move(moveDirection * Time.deltaTime);
    }


    void RotateView()
    {
        // R�cup�rer l'entr�e de la souris
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Rotation du joueur sur l'axe Y (gauche/droite)
        transform.Rotate(Vector3.up * mouseX);

        // Rotation verticale de la cam�ra (haut/bas)
        // R�cup�rer la rotation actuelle
        Vector3 currentRotation = playerCamera.transform.localEulerAngles;

        // Calculer la nouvelle rotation en X (avec inversion de mouseY pour un comportement naturel)
        float newRotationX = currentRotation.x - mouseY;

        // Ajuster la rotation si elle d�passe les limites
        if (newRotationX > 180)
            newRotationX -= 360;

        // Limiter la rotation entre -90 et 90 degr�s
        newRotationX = Mathf.Clamp(newRotationX, -90f, 90f);

        // Appliquer la nouvelle rotation � la cam�ra
        playerCamera.transform.localEulerAngles = new Vector3(newRotationX, 0f, 0f);
    }
}
