using UnityEngine;

public class BossRoomManager : MonoBehaviour
{
    [Header("Repulsor Objects (Disabled as terminals are hacked)")]
    public GameObject repulsor1;
    public GameObject repulsor2;
    public GameObject repulsor3;
    public GameObject repulsor4;

    [Header("Laser Gates (Disable on Phase Update)")]
    public GameObject laserGateA;
    public GameObject laserGateB;
    public GameObject laserGateC;

    [Header("Boss & Turret Entities")]
    public GameObject centralTurret;
    public Turret turretAttackScript; // Drag your Turret component here!
    public ExitDoor exitDoorScript;  // Change type to ExitDoor script component
    public CardiacWarden warden;

    [Header("Progress Tracker")]
    public int totalTerminalsHacked = 0;

    public void OnTerminalHacked(int terminalID)
    {
        totalTerminalsHacked++;
        Debug.Log($"[BOSS MANAGER] Terminal {terminalID} Hacked! ({totalTerminalsHacked}/4)");

        switch (totalTerminalsHacked)
        {
            case 1:
                if (repulsor1 != null) repulsor1.SetActive(false);
                if (laserGateA != null) laserGateA.SetActive(false);
                if (warden != null) warden.TriggerOverdrive();
                break;

            case 2:
                if (repulsor2 != null) repulsor2.SetActive(false);
                if (laserGateB != null) laserGateB.SetActive(false);
                break;

            case 3:
                if (repulsor3 != null) repulsor3.SetActive(false);
                if (laserGateC != null) laserGateC.SetActive(false);
                break;

            case 4:
                ExecutePhase4_AutoShutdown();
                break;
        }
    }

    public void ExecutePhase4_AutoShutdown()
    {
        Debug.Log("[PHASE 4] Terminal 4 cleared! Shutting down all repulsors, turret, and Warden...");

        // 1. Explicitly turn off ALL 4 Repulsors
        if (repulsor1 != null) repulsor1.SetActive(false);
        if (repulsor2 != null) repulsor2.SetActive(false);
        if (repulsor3 != null) repulsor3.SetActive(false);
        if (repulsor4 != null) repulsor4.SetActive(false);

        // 2. Shut down Turret shooting immediately
        if (turretAttackScript != null)
        {
            turretAttackScript.enabled = false;
        }

        // 3. Freeze / shut down the Warden 
        if (warden != null)
        {
            warden.TriggerStun(9999f);
            warden.enabled = false;
        }

        // 4. Wait 2 seconds before unlocking the exit door
        Invoke(nameof(TriggerStageWin), 2.0f);
    }

    public void TriggerStageWin()
    {
        // Unlock exit door script (Keeps door visible and lets player press 'F' to escape)
        if (exitDoorScript != null)
        {
            exitDoorScript.UnlockDoor();
        }

        Debug.Log("==========================================");
        Debug.Log("STAGE CLEAR! All Repulsors, Turret, and Warden deactivated!");
        Debug.Log("==========================================");
    }
}