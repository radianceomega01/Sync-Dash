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
            transform.localPosition = Vector3.Lerp(transform.localPosition, targetPos, 10f * Time.deltaTime);
    }

    void Apply(SyncMessage msg)
    {
        targetPos = msg.position;
        initialized = true;
    }
}