
using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
public class GhostController : MonoBehaviour
{
    Rigidbody rigidBody;
    Vector3 velocity;

    void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (SyncManager.Instance.TryGetMessage(out SyncMessage msg))
        {
            velocity = (msg.position - transform.localPosition) / SyncManager.Instance.SyncInterval;
        }
    }
    void FixedUpdate()
    {
        rigidBody.velocity = velocity;
    }

}