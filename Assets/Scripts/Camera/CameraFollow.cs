
using UnityEngine;

/// <summary>
/// Smooth third-person / isometric follow camera for Sync Dash.
/// Follows target position but keeps a fixed angle and offset.
/// </summary>
public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float zOffset = 3f;
    private Vector3 initialOffset;

    void Start()
    {
        initialOffset = transform.localPosition;
    }
    void LateUpdate()
    {
        if (target == null) return;

        // Smoothly interpolate to target position
        transform.localPosition = target.localPosition + initialOffset + new Vector3(0, 0, zOffset);
    }
}