using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{   
    #region References
    [Header("References")]
    [SerializeField] private GameObject Player;
    [SerializeField] private GameObject DroppedItemsParrent;
    [SerializeField] private GameObject DroppedItemPrefab;
    [SerializeField] private Material AppleMaterial;
    [SerializeField] private Material SwordMaterial;
    [SerializeField] private Material CoalMaterial;
    [SerializeField] private GameObject InventoryPanel;
    [SerializeField] private GameObject itemCursor;   
    [SerializeField] private GameObject slotHolder;
    [SerializeField] private GameObject hotbarSlotHolder;
    [SerializeField] private int selectedSlotIndex = 0;
    [SerializeField] private GameObject hotbarSelector;


    [Header("Debug")]
    [SerializeField] private ItemClass itemToAdd;
    [SerializeField] private ItemClass itemToRemove;
    [SerializeField] private SlotClass[] startingItems;
    #endregion

    private SlotClass[] items;

    private GameObject[] slots;
    private GameObject[] hotbarSlots;

    private SlotClass movingSlot;
    private SlotClass tempSlot;
    private SlotClass originalSlot;
    bool isMovingItem;
    public ItemClass selectedItem;



    private void Start() {
        slots = new GameObject[slotHolder.transform.childCount];
        items = new SlotClass[slots.Length];  

        hotbarSlots = new GameObject[hotbarSlotHolder.transform.childCount];
        for (int i = 0; i < hotbarSlots.Length; i++) {
            hotbarSlots[i] = hotbarSlotHolder.transform.GetChild(i).gameObject;
        }

        for (int i = 0; i < items.Length; i++) {
            items[i] = new SlotClass();
        }
        for (int i = 0; i < startingItems.Length; i++) {
            items[i] = startingItems[i];
        }

        //set all the slots
        for (int i = 0; i < slotHolder.transform.childCount; i++) {
            slots[i] = slotHolder.transform.GetChild(i).gameObject;
        }
        RefreshUI();
        Add(itemToAdd, 1);
        Remove(itemToRemove);
    }

    private void Update() {
        itemCursor.SetActive(isMovingItem);
        itemCursor.transform.position = Input.mousePosition;
        if (isMovingItem) {
            itemCursor.GetComponent<Image>().sprite = movingSlot.GetItem().itemIcon;
        }

        if (Input.GetMouseButtonDown(0)) { //we left clicked
            //find the closest slot (slot we clicked)
            if (isMovingItem) {
                EndItemMove();
            } else {
                BeginItemMove();
            }
        } else if (Input.GetMouseButtonDown(1)) { //We right clicked
            if (isMovingItem) {
                EndItemMove_Single();
            } else {
                BeginItemMove_Half();
            }
        }

        if (Input.GetAxis("Mouse ScrollWheel") > 0) { // scroll up
            selectedSlotIndex = Mathf.Clamp(selectedSlotIndex+1, 0, hotbarSlots.Length-1);
        } else if (Input.GetAxis("Mouse ScrollWheel") < 0) { // scroll down
            selectedSlotIndex = Mathf.Clamp(selectedSlotIndex-1, 0, hotbarSlots.Length-1);
        }

        hotbarSelector.transform.position = hotbarSlots[selectedSlotIndex].transform.position;
        selectedItem = items[selectedSlotIndex + (hotbarSlots.Length*3)].GetItem();
        // if (Input.GetKeyDown(KeyCode.Alpha1)) {
        //     selectedSlotIndex = 0;
        // } else if (Input.GetKeyDown(KeyCode.Alpha2)) {
        //     selectedSlotIndex = 1;
        // } else if (Input.GetKeyDown(KeyCode.Alpha3)) {
        //     selectedSlotIndex = 2;
        // } else if (Input.GetKeyDown(KeyCode.Alpha4)) {
        //     selectedSlotIndex = 3;
        // } else if (Input.GetKeyDown(KeyCode.Alpha5)) {
        //     selectedSlotIndex = 4;
        // } else if (Input.GetKeyDown(KeyCode.Alpha6)) {
        //     selectedSlotIndex = 5;
        // } else if (Input.GetKeyDown(KeyCode.Alpha7)) {
        //     selectedSlotIndex = 6;
        // } else if (Input.GetKeyDown(KeyCode.Alpha8)) {
        //     selectedSlotIndex = 7;
        // } else if (Input.GetKeyDown(KeyCode.Alpha9)) {
        //     selectedSlotIndex = 8;
        // }
    } 

    #region Inventory Utils
    public void RefreshUI() {
        for (int i = 0; i < slots.Length; i++) {
            try {
                slots[i].transform.GetChild(0).GetComponent<Image>().enabled = true;
                slots[i].transform.GetChild(0).GetComponent<Image>().sprite = items[i].GetItem().itemIcon;
                if (items[i].GetItem().isStackable) {
                    slots[i].transform.GetChild(1).GetComponent<Text>().text = items[i].GetQuantity()+"";     
                } else {
                    slots[i].transform.GetChild(1).GetComponent<Text>().text = "";
                } 
            }
            catch {
                slots[i].transform.GetChild(0).GetComponent<Image>().sprite = null;   
                slots[i].transform.GetChild(0).GetComponent<Image>().enabled = false;
                slots[i].transform.GetChild(1).GetComponent<Text>().text = "";
            }
        }
        RefreshHotbar();
    }
    
    public void RefreshHotbar() {
        for (int i = 0; i < hotbarSlots.Length; i++) {
            try {
                hotbarSlots[i].transform.GetChild(0).GetComponent<Image>().enabled = true;
                hotbarSlots[i].transform.GetChild(0).GetComponent<Image>().sprite = items[i + (hotbarSlots.Length*3)].GetItem().itemIcon;
                if (items[i + (hotbarSlots.Length*3)].GetItem().isStackable) {
                    hotbarSlots[i].transform.GetChild(1).GetComponent<Text>().text = items[i + (hotbarSlots.Length*3)].GetQuantity()+"";     
                } else {
                    hotbarSlots[i].transform.GetChild(1).GetComponent<Text>().text = "";
                } 
            }
            catch {
                hotbarSlots[i].transform.GetChild(0).GetComponent<Image>().sprite = null;   
                hotbarSlots[i].transform.GetChild(0).GetComponent<Image>().enabled = false;
                hotbarSlots[i].transform.GetChild(1).GetComponent<Text>().text = "";
            }
        }
    }

    public bool Add(ItemClass item, int quantity) {
        Debug.Log("Adding item: " + item.itemName + " x" + quantity);
        //items.Add(item);
        SlotClass slot = Contains(item);
        if (slot != null && slot.GetItem().isStackable) {
            slot.AddQuantity(quantity);
        } else {
                for (int i = 0; i < items.Length; i++) {
                if (items[i].GetItem() == null) {//this is an empty slot 
                    items[i].AddItem(item, quantity);
                    break;
                }
            }

            /*if (items.Count < slots.Length) {
                items.Add(new SlotClass(item, 1));
            } else {
                return false;
            } */
        }
        RefreshUI();
        return true;
    }

    public bool Remove(ItemClass item) {
        //items.Remove(item);
        SlotClass temp = Contains(item);
        if (temp != null) {
            if (temp.GetQuantity() > 1) {
                temp.SubQuantity(1);
            } else {
                int slotToRemoveIndex = 0;
                for (int i =0; i < items.Length; i++) {
                    if (items[i].GetItem() == item) {
                        slotToRemoveIndex = i;
                        break;
                    }
                }
                items[slotToRemoveIndex].Clear();
            }
        } else {
            return false;
        }
        RefreshUI();
        return true;
    }

    public SlotClass Contains(ItemClass item) {
        for (int i = 0; i < items.Length; i++) {
            if (items[i].GetItem() == item) {
                return items[i];
            }
        }

        return null;
    }
    #endregion

    #region Moving Stuff
    private bool BeginItemMove() {
        originalSlot = GetClosestSlot();
        if (originalSlot == null || originalSlot.GetItem() == null) {
            return false; //no item to move
        }
        movingSlot = new SlotClass(originalSlot);
        originalSlot.Clear();
        RefreshUI();
        isMovingItem = true;
        return true;
    }

    private bool BeginItemMove_Half() {
        originalSlot = GetClosestSlot();
        if (originalSlot == null || originalSlot.GetItem() == null) {
            return false; //no item to move
        }
        movingSlot = new SlotClass(originalSlot.GetItem(), Mathf.CeilToInt(originalSlot.GetQuantity()/2f));
        originalSlot.SubQuantity(Mathf.CeilToInt(originalSlot.GetQuantity()/2f));
        if (originalSlot.GetQuantity()==0) {
            originalSlot.Clear();
        }
        isMovingItem = true;
        RefreshUI();
        return true;
    }

    private bool EndItemMove() {
        RectTransform panelRect = InventoryPanel.GetComponentInChildren<RectTransform>(); // Get the InventoryPanel RectTransform
        Vector2 mousePosition = Input.mousePosition;
        originalSlot = GetClosestSlot();
        if (originalSlot == null) { //Outside of inv? Drop item
            if (!RectTransformUtility.RectangleContainsScreenPoint(panelRect, mousePosition)) {
                //ItemDropped(movingSlot.GetItem(), movingSlot.GetQuantity());
                        
                Vector3 DropPos = Player.transform.position + Player.transform.forward * 3 + Vector3.down * 0.65f;
                GameObject droppedItem = Instantiate(DroppedItemPrefab, DropPos, Quaternion.identity);
                droppedItem.transform.SetParent(DroppedItemsParrent.transform); // Parent it under DroppedItems

                Debug.Log("Dropped Item Name: " + movingSlot.GetItem().name);

                Renderer itemRenderer = droppedItem.GetComponentInChildren<Renderer>(); // Finds Renderer in children

                if (itemRenderer != null) {
                    if (movingSlot.GetItem().name == "Apple") {
                        itemRenderer.material = AppleMaterial;
                    }
                    else if (movingSlot.GetItem().name == "Sword") {
                        itemRenderer.material = SwordMaterial;
                    }
                    else if (movingSlot.GetItem().name == "Coal") {
                        itemRenderer.material = CoalMaterial;
                    } 
                }
                else {
                    Debug.LogError("Renderer not found on " + droppedItem.name);
                }
                
                droppedItem.GetComponent<DroppedItem>().SetItem(movingSlot.GetItem());
                droppedItem.GetComponent<DroppedItem>().SetQuantity(movingSlot.GetQuantity());

                movingSlot.Clear(); // Clear the moving slot
                isMovingItem = false;
                RefreshUI();
                return false;
            }
            Add(movingSlot.GetItem(), movingSlot.GetQuantity()); //put it back in the inventory
            movingSlot.Clear(); //clear the moving slot
        } else {
            if (originalSlot.GetItem() != null) { //there is item under cursor
            if(originalSlot.GetItem() == movingSlot.GetItem()) { //same item, just move quantity
                if (originalSlot.GetItem().isStackable) {
                    originalSlot.AddQuantity(movingSlot.GetQuantity());
                    movingSlot.Clear(); 
                } else {
                    return false;
                }
            } else { //different item, swap them
                tempSlot = new SlotClass(originalSlot); //a = b
                originalSlot.AddItem(movingSlot.GetItem(), movingSlot.GetQuantity());//b = c
                movingSlot.AddItem(tempSlot.GetItem(), tempSlot.GetQuantity());//a = c
                
                RefreshUI();
                return true;
            }
        } else { //place item as usual
            originalSlot.AddItem(movingSlot.GetItem(), movingSlot.GetQuantity());
            movingSlot.Clear(); 
        }
        }
        isMovingItem = false;
        RefreshUI();
        return true;
    }

    private bool EndItemMove_Single() {
        RectTransform panelRect = InventoryPanel.GetComponentInChildren<RectTransform>();
        Vector2 mousePosition = Input.mousePosition;

        // Check if mouse is outside inventory
        if (!RectTransformUtility.RectangleContainsScreenPoint(panelRect, mousePosition)) {
            // Drop one item
            Vector3 DropPos = Player.transform.position + Player.transform.forward * 3 + Vector3.down * 0.65f;
            GameObject droppedItem = Instantiate(DroppedItemPrefab, DropPos, Quaternion.identity);
            droppedItem.transform.SetParent(DroppedItemsParrent.transform);

            Debug.Log("Dropped Item Name: " + movingSlot.GetItem().name);

            Renderer itemRenderer = droppedItem.GetComponentInChildren<Renderer>();
            if (itemRenderer != null) {
                if (movingSlot.GetItem().name == "Apple") {
                    itemRenderer.material = AppleMaterial;
                } else if (movingSlot.GetItem().name == "Sword") {
                    itemRenderer.material = SwordMaterial;
                } else if (movingSlot.GetItem().name == "Coal") {
                    itemRenderer.material = CoalMaterial;
                }
            }

            droppedItem.GetComponent<DroppedItem>().SetItem(movingSlot.GetItem());
            droppedItem.GetComponent<DroppedItem>().SetQuantity(1);

            // Subtract from movingSlot
            movingSlot.SubQuantity(1);
            if (movingSlot.GetQuantity() < 1) {
                movingSlot.Clear();
                isMovingItem = false;
            }

            RefreshUI();
            return true;
        }

        // Handle adding one to a slot
        originalSlot = GetClosestSlot();
        if (originalSlot == null) {
            return false;
        }
        if (originalSlot.GetItem() != null && originalSlot.GetItem() != movingSlot.GetItem()) {
            return false;
        }

        movingSlot.SubQuantity(1);
        if (originalSlot.GetItem() != null && originalSlot.GetItem() == movingSlot.GetItem()) {
            originalSlot.AddQuantity(1);
        } else {
            originalSlot.AddItem(movingSlot.GetItem(), 1);
        }

        if (movingSlot.GetQuantity() < 1) {
            movingSlot.Clear();
            isMovingItem = false;
        }

        RefreshUI();
        return true;
    }

    #endregion

    #region Dropping Items on ground
    /*private void ItemDropped(ItemClass item, int quantity) {
        Debug.Log(item + " " + quantity);
        Debug.Log("Item type: " + item.GetType().Name);


        Vector3 DropPos = Player.transform.position + Player.transform.forward * 3 + Vector3.down * 0.65f;
        GameObject droppedItem = Instantiate(DroppedItemPrefab, DropPos, Quaternion.identity);
        droppedItem.transform.SetParent(DroppedItemsParrent.transform); // Parent it under DroppedItems

        Debug.Log("Dropped Item Name: " + item.name);

        Renderer itemRenderer = droppedItem.GetComponentInChildren<Renderer>(); // Finds Renderer in children

        if (itemRenderer != null) {
            if (item.name == "Apple") {
                itemRenderer.material = AppleMaterial;
            }
            else if (item.name == "Sword") {
                itemRenderer.material = SwordMaterial;
            }
            else if (item.name == "Coal") {
                itemRenderer.material = CoalMaterial;
            } 
        }
        else {
            Debug.LogError("Renderer not found on " + droppedItem.name);
        }


        droppedItem.GetComponent<DroppedItem>().SetItem(item.name);
        droppedItem.GetComponent<DroppedItem>().SetQuantity(quantity);  
        droppedItem.GetComponent<DroppedItem>().SetType(item.GetType().Name); // Set the item type

    }*/

    /*private void ItemDropped(ItemClass item, int quantity) {
        Debug.Log(item + " " + quantity);
        Debug.Log("Item type: " + item.GetType().Name);

        Vector3 DropPos = Player.transform.position + Player.transform.forward * 3 + Vector3.down * 0.65f;
        GameObject droppedItemObj = Instantiate(DroppedItemPrefab, DropPos, Quaternion.identity);
        droppedItemObj.transform.SetParent(DroppedItemsParrent.transform); // Parent it under DroppedItems

        Debug.Log("Dropped Item Name: " + item.name);

        Renderer itemRenderer = droppedItemObj.GetComponentInChildren<Renderer>();

        if (itemRenderer != null) {
            if (item.name == "Apple") {
                itemRenderer.material = AppleMaterial;
            } else if (item.name == "Sword") {
                itemRenderer.material = SwordMaterial;
            } else if (item.name == "Coal") {
                itemRenderer.material = CoalMaterial;
            }
        } else {
            Debug.LogError("Renderer not found on " + droppedItemObj.name);
        }

        // Apply values using the new API in DroppedItem
        DroppedItem dropped = droppedItemObj.GetComponent<DroppedItem>();
        dropped.SetItem(item);
        dropped.SetQuantity(quantity);
    }*/

    #endregion

    #region Pickup Items
    //public void PickupItem(string itemName, int quantity) {
        //Add(item, quantity);
    //}
    public void PickupItem(ItemClass item, int quantity) {
        
        Add(item, quantity);
        Debug.Log($"Picked up {item.itemName} x{quantity}");
    }

    #endregion

    private SlotClass GetClosestSlot() {
        for (int i = 0; i < slots.Length; i++) {
            if (Vector2.Distance(slots[i].transform.position, Input.mousePosition) <= 32) {
                return items[i];
            }
        }

        return null;
    }
}