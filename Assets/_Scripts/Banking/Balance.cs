using UnityEngine;
using TMPro; // Import TextMeshPro namespace

public class Balance : MonoBehaviour
{
    [Header("ATM Balance Refresh")]
    private TMP_Text balanceText;  // Automatically reference the TMP_Text component
    private PlayerBanking playerBanking;

    private void Start() {
        // Automatically get the TMP_Text component attached to the same GameObject
        balanceText = GetComponent<TMP_Text>();  

        if (balanceText == null) {
            Debug.LogError("No TMP_Text component found on this GameObject!");
        }

        GameObject player = GameObject.FindWithTag("Player");

        if (player != null) {
            playerBanking = player.GetComponent<PlayerBanking>();
            
            if (playerBanking != null) {
                Debug.Log("PlayerBanking component found on player object.");
            } else {
                Debug.LogError("PlayerBanking component not found on player object!");
            }
        } else {
            Debug.LogError("Player object not found!");
        }
    }

    private void Update() {
        if (playerBanking != null && balanceText != null) {
            // Assuming playerBanking.GetBalance() returns a float
            balanceText.text = "Balance: " + playerBanking.GetBalance().ToString(); // Update balance text on UI
        }
    }
}
