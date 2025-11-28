using UnityEngine;
using System.Collections.Generic;

public class SyncManager : MonoBehaviour
{
    public static SyncManager Instance;

    [Range(0f, 2f)]
    public float simulatedDelay = 0.2f;
    public float SyncInterval { get; } = 0.03f; // ~33 times per second max

    Queue<SyncMessage> buffer = new Queue<SyncMessage>();
    Queue<int> hurdleBuffer = new Queue<int>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // keep across scenes if needed
        }
        else
        {
            Destroy(gameObject);
            return;
        }
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

    public int GetRandomHurdleID(int hurdlePoolLength)
    {
        int id = Random.Range(0, hurdlePoolLength);
        hurdleBuffer.Enqueue(id);
        return id;
    }

    public int GetMainWorldHurdleID()
    {
        if (hurdleBuffer.Count == 0) return 0;
        return hurdleBuffer.Dequeue();
    }
}