using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance;
    public int playerMaxSlots = 5;
    public List<ItemData> items = new List<ItemData>();
    public UnityEvent onInventoryChanged;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public bool AddItem(ItemData newItem)
    {
        if(items.Count >= playerMaxSlots)
        {
            Debug.Log("Your Inventory is Full");
            return false;
        }

        items.Add(newItem);
        onInventoryChanged?.Invoke();
        Debug.Log("picked up " + newItem.itemName);
        return true;
    }

    public void RemoveItem(ItemData itemToRemove)
    {
        if(items.Contains(itemToRemove));
        {
            items.Remove(itemToRemove);
            onInventoryChanged?.Invoke();
        }
    }
}
