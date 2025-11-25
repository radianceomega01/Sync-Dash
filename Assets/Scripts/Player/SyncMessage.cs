using System;
using UnityEngine;


[Serializable]
public struct SyncMessage {
public float time; // Time.time when captured
public Vector3 position;
public Vector3 velocity;
public bool jumped;
public int collectedOrbId; // -1 if none
public int collidedObstacleId; // -1 if none


public static SyncMessage Create(Vector3 pos, Vector3 vel, bool jumped = false, int orb = -1, int obs = -1) {
return new SyncMessage {
time = Time.time,
position = pos,
velocity = vel,
jumped = jumped,
collectedOrbId = orb,
collidedObstacleId = obs
};
}
}