# Sync‑Dash

Sync‑Dash is a Unity-based movement synchronization and ghost‑replay
system.\
It synchronizes player movement snapshots (local‑space positions) and
replays them smoothly using transform‑based interpolation.\
Useful for ghost racing, replay systems, and multiplayer sync
prototypes.

------------------------------------------------------------------------

## 📌 Features

-   Smooth ghost replay using local‑space transform movement\
-   Snapshot-based synchronization (`SyncMessage`)\
-   `SyncManager` queue-based message distribution\
-   Clean architecture ready for network or replay expansion\
-   Works with transform‑based interpolation to eliminate physics jitter

------------------------------------------------------------------------

## 🚀 Getting Started

### 1. Clone the repository:

``` bash
git clone https://github.com/radianceomega01/Sync-Dash.git
```

### 2. Open in Unity

Use Unity 2021+ (or your preferred version compatible with the project).

### 3. Run the main scene

The system will replay ghost snapshots from `SyncManager`.

------------------------------------------------------------------------

## 🎮 Controls

-   Start and Exit buttons on Main Menu
-   Tap to jump during gameplay.
-   Restart and main menu buttons on game over panel

------------------------------------------------------------------------

## 🎮 How It Works

### **SyncManager**

-   Collects and distributes `SyncMessage` snapshots\
-   Acts as the central sync buffer

### **GhostController**

-   Receives target positions from snapshots\
-   Moves using:
    -   **transform.localPosition** (recommended for ghosts)
    -   Smooth interpolation (`Vector3.Lerp`)
-   Jitter‑free because no physics is used

------------------------------------------------------------------------

## 🧱 Project Structure

    Sync-Dash/
    ├── Assets/
    │   ├── Scripts/
    │   ├── Scenes/
    │   └── Prefabs/
    ├── Packages/
    ├── ProjectSettings/
    └── README.md

------------------------------------------------------------------------

