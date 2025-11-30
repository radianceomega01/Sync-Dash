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
        playerInputActions = new InputSystem_Actions();
        rb = GetComponent<Rigidbody>();
    }

    void OnEnable()
    {
        playerInputActions.Enable();
        playerInputActions.Player.Jump.performed += HandleJumpPressed;
        GameManager.Instance.OnGameOver += GameOver;
    }

    void OnDisable()
    {
        playerInputActions.Disable();
        playerInputActions.Player.Jump.performed -= HandleJumpPressed;
        GameManager.Instance.OnGameOver -= GameOver;
    }

    void Update()
    {
        //Throttle sync messages
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
        if (rb.isKinematic) return;

        // get dynamic speed from GameManager
        Vector3 vel = rb.velocity;
        vel.z = GameManager.Instance.CurrentPlayerSpeed;
        rb.velocity = vel;

        // Ground check
        isGrounded = Physics.OverlapSphere(groundCheck.position, 0.1f, groundMask).Length > 0;

    }

    void HandleJumpPressed(UnityEngine.InputSystem.InputAction.CallbackContext ct)
    {
        if (isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
        }
    }

    void GameOver()
    {
        rb.useGravity = false;
        rb.isKinematic = true;
    }
}