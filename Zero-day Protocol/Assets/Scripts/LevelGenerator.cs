using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [Header("Special Fixed Rooms")]
    public GameObject safeRoomPrefab;
    public GameObject exitRoomPrefab;
    public GameObject keyRoomPrefab;    
    public GameObject supplyRoomPrefab; 

    [Header("Room Pool For This Specific Scene")]
    [Tooltip("Drag the room prefabs you want to use for THIS stage here.")]
    public GameObject[] challengeRoomPrefabs;

    [Header("Generation Settings")]
    public int totalRoomsToSpawn = 6;

    [Header("Scene References")]
    public Transform globalGridTransform;

    private int keyRoomIndex;
    private int supplyRoomIndex; 

    void Start()
    {
        CalculateSpecialRoomIndices();
        GenerateSmartLevel();
    }

    void CalculateSpecialRoomIndices()
    {
        // Safety check: ensure enough rooms are available to place both Key and Supply rooms
        if (totalRoomsToSpawn < 4)
        {
            Debug.LogWarning("Total rooms to spawn is too low for both Key and Supply rooms! Defaulting to sequential middle slots.");
            keyRoomIndex = 1;
            supplyRoomIndex = 2;
            return;
        }

        // Create a list of available indices for the Key and Supply rooms, excluding the first and last room
        List<int> availableIndices = new List<int>();
        for (int i = 1; i < totalRoomsToSpawn - 1; i++)
        {
            availableIndices.Add(i);
        }

        // Randomly pick one index from the list for the Key Room, then remove it
        int randomKeyChoice = Random.Range(0, availableIndices.Count);
        keyRoomIndex = availableIndices[randomKeyChoice];
        availableIndices.RemoveAt(randomKeyChoice);

        // Randomly pick another index from the remaining slots for the Supply Room
        int randomSupplyChoice = Random.Range(0, availableIndices.Count);
        supplyRoomIndex = availableIndices[randomSupplyChoice];
    }

    void GenerateSmartLevel()
    {
        Vector3 nextSpawnPosition = Vector3.zero;

        for (int i = 0; i < totalRoomsToSpawn; i++)
        {
            GameObject roomPrefabToSpawn = null;

            // Pick the right room type depending on layout sequence
            if (i == 0)
            {
                roomPrefabToSpawn = safeRoomPrefab;
            }
            else if (i == totalRoomsToSpawn - 1)
            {
                roomPrefabToSpawn = exitRoomPrefab;
            }
            else if (i == keyRoomIndex)
            {
                roomPrefabToSpawn = keyRoomPrefab;
            }
            else if (i == supplyRoomIndex)
            {
                roomPrefabToSpawn = supplyRoomPrefab; // Spawns the single supply room at its random slot
            }
            else
            {
                // Pull from this scene's standard challenge room pool
                roomPrefabToSpawn = challengeRoomPrefabs[Random.Range(0, challengeRoomPrefabs.Length)];
            }

            if (roomPrefabToSpawn == null) continue;

            // --- Your Original Spawning and Alignment Logic ---
            GameObject spawnedRoom = Instantiate(roomPrefabToSpawn, Vector3.zero, Quaternion.identity);

            if (globalGridTransform != null)
            {
                spawnedRoom.transform.SetParent(globalGridTransform, false);
            }

            Transform entrance = spawnedRoom.transform.Find("EntrancePoint");
            if (entrance != null)
            {
                spawnedRoom.transform.localPosition = nextSpawnPosition - entrance.localPosition;
            }
            else if (i > 0)
            {
                Debug.LogError($"SMART GEN ERROR: {roomPrefabToSpawn.name} is missing an 'EntrancePoint'!");
            }

            Transform exit = spawnedRoom.transform.Find("ExitPoint");
            if (exit != null)
            {
                nextSpawnPosition = spawnedRoom.transform.localPosition + exit.localPosition;
            }
            else if (i != totalRoomsToSpawn - 1)
            {
                Debug.LogError($"SMART GEN ERROR: {roomPrefabToSpawn.name} is missing an 'ExitPoint'!");
            }
        }

        Debug.Log($"Level generated! Key Room at index {keyRoomIndex}, Supply Room at index {supplyRoomIndex}.");
    }
}