using System.Data;
using UnityEngine;

public class LevelSpawner : MonoBehaviour
{
    [SerializeField] private ObjectPool[] hurdlePools;
    [SerializeField] private Transform worldEnv;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private SpawnerType spawnerType;

    private Vector3 lastSpawnPoint;
    private float initialYOffset;
    private float initialZOffset;
    private int hurdleID;
    private float previousHurdleLength = 0f;

    void Start()
    {
        lastSpawnPoint = worldEnv.GetChild(worldEnv.childCount - 1).localPosition;
        previousHurdleLength = 20f;
        initialZOffset = transform.position.z;
        initialYOffset = transform.position.y;
    }

    void Update()
    {
        if (playerTransform != null)
        {
            transform.position = new Vector3(0, initialYOffset, initialZOffset + playerTransform.position.z);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Hurdle")) return;
        ReturnToPool(other);
        SpawnRandomHurdle();
    }

    private void ReturnToPool(Collider other)
    {
        Hurdle hurdle = other.GetComponentInParent<Hurdle>();
        int id = hurdle.ID;
        hurdlePools[id].Return(hurdle.gameObject);
        hurdle.ResetOrbVisibility();
    }

    private void SpawnRandomHurdle()
    {
        if (spawnerType == SpawnerType.MainWorld)
            hurdleID = SyncManager.Instance.GetRandomHurdleID(hurdlePools.Length);
        else
            hurdleID = SyncManager.Instance.GetMainWorldHurdleID();    

        GameObject worldHurdle = hurdlePools[hurdleID].Get();
        float hurdleLength = worldHurdle.GetComponent<Hurdle>().Length;
        worldHurdle.transform.SetParent(worldEnv);
        lastSpawnPoint += new Vector3(0, 0, hurdleLength/2 + previousHurdleLength/2);
        worldHurdle.transform.localPosition = lastSpawnPoint;
        previousHurdleLength = hurdleLength;
    }

    public enum SpawnerType
    {
        MainWorld,
        CloneWorld
    }
}
