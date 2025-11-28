using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [SerializeField] private Animator dissolveAnimator;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        GameManager.Instance.PlayerHit();

        if (dissolveAnimator != null)
            dissolveAnimator.SetTrigger("Dissolve");
    }
}