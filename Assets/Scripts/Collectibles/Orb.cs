using UnityEngine;

public class Orb : MonoBehaviour
{
    [SerializeField] private ParticleSystem collectFx;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        GameManager.Instance.OrbCollected();

        if (collectFx != null)
            collectFx.Play();

        gameObject.SetActive(false);
    }
}