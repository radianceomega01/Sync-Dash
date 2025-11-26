using UnityEngine;
using System.Collections.Generic;

public class SyncManager : MonoBehaviour
{
    public static SyncManager Instance;

    [Range(0f, 2f)]
    public float simulatedDelay = 0.2f;
    public float SyncInterval { get; } = 0.03f; // ~33 times per second max

    Queue<SyncMessage> buffer = new Queue<SyncMessage>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void SendMessage(SyncMessage msg) => buffer.Enqueue(msg);

    public bool TryGetMessage(out SyncMessage msg)
    {
        if (buffer.Count == 0) { msg = default; return false; }

        SyncMessage peek = buffer.Peek();
        if (Time.time - peek.time >= simulatedDelay)
        {
            msg = buffer.Dequeue();
            return true;
        }

        msg = default;
        return false;
    }
}