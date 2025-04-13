using UnityEngine;

public class PlayerBanking : MonoBehaviour
{
    [Header("Player Banking")]
    [SerializeField] private float playerBalance;  // actual field
    [SerializeField] private bool HasAccount;      // actual field

    // ✅ Proper public getters
    public float GetBalance() => playerBalance;
    public bool HasAnAccount() => HasAccount;

    public void SetBalance(float amount) {
        playerBalance = amount;
    }
    public void SetAccount(bool hasAccount) {
        HasAccount = hasAccount;
    }
    public void AddToBalance(float amount) {
        playerBalance += amount;
    }
    public void SubtractFromBalance(float amount) {
        playerBalance -= amount;
    }
}
