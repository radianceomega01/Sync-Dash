using System.Collections;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [SerializeField] private float effectTime = 1f;

    private Material material;
    void Awake()
    {
        material = GetComponent<Renderer>().material;
    }
    private void OnCollisionEnter(Collision other)
    {
        CollisionResponder responder = other.gameObject.GetComponent<CollisionResponder>();
        if (responder != null)
        {
            responder.OnCollided();
        }

        GameManager.Instance.PlayerHit();
        if (material != null)
            StartCoroutine(PlayEffect(effectTime));
    }

    IEnumerator PlayEffect(float time)
    {
        float t = 0f;

        while (t < time)
        {
            t += Time.deltaTime;
            float current = Mathf.Lerp(0, 1, t / time);

            material.SetFloat("_EffectValue", current);

            yield return null;
        }

        // ensure exact end value
        material.SetFloat("_EffectValue", 1);
    }
}