using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionResponder : MonoBehaviour
{
    [SerializeField] private CameraShake cameraShake;
    public void OnCollided()
    {
        cameraShake.Shake();
    }
}
