using UnityEngine;

public class CameraShake : MonoBehaviour
{

    [Header("Shake Settings")]
    public float shakeDuration = 0.3f;
    public float shakeMagnitude = 0.3f;
    public float dampingSpeed = 1.0f;

    private float currentDuration = 0f;
    private Vector3 initialPos;

    void LateUpdate()
    {
        if (currentDuration > 0)
        {
            Vector3 randomPoint = initialPos + Random.insideUnitSphere * shakeMagnitude;

            transform.localPosition = randomPoint;

            currentDuration -= Time.unscaledDeltaTime * dampingSpeed;

            if (currentDuration <= 0f)
            {
                currentDuration = 0f;
                transform.localPosition = initialPos;  // reset
            }
        }
    }

    public void Shake(float duration = -1, float magnitude = -1)
    {
        initialPos = transform.localPosition;

        currentDuration = (duration > 0) ? duration : shakeDuration;
        shakeMagnitude = (magnitude > 0) ? magnitude : shakeMagnitude;
    }
}