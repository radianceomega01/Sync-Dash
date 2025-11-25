using UnityEngine;

public class GhostController : MonoBehaviour
{
    Vector3 targetPos;
    bool initialized = false;

    void Update()
    {
        if (SyncManager.Instance.TryGetMessage(out SyncMessage msg))
        {
            Apply(msg);
        }

        if (initialized)
            transform.position = Vector3.Lerp(transform.position, targetPos, 10f * Time.deltaTime);
    }

    void Apply(SyncMessage msg)
    {
        targetPos = msg.position;
        initialized = true;
    }
}