
using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
public class GhostController : MonoBehaviour
{
    [SerializeField] private SpeedResponder speedResponder;
    Vector3 velocity;
    Vector3 targetPosition;

    void Update()
    {
        if (SyncManager.Instance.TryGetMessage(out SyncMessage msg))
        {
            targetPosition = msg.position;
            velocity = (msg.position - transform.localPosition) / SyncManager.Instance.SyncInterval;
        }
        transform.localPosition = Vector3.MoveTowards(
            transform.localPosition,
            targetPosition,
            velocity.magnitude * Time.deltaTime
        );
        
        if (msg.isSpeedIncreased)
        {
            speedResponder.TriggerSpeedEffect();
        }
    }
}