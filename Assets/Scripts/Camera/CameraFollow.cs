using UnityEngine;

/// <summary>
/// Smooth third-person / isometric follow camera for Sync Dash.
/// Follows target position but keeps a fixed angle and offset.
/// </summary>
public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Follow Settings")]
    [Tooltip("Offset relative to target (local space).")]
    public Vector3 followOffset = new Vector3(-2f, 2f, 0f); // slightly behind + above

    [Tooltip("Smoothing factor (higher = snappier, lower = floaty).")]
    [Range(1f, 20f)]
    public float followSmoothness = 10f;

    void LateUpdate()
    {
        if (target == null) return;

        // Desired camera position based on target
        Vector3 targetPos = target.position + followOffset;

        // Smoothly interpolate to target position
        transform.position = Vector3.Lerp(transform.position, targetPos, followSmoothness * Time.deltaTime);
    }
}