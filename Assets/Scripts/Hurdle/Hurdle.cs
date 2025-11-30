using System.Collections.Generic;
using UnityEngine;

public class Hurdle : MonoBehaviour
{
    [SerializeField] private int id;
    [SerializeField] private float length = 10f;

    public int ID => id;
    public float Length => length;

    List<Orb> orbs = new List<Orb>();

    void Start()
    {
        foreach (Transform child in transform)
        {
            Orb orb = child.GetComponent<Orb>();
            if (orb != null)
            {
                orbs.Add(orb);
            }
        }
    }
    public void ResetOrbVisibility()
    {
        foreach (Orb orb in orbs)
        {
            orb.EnableMesh();
        }
    }
}
