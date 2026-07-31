using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class IPStreamPuzzle : MonoBehaviour
{
    [Header("Scene Object Links")]
    [Tooltip("Drag your physical Door object from the hierarchy here.")]
    public GameObject targetVaultDoor;

    [Header("UI Element Links")]
    public TextMeshProUGUI headerDisplayTemplate;
    public Button[] optionGridButtons;

    [Header("IP Database Pool")]
    public string[] ipSubnetAddresses = new string[]
    {
        "192.168.1.45", "192.168.1.99", "192.168.0.45", "192.170.1.45",
        "10.0.0.4", "10.0.0.14", "10.1.0.4", "172.16.2.9",
        "172.16.2.99", "192.168.5.5", "192.168.5.50", "127.0.0.1"
    };

    private VaultTerminal activeTerminal;
    private string correctSecretKey;

    public void LinkActiveTerminal(VaultTerminal terminal)
    {
        activeTerminal = terminal;
    }

    void OnEnable()
    {
        SetupRandomizedPuzzle();
    }

    private void SetupRandomizedPuzzle()
    {
        if (optionGridButtons == null || optionGridButtons.Length == 0 || ipSubnetAddresses.Length < optionGridButtons.Length) return;

        int randomTargetIndex = Random.Range(0, ipSubnetAddresses.Length);
        correctSecretKey = ipSubnetAddresses[randomTargetIndex];

        if (headerDisplayTemplate != null)
        {
            headerDisplayTemplate.text = $"TARGET INTERCEPT: [ {correctSecretKey} ]";
        }

        int winningSlot = Random.Range(0, optionGridButtons.Length);

        for (int i = 0; i < optionGridButtons.Length; i++)
        {
            TextMeshProUGUI label = optionGridButtons[i].GetComponentInChildren<TextMeshProUGUI>();
            if (label == null) continue;

            if (i == winningSlot)
            {
                label.text = correctSecretKey;
            }
            else
            {
                int decoyIndex;
                do
                {
                    decoyIndex = Random.Range(0, ipSubnetAddresses.Length);
                } while (decoyIndex == randomTargetIndex);

                label.text = ipSubnetAddresses[decoyIndex];
            }
        }
    }

    public void SelectGridOption(Button clickedButton)
    {
        TextMeshProUGUI label = clickedButton.GetComponentInChildren<TextMeshProUGUI>();
        if (label == null) return;

        if (label.text == correctSecretKey)
        {
            ExecuteBypassSuccess();
        }
        else
        {
            Debug.LogWarning("ACCESS DENIED! Shuffling packets...");
            SetupRandomizedPuzzle();
        }
    }

    private void ExecuteBypassSuccess()
    {
        // Hide the Door object from your hierarchy completely!
        if (targetVaultDoor != null)
        {
            targetVaultDoor.SetActive(false);
            Debug.Log($"{targetVaultDoor.name} deactivated. Path clear!");
        }

        if (activeTerminal != null)
        {
            activeTerminal.OnPuzzleComplete();
        }

        gameObject.SetActive(false);
    }
}