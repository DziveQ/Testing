using UnityEngine;
using UnityEngine.UI; // Make sure to include this for UI components
using TMPro;

public class AtmScreen : MonoBehaviour {
    [Header("Screens")]
    [SerializeField] public GameObject homeScreen;
    [SerializeField] public GameObject withdrawScreen;
    [SerializeField] public GameObject depositScreen;
    [SerializeField] public GameObject cardOptionsScreen;
    [SerializeField] public GameObject balanceScreen;
    [SerializeField] public GameObject transferScreen;
    
    public InputField WithdrawAmountField;
    private PlayerBanking playerBanking;
    private InventoryManager inventory;
    public GameObject Atm;

    // Reference to the Cash Item Scriptable Object
    [SerializeField] private ItemClass cashItem;


    public void Start() {
        GameObject player = GameObject.FindWithTag("Player");
        inventory = FindFirstObjectByType<InventoryManager>(); // Find InventoryManager in scene (to add cash item and remove cash item)

        if (player != null) {
            playerBanking = player.GetComponent<PlayerBanking>(); // Get the PlayerBanking component from the player object to get account and balance info
            
            if (playerBanking != null) {
                Debug.Log("PlayerBanking component found on player object.");
            } else {
                Debug.LogError("PlayerBanking component not found on player object!");
            }
        } else {
            Debug.LogError("Player object not found!");
        }

        ShowHomeScreen(); //Default screen when ATM is opened
    }


    public void ShowWithdrawScreen() { 
        RefreshBalance();

        homeScreen.SetActive(false);
        withdrawScreen.SetActive(true); //This
        depositScreen.SetActive(false);
        balanceScreen.SetActive(false);
        cardOptionsScreen.SetActive(false);
        transferScreen.SetActive(false);
    }

    public void ShowHomeScreen() {
        RefreshBalance();

        homeScreen.SetActive(true); //This
        withdrawScreen.SetActive(false);
        depositScreen.SetActive(false);
        balanceScreen.SetActive(false);
        cardOptionsScreen.SetActive(false);
        transferScreen.SetActive(false);
    }

    public void ShowDepositScreen() {
        RefreshBalance();

        homeScreen.SetActive(false);
        withdrawScreen.SetActive(false);
        depositScreen.SetActive(true); //This
        balanceScreen.SetActive(false);
        cardOptionsScreen.SetActive(false);
        transferScreen.SetActive(false);
    }

    public void ExitButton() {
        if (Atm != null) {
            var atmScript = Atm.GetComponent<Atm>(); // Replace AtmScript with the actual script name on the Atm GameObject
            if (atmScript != null) {
                Debug.Log("ExitATMView is being called.");
                atmScript.ExitATMView();
            } else {
                Debug.LogError("AtmScript not found on Atm object!");
            }
        } else {
            Debug.LogError("Atm object is not assigned!");
        }
    }

    public void ShowBalanceScreen() {
        homeScreen.SetActive(false);
        withdrawScreen.SetActive(false);
        depositScreen.SetActive(false);
        cardOptionsScreen.SetActive(false);
        balanceScreen.SetActive(true); //This
        transferScreen.SetActive(false);
    }

    public void ShowCardOptionsScreen() {
        homeScreen.SetActive(false);
        withdrawScreen.SetActive(false);
        depositScreen.SetActive(false);
        cardOptionsScreen.SetActive(true); //This
        balanceScreen.SetActive(false);
        transferScreen.SetActive(false);
    }

    public void ShowTransferScreen() {
        homeScreen.SetActive(false);
        withdrawScreen.SetActive(false);
        depositScreen.SetActive(false);
        cardOptionsScreen.SetActive(false);
        balanceScreen.SetActive(false);
        transferScreen.SetActive(true); //This
    }

    public void RefreshBalance() { //prob delete this function and just use the one in Balance.cs
        float balance = playerBanking.GetBalance();
        bool hasAccount = playerBanking.HasAnAccount();
        Debug.Log("Player Balance: " + balance + ", Has Account: " + hasAccount);
    }

    public void WithdrawFunction() { // Called when the withdraw button is pressed
        int withdrawAmount = int.Parse(WithdrawAmountField.text); // Get the amount to withdraw from the input field
        Debug.Log("Withdraw Amount: " + withdrawAmount);

        // Subtract the withdrawn amount from the player's balance
        playerBanking.SubtractFromBalance(withdrawAmount);

        Debug.Log(withdrawAmount + " withdrawn from account. Adding to inventory.");

        // Add the cash item to the player's inventory with the correct quantity
        inventory.Add(cashItem, 100);
        Debug.Log("Withdrawn: " + withdrawAmount + ", New Balance: " + playerBanking.GetBalance());
    }

    public void DepositFunction() { // Called when the withdraw button is pressed
        int withdrawAmount = int.Parse(WithdrawAmountField.text); // Get the amount to withdraw from the input field
        Debug.Log("Withdraw Amount: " + withdrawAmount);

        // Subtract the withdrawn amount from the player's balance
        playerBanking.SubtractFromBalance(withdrawAmount);

        Debug.Log(withdrawAmount + " withdrawn from account. Adding to inventory.");

        // Add the cash item to the player's inventory with the correct quantity
        inventory.Add(cashItem, 100);
        Debug.Log("Withdrawn: " + withdrawAmount + ", New Balance: " + playerBanking.GetBalance());
    }
}