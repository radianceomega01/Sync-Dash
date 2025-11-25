using UnityEngine;

public class Orb : MonoBehaviour
{
    public int id;
    public ParticleSystem collectFx;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        GameManager.Instance.OrbCollected(id);

        if (collectFx != null)
            collectFx.Play();

        gameObject.SetActive(false);
    }
}