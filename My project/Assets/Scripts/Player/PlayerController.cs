using Cinemachine;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private CharacterController controller;
    private PlayerInputActions input;

    [Header("Movement")]
    [SerializeField] private float walkSpeed = 6f;
    [SerializeField] private float sprintSpeed = 10f;
    [SerializeField] private float gravity = -15f;
    [SerializeField] private float jumpHeight = 1f;

    private Vector2 moveInput;
    private Vector3 velocity;
    private bool isGrounded;
    private bool isSprinting;

    [Header("Camera movement")]
    [SerializeField] private Transform cameraTarget;
    [SerializeField] private float sensitivity = 0.1f;
    private Vector2 lookInput;
    private float xRotation = 0f;


    private bool canMove = true;
    private bool canLookAround = true;

    private void Awake()
    {
        LockMouse();
        controller = GetComponent<CharacterController>();

        input = new PlayerInputActions();

        //Link Movement avec le input system
        input.PlayerActions.Move.performed += context => moveInput = context.ReadValue<Vector2>();
        input.PlayerActions.Move.canceled += context => moveInput = Vector2.zero;

        //Link Jump et Sprint avec le input system
        input.PlayerActions.Jump.performed += context => Jump();
        //input.PlayerMovement.Sprint.performed += context => isSprinting = true;
        //input.PlayerMovement.Sprint.canceled += context => isSprinting = false;

        //Link LookAround avec le input system
        input.PlayerActions.LookAround.performed += ctx => lookInput = ctx.ReadValue<Vector2>();
        input.PlayerActions.LookAround.canceled += ctx => lookInput = Vector2.zero;
    }

    private void OnEnable() => input.PlayerActions.Enable();
    private void OnDisable() => input.PlayerActions.Disable();

    private void Update()
    {
        if (!canMove || !canLookAround) return;

        HandleMovement();
        HandleLookAround();
    }

    private void HandleMovement()
    {
        isGrounded = controller.isGrounded;

        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;

        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;

        float speed = isSprinting ? sprintSpeed : walkSpeed;

        controller.Move(move * speed * Time.deltaTime);

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    private void HandleLookAround()
    {
        float mouseX = lookInput.x * sensitivity;
        float mouseY = lookInput.y * sensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -85f, 85f);

        cameraTarget.localRotation = Quaternion.Euler(xRotation, 0f, 0f); // rotation verticale sur CameraTarget
        transform.Rotate(Vector3.up * mouseX); // rotation horizontale sur le joueur
    }

    private void Jump()
    {
        if (isGrounded)
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
    }

    public void SnapToBall(Transform ballTransform, Transform flagTransform)
    {
        Vector3 directionToFlag = (flagTransform.position - ballTransform.position).normalized;
        directionToFlag.y = 0f;

        // Le joueur se place derrière la balle par rapport au drapeau
        float stanceDistance = 1.5f; // distance derrière la balle
        float stanceHeight = 1f;   // hauteur fixe indépendante de la balle

        Vector3 left = Vector3.Cross(directionToFlag, Vector3.up);
        Vector3 stancePosition = ballTransform.position + left * stanceDistance;
        stancePosition.y = ballTransform.position.y + stanceHeight;

        Vector3 directionToBall = (ballTransform.position - stancePosition).normalized;
        directionToBall.y = 0f;
        Quaternion lookAtBall = Quaternion.LookRotation(directionToBall);

        
        FreezePlayer();
        transform.SetPositionAndRotation(stancePosition, lookAtBall);
        UnFreezePlayer();
    }

    public void FreezePlayer()
    {
        this.enabled = false;
    }

    public void UnFreezePlayer()
    {
        this.enabled = true;
    }

    public void LockMouse()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void UnlockMouse()
    {
        Cursor.lockState = CursorLockMode.None;
    }
}
