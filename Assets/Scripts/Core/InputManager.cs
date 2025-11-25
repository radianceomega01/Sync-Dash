using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Centralized mobile/desktop tap handler for Sync Dash.
/// Detects Jump input and broadcasts event.
/// Game systems (PlayerController, Tutorial, UI) subscribe to OnJumpPressed.
/// </summary>
public class InputManager : MonoBehaviour
{
    public static InputManager Instance;

    /// <summary>
    /// Triggered once when the user performs a tap/jump input.
    /// </summary>
    public UnityEvent OnJumpPressed;

    private bool inputBlocked => GameManager.Instance.CurrentState != GameManager.GameState.Playing;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        if (OnJumpPressed == null)
            OnJumpPressed = new UnityEvent();
    }

    void Update()
    {
        if (inputBlocked) return;

        if (TapDetected())
            OnJumpPressed.Invoke();
    }

    /// <summary>
    /// Cross-platform tap detection (mouse + touch).
    /// </summary>
    /// <returns>true only once on touch/click begin</returns>
    bool TapDetected()
    {
#if UNITY_EDITOR
        return Input.GetMouseButtonDown(0);
#else
        if (Input.touchCount > 0)
        {
            return Input.GetTouch(0).phase == TouchPhase.Began;
        }
        return false;
#endif
    }
}