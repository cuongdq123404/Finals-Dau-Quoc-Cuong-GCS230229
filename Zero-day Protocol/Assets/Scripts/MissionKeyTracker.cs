using UnityEngine;
using System.Collections.Generic;

public class MissionKeyTracker : MonoBehaviour
{
    // A hashset safely records unique string IDs without messing up your UI inventory slots
    private HashSet<string> heldKeycardIDs = new HashSet<string>();

    public void AddKey(string id)
    {
        if (!heldKeycardIDs.Contains(id))
        {
            heldKeycardIDs.Add(id);
        }
    }

    public bool HasKey(string id)
    {
        return heldKeycardIDs.Contains(id);
    }
}