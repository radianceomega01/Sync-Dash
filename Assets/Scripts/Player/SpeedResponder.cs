using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedResponder : MonoBehaviour
{
    [SerializeField] private GameObject speedTrail;
    [SerializeField] private float trailTime = 2f;
    Rigidbody rigidBody;
    float prevZVelocity;

    void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
    }
    void OnEnable()
    {
        GameManager.Instance.OnSpeedChanged += HandleSpeedChanged;
    }
    void OnDisable()
    {
        GameManager.Instance.OnSpeedChanged -= HandleSpeedChanged;
    }

    void FixedUpdate()
    {
        // if(rigidBody.velocity.z > prevZVelocity)
        // {
        //     HandleSpeedChanged();
        // }
        // prevZVelocity = rigidBody.velocity.z;
    }

    private void HandleSpeedChanged(float newSpeed)
    {
        StartCoroutine(ToggleSpeedTrail());
    }
    private IEnumerator ToggleSpeedTrail()
    {
        speedTrail.SetActive(true);
        yield return new WaitForSeconds(trailTime);
        speedTrail.SetActive(false);
    }
}
