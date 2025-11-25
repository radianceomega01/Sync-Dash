using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public int id;
    public Animator dissolveAnimator;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        GameManager.Instance.PlayerHit(id);

        if (dissolveAnimator != null)
            dissolveAnimator.SetTrigger("Dissolve");
    }
}