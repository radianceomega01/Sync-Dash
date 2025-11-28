using UnityEngine;

public class Collector : MonoBehaviour
{
    [SerializeField] private ObjectPool[] hurdlePools;
    [SerializeField] private Transform playerTransform;

    void Update()
    {
        if (playerTransform != null)
        {
            transform.position = new Vector3(0, 0, transform.position.z + playerTransform.position.z);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Hurdle")) return;
        ReturnToPool(other);
    }

    private void ReturnToPool(Collider other)
    {
        int id = other.GetComponent<Hurdle>().ID;
        hurdlePools[id].Return(other.gameObject);
    }
}
