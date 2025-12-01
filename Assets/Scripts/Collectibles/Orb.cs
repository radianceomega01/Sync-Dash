using UnityEngine;

public class Orb : MonoBehaviour
{
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private ParticleSystem collectFx;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") && !other.CompareTag("Clone")) return;

        GameManager.Instance.OrbCollected();

        if (collectFx != null)
            collectFx.Play();

        meshRenderer.enabled = false;
    }

    public void EnableMesh()
    {
        meshRenderer.enabled = true;
    }
}