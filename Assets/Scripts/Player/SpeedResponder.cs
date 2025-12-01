using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedResponder : MonoBehaviour
{
    [SerializeField] private GameObject speedTrail;
    [SerializeField] private float trailTime = 1.5f;

    public void TriggerSpeedEffect()
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
