using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [SerializeField] private Animator dissolveAnimator;

    private void OnCollisionEnter(Collision other)
    {
        CollisionResponder responder = other.gameObject.GetComponent<CollisionResponder>();
        if (responder != null)
        {
            responder.OnCollided();
        }
        
        GameManager.Instance.PlayerHit();

        if (dissolveAnimator != null)
            dissolveAnimator.SetTrigger("Dissolve");
    }
}