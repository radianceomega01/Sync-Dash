using System;
using UnityEngine;


[Serializable]
public struct SyncMessage {
    public float time; // Time.time when captured
    public Vector3 position;
    public bool isDead;

    public static SyncMessage Create(Vector3 pos, bool dead = false) {
        return new SyncMessage {
            time = Time.time,
            position = pos,
            isDead = dead
        };
    }
}