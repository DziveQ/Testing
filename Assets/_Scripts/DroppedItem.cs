using UnityEngine;
using TMPro;

public class DroppedItem : MonoBehaviour
{
    [Header("Pickup Settings")]
    [SerializeField] private float pickupRange = 3f;
    private Canvas worldCanvas;
    private TextMeshProUGUI pickupText;
    private Transform player;
    private InventoryManager inventory;

    [Header("Item Data")]
    [SerializeField] private ItemClass item;
    [SerializeField] private int quantity;


    private void Start()
    {
        inventory = FindFirstObjectByType<InventoryManager>(); // Find InventoryManager in scene
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        // Auto-find canvas and text
        worldCanvas = GetComponentInChildren<Canvas>();
        pickupText = GetComponentInChildren<TextMeshProUGUI>();

        if (worldCanvas != null)
        {
            // Position the canvas slightly above the item
            worldCanvas.transform.localPosition = new Vector3(0, 0.5f, 0);
        }

        UpdateText();
}


    private void Update() {
        if (player == null || worldCanvas == null) return;

        // Face the player
        worldCanvas.transform.rotation = Quaternion.LookRotation(worldCanvas.transform.position - player.position);

        // Toggle visibility based on range
        float dist = Vector3.Distance(transform.position, player.position);
        worldCanvas.enabled = dist <= pickupRange;

        if (dist <= pickupRange && Input.GetKeyDown(KeyCode.E))
        {
            if (item != null && inventory != null) {
                Debug.Log($"Picked up {item.itemName} x{quantity}");
                inventory.PickupItem(item, quantity);
            }
            Destroy(gameObject); // Destroy the item after pickup
        }

    }

    /*
    #region Getters and Setters
    public void SetItem(string name) {
        itemName = name;
        UpdateText();
    }

    public void SetQuantity(int quantity) {
        itemQuantity = quantity;
        UpdateText();
    }

    public void SetType(string type) {
        itemType = type;
    }
    #endregion
    */

    public void SetItem(ItemClass _item) {
        item = _item;
        UpdateText();
    }

    public void SetQuantity(int _quantity) {
        quantity = _quantity;
        UpdateText();
    }

    public ItemClass GetItem() {
        return item;
    }

    public int GetQuantity() {
        return quantity;
    }

    private void UpdateText()
    {
        if (pickupText != null && item != null)
        {
            pickupText.text = $"{item.itemName} x{quantity}\n<color=yellow>Press E to pick up</color>";
        }
    }

}
