using UnityEngine;

public class InvPopUp : MonoBehaviour
{
    [Header("References")]
    [SerializeField] public GameObject InventoryCanvas;
    [SerializeField] public MonoBehaviour PlayerController; // Reference to the component, not the GameObject

    void Update()
    {
        bool isActive = !InventoryCanvas.activeSelf;
        if (Input.GetKeyDown(KeyCode.I))
        {
            InventoryCanvas.SetActive(isActive);
            PlayerController.enabled = !isActive; // Disable PlayerController when Inventory is open

            Cursor.visible = isActive;
            Cursor.lockState = isActive ? CursorLockMode.None : CursorLockMode.Locked;
        } else if (Input.GetKeyDown(KeyCode.Escape) && InventoryCanvas.activeSelf)
        {
            InventoryCanvas.SetActive(false);
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            PlayerController.enabled = true; // Enable PlayerController when Inventory is closed
        }
    }
}
