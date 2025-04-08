// Unity Starter Package - Version 1
// University of Florida's Digital Worlds Institute
// Written by Logan Kemper

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// Add to the player to allow them to pick up items and display them on the UI.
/// </summary>
public class Inventory : MonoBehaviour
{
    [Tooltip("Optional: Assign a layout group on the UI to display the current items in the inventory.")]
    [SerializeField] private LayoutGroup layoutGroup;

    [Tooltip("The maximum number of items that can be in the inventory at once.")]
    [SerializeField] private int maxItems = 10;

    [Tooltip("If true, a trigger collision with an item will automatically attempt to pick it up.")]
    [SerializeField] private bool autoPickUpItems = true;

    [Tooltip("Control whether the inventory should persist between scenes. If false, the inventory will be cleared on Start.")]
    [SerializeField] private bool persistentInventory = false;

    [Space(20)]
    [SerializeField] private UnityEvent onItemPickedUp;

    private static List<ItemData> items = new();

    private void Start()
    {
        if (persistentInventory)
        {
            for (int i = 0; i < items.Count; i++)
            {
                AddItemToUI(items[i]);
            }
        }
        else
        {
            ClearInventory();
        }
    }

    // Check trigger collisions for items (2D physics)
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if items can be automatically picked up and the triggering GameObject has an Item component
        if (autoPickUpItems && collision.gameObject.TryGetComponent(out Item item))
        {
            PickUpItem(item);
        }
    }

    // Check trigger collisions for items (3D physics)
    private void OnTriggerEnter(Collider other)
    {
        // Check if items can be automatically picked up and the triggering GameObject has an Item component
        if (autoPickUpItems && other.gameObject.TryGetComponent(out Item item))
        {
            PickUpItem(item);
        }
    }

    public void PickUpItem(Item item)
    {
        // Attempt to pick up the item
        bool pickedUp = TryToPickUpItem(item);

        // Destroy the item if it has been successfully picked up and it's set to be destroyed on pickup
        if (pickedUp && item.destroyOnPickup)
        {
            Destroy(item.gameObject);
        }
    }

    // Returns true or false based on whether it was successfully picked up
    private bool TryToPickUpItem(Item item)
    {
        // Don't pick up item if inventory is full
        if (items.Count >= maxItems)
        {
            return false;
        }

        // Don't pick up item if it's unique and there's already one in the inventory
        if (item.isUnique)
        {
            ItemData existingItem = items.Find(existingItem => existingItem.name == item.itemData.name);
            if (existingItem != null)
            {
                return false;
            }
        }

        AddItemToInventory(item.itemData);

        // Play the item's pickup sound
        if (item.pickupSound != null)
        {
            AudioSource.PlayClipAtPoint(item.pickupSound, transform.position);
        }

        onItemPickedUp.Invoke();

        return true;
    }

    public void ClearInventory()
    {
        // Remove all entries from the item list
        items.Clear();

        if (layoutGroup != null)
        {
            // Destroy all children of the layout group
            for (int i = layoutGroup.transform.childCount - 1; i >= 0; i--)
            {
                Destroy(layoutGroup.transform.GetChild(i).gameObject);
            }
        }
    }

    // Add an item to the inventory by its ItemData
    public void AddItemToInventory(ItemData itemData)
    {
        items.Add(itemData);
        AddItemToUI(itemData);
    }

    // Add an item to the inventory by a name and sprite
    public void AddItemToInventory(string name = "", Sprite sprite = null)
    {
        ItemData itemData = new();
        itemData.name = name;
        itemData.sprite = sprite;
        items.Add(itemData);

        AddItemToUI(itemData);
    }

    private void AddItemToUI(ItemData itemData)
    {
        if (layoutGroup == null)
        {
            // Don't add to the UI if the layout group is not assigned
            return;
        }

        GameObject newItem = new(itemData.name);
        newItem.AddComponent<Image>().sprite = itemData.sprite;
        newItem.transform.SetParent(layoutGroup.transform);
        newItem.transform.localScale = Vector3.one;
    }

    // Delete an item from the inventory and layout group, given its name
    public void DeleteItemFromInventory(string targetName)
    {
        ItemData item = items.Find(item => item.name == targetName);
        items.Remove(item);

        if (layoutGroup != null)
        {
            foreach (Transform child in layoutGroup.transform)
            {
                if (child.name == targetName)
                {
                    Destroy(child.gameObject);
                    break;
                }
            }
        }
    }

    // Delete a given number of items from the inventory and layout group, given its name
    public void DeleteItemFromInventory(string targetName, int count)
    {
        if (count <= 0)
        {
            return;
        }

        // Find all matching items
        List<ItemData> matchingItems = items.FindAll(item => item.name == targetName);

        // Determine how many to remove (limit to available items)
        int removeCount = Mathf.Min(count, matchingItems.Count);

        // Remove items from the inventory list
        for (int i = 0; i < removeCount; i++)
        {
            items.Remove(matchingItems[i]);
        }

        // Remove corresponding UI elements from the layout group
        if (layoutGroup != null)
        {
            int removedUI = 0;
            foreach (Transform child in layoutGroup.transform)
            {
                if (child.name == targetName)
                {
                    Destroy(child.gameObject);
                    removedUI++;

                    if (removedUI >= removeCount)
                    {
                        // Stop once enough items are removed
                        break;
                    }
                }
            }
        }
    }

    public bool HasItem(string itemName)
    {
        ItemData item = items.Find(item => item.name == itemName);

        return item != null;
    }

    public int GetItemCount(string itemName)
    {
        int count = 0;

        // Iterate through the list of items and check if the item's name matches the given name
        foreach (ItemData item in items)
        {
            if (item.name == itemName)
            {
                count++;
            }
        }

        return count;
    }

    [RuntimeInitializeOnLoadMethod]
    private static void InitializeOnLoad()
    {
        // Item list will be reset when the game first starts
        items = new();
    }
}