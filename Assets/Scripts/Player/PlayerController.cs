using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] public float jumpForce = 6f;
    [SerializeField] public LayerMask groundMask;
    [SerializeField] public Transform groundCheck;
    [SerializeField] public SpeedResponder speedResponder;

    Rigidbody rb;
    bool isGrounded;
    float syncTimer = 0f;
    InputSystem_Actions playerInputActions;
    bool speedChanged;

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
        GameManager.Instance.OnSpeedChanged += SpeedChanged;
    }

    void OnDisable()
    {
        playerInputActions.Disable();
        playerInputActions.Player.Jump.performed -= HandleJumpPressed;
        GameManager.Instance.OnGameOver -= GameOver;
        GameManager.Instance.OnSpeedChanged -= SpeedChanged;
    }

    void Update()
    {
        //Throttle sync messages
        syncTimer += Time.deltaTime;
        if (syncTimer >= SyncManager.Instance.SyncInterval)
        {
            SyncManager.Instance.SendMessage(
                SyncMessage.Create(transform.localPosition, speedChanged)
            );
            syncTimer = 0f;
            speedChanged = false;
        }
    }

    void FixedUpdate()
    {
        if (rb.isKinematic) return;

        float speed = GameManager.Instance.CurrentPlayerSpeed;
        Vector3 newPos = rb.position + Vector3.forward * speed * Time.fixedDeltaTime;

        rb.MovePosition(newPos);

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
    private void SpeedChanged(float obj)
    {
        speedResponder.TriggerSpeedEffect();
        speedChanged = true;
    }

    void GameOver()
    {
        rb.useGravity = false;
        rb.isKinematic = true;
    }
}