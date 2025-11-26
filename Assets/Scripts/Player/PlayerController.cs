using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] public float jumpForce = 6f;
    [SerializeField] public LayerMask groundMask;
    [SerializeField] public Transform groundCheck;

    Rigidbody rb;
    bool isGrounded;
    float syncTimer = 0f;
    InputSystem_Actions playerInputActions;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerInputActions = new InputSystem_Actions();
    }

    void OnEnable()
    {
        playerInputActions.Enable();
        playerInputActions.Player.Jump.performed += HandleJumpPressed;
    }

    void OnDisable()
    {
        playerInputActions.Disable();
        playerInputActions.Player.Jump.performed -= HandleJumpPressed;
    }

    void Update()
    {
        // Throttle sync messages
        syncTimer += Time.deltaTime;
        if (syncTimer >= SyncManager.Instance.SyncInterval)
        {
            SyncManager.Instance.SendMessage(
                SyncMessage.Create(transform.localPosition)
            );
            syncTimer = 0f;
        }
    }

    void FixedUpdate()
    {
        // get dynamic speed from GameManager
        Vector3 vel = rb.linearVelocity;
        vel.x = GameManager.Instance.CurrentPlayerSpeed;
        rb.linearVelocity = vel;

        // Ground check
        isGrounded = Physics.OverlapSphere(groundCheck.position, 0.1f, groundMask).Length > 0;
    }

    void HandleJumpPressed(UnityEngine.InputSystem.InputAction.CallbackContext ct)
    {
        if (isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);

            // SyncManager.Instance.SendMessage(
            //         SyncMessage.Create(transform.position)
            //     );
        }
    }
}