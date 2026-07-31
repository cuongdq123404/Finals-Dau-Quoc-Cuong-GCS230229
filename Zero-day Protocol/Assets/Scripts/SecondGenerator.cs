using System.Collections.Generic;
using UnityEngine;

public class SecondGenerator : MonoBehaviour
{
    public enum BlockType
    {
        SafeRoom,
        ExitRoom,
        KeyRoom,
        SupplyRoom,
        RandomHorizontalRoom,
        RandomVerticalRoom,
        HallwayA_StraightHorizontal,
        HallwayB_CornerDownRight,
        HallwayC_CornerUp,
        HallwayD_StraightVertical
    }

    [System.Serializable]
    public struct ManualStep
    {
        public BlockType blockToSpawn;
    }

    [System.Serializable]
    public class LevelManual
    {
        public string manualName = "New Layout Pattern";
        public List<ManualStep> steps = new List<ManualStep>();
    }

    [Header("Manual Layout Instructions")]
    public List<LevelManual> levelManuals = new List<LevelManual>();

    [Header("Special Fixed Rooms")]
    public GameObject safeRoomPrefab;
    public GameObject exitRoomPrefab;
    public GameObject keyRoomPrefab;
    public GameObject supplyRoomPrefab;

    [Header("Directional Connected Hallways")]
    public GameObject hallwayPrefabA;
    public GameObject hallwayPrefabB;
    public GameObject hallwayPrefabC;
    public GameObject hallwayPrefabD;

    [Header("Separated Room Pools")]
    public GameObject[] horizontalChallengeRooms;
    public GameObject[] verticalChallengeRooms;

    [Header("Scene References")]
    public Transform globalGridTransform;
    public float gridCellSize = 1f;

    // Global Checklist tracking set to eliminate duplicate assets completely
    private HashSet<BlockType> spawnedUniqueRooms = new HashSet<BlockType>();

    void Start()
    {
        if (levelManuals == null || levelManuals.Count == 0)
        {
            Debug.LogError("[Generator] You must design at least one Level Manual in the Inspector layout!");
            return;
        }

        GenerateManualLevel();
    }

    void GenerateManualLevel()
    {
        // Clear the checklist for this fresh generation run
        spawnedUniqueRooms.Clear();

        int randomManualIndex = Random.Range(0, levelManuals.Count);
        LevelManual activeManual = levelManuals[randomManualIndex];
        Debug.Log($"[Generator] Loading Instruction Manual: {activeManual.manualName}");

        Vector3 nextSpawnPosition = Vector3.zero;

        // Loop through the steps exactly as written in the chosen manual
        for (int i = 0; i < activeManual.steps.Count; i++)
        {
            BlockType currentType = activeManual.steps[i].blockToSpawn;

            // 1. DYNAMIC SUBSTITUTION INJECTION (Ensures they show up at least once)
            if (currentType == BlockType.RandomHorizontalRoom && !spawnedUniqueRooms.Contains(BlockType.KeyRoom))
            {
                currentType = BlockType.KeyRoom;
            }
            else if (currentType == BlockType.RandomVerticalRoom && !spawnedUniqueRooms.Contains(BlockType.SupplyRoom))
            {
                currentType = BlockType.SupplyRoom;
            }

            // ensure that unique rooms are not duplicated
            if (currentType == BlockType.KeyRoom)
            {
                if (spawnedUniqueRooms.Contains(BlockType.KeyRoom))
                {
                    // Downgrade this step back to a standard horizontal room layout instead
                    Debug.LogWarning($"[Generator Anti-Duplicate] Key Room duplicate blocked at step {i}! Replacing with a standard horizontal room.");
                    currentType = BlockType.RandomHorizontalRoom;
                }
                else
                {
                    spawnedUniqueRooms.Add(BlockType.KeyRoom); 
                }
            }
            else if (currentType == BlockType.SupplyRoom)
            {
                if (spawnedUniqueRooms.Contains(BlockType.SupplyRoom))
                {
                    // Downgrade this step back to a standard vertical room layout instead
                    Debug.LogWarning($"[Generator Anti-Duplicate] Supply Room duplicate blocked at step {i}! Replacing with a standard vertical room.");
                    currentType = BlockType.RandomVerticalRoom;
                }
                else
                {
                    spawnedUniqueRooms.Add(BlockType.SupplyRoom); 
                }
            }

            // 3. Finalize Asset Fetching
            GameObject targetPrefab = GetPrefabFromBlockType(currentType);
            if (targetPrefab == null) continue;

            nextSpawnPosition = SpawnAndAlignRoom(targetPrefab, nextSpawnPosition);
        }

        // Safety check: Ensure that both unique rooms were actually spawned at least once
        if (!spawnedUniqueRooms.Contains(BlockType.KeyRoom))
            Debug.LogWarning("[Generator Failure Check] Key Room missed generation! Ensure your layout pattern includes at least one 'RandomHorizontalRoom' or explicit 'KeyRoom' step.");
        if (!spawnedUniqueRooms.Contains(BlockType.SupplyRoom))
            Debug.LogWarning("[Generator Failure Check] Supply Room missed generation! Ensure your layout pattern includes at least one 'RandomVerticalRoom' or explicit 'SupplyRoom' step.");

        Debug.Log("[Generator] Level built smoothly following the instruction blueprint!");
    }

    private GameObject GetPrefabFromBlockType(BlockType type)
    {
        switch (type)
        {
            case BlockType.SafeRoom: return safeRoomPrefab;
            case BlockType.ExitRoom: return exitRoomPrefab;
            case BlockType.KeyRoom: return keyRoomPrefab;
            case BlockType.SupplyRoom: return supplyRoomPrefab;

            case BlockType.RandomHorizontalRoom:
                if (horizontalChallengeRooms.Length == 0) return null;
                return horizontalChallengeRooms[Random.Range(0, horizontalChallengeRooms.Length)];

            case BlockType.RandomVerticalRoom:
                if (verticalChallengeRooms.Length == 0) return null;
                return verticalChallengeRooms[Random.Range(0, verticalChallengeRooms.Length)];

            case BlockType.HallwayA_StraightHorizontal: return hallwayPrefabA;
            case BlockType.HallwayB_CornerDownRight: return hallwayPrefabB;
            case BlockType.HallwayC_CornerUp: return hallwayPrefabC;
            case BlockType.HallwayD_StraightVertical: return hallwayPrefabD;

            default: return null;
        }
    }

    private Vector3 SnapToGrid(Vector3 v)
    {
        if (gridCellSize <= 0f) return v;
        return new Vector3(
            Mathf.Round(v.x / gridCellSize) * gridCellSize,
            Mathf.Round(v.y / gridCellSize) * gridCellSize,
            v.z
        );
    }

    private Vector3 SpawnAndAlignRoom(GameObject prefab, Vector3 currentSpawnPos)
    {
        GameObject spawnedObj = Instantiate(prefab, Vector3.zero, Quaternion.identity);
        if (globalGridTransform != null)
            spawnedObj.transform.SetParent(globalGridTransform, false);

        Transform entrance = spawnedObj.transform.Find("EntrancePoint");
        Vector3 desiredLocalPos = currentSpawnPos;
        if (entrance != null)
        {
            desiredLocalPos = currentSpawnPos - entrance.localPosition;
        }
        else
        {
            Debug.LogError($"[Generator Error] {prefab.name} is missing its 'EntrancePoint' child pivot!");
        }

        desiredLocalPos = SnapToGrid(desiredLocalPos);
        spawnedObj.transform.localPosition = desiredLocalPos;

        Transform exit = spawnedObj.transform.Find("ExitPoint");
        if (exit != null)
        {
            return spawnedObj.transform.localPosition + exit.localPosition;
        }

        return spawnedObj.transform.localPosition;
    }
}