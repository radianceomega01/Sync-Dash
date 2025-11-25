using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    public float jumpForce = 6f;
    public LayerMask groundMask;
    public Transform groundCheck;

    Rigidbody rb;
    bool isGrounded;
    float syncTimer = 0f;
    float syncInterval = 0.03f; // ~33 times per second max

    void Awake() => rb = GetComponent<Rigidbody>();

    void OnEnable()
    {
        InputManager.Instance.OnJumpPressed.AddListener(HandleJumpPressed);
    }

    void OnDisable()
    {
        InputManager.Instance.OnJumpPressed.RemoveListener(HandleJumpPressed);
    }

    void Update()
    {
        if (GameManager.Instance.CurrentState != GameManager.GameState.Playing)
            return;

        // Throttle sync messages
        syncTimer += Time.deltaTime;
        if (syncTimer >= syncInterval)
        {
            SyncManager.Instance.SendMessage(
                SyncMessage.Create(transform.position, rb.linearVelocity)
            );
            syncTimer = 0f;
        }
    }

    void FixedUpdate()
    {
        if (GameManager.Instance.CurrentState != GameManager.GameState.Playing)
            return;

        // get dynamic speed from GameManager
        Vector3 vel = rb.linearVelocity;
        vel.x = GameManager.Instance.CurrentPlayerSpeed;
        rb.linearVelocity = vel;

        // Ground check
        isGrounded = Physics.Raycast(groundCheck.position, Vector3.down, 0.1f, groundMask);
    }

    void HandleJumpPressed()
    {
        if (isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);

            SyncManager.Instance.SendMessage(
                    SyncMessage.Create(transform.position, rb.linearVelocity, jumped: true)
                );
        }
    }
}