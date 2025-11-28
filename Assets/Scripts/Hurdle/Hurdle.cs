using UnityEngine;

public class Hurdle : MonoBehaviour
{
    [SerializeField] private int id;
    [SerializeField] private float length = 10f;

    public int ID => id;
    public float Length => length;
}
