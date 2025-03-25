using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Add to a GameObject with a trigger collider to create a lock that can only be unlocked if the player has the requisite item(s) in their inventory.
/// </summary>
public class Lock : MonoBehaviour
{
    [Tooltip("Enter the tag name that should register collisions.")]
    [SerializeField] private string tagName;

    [Tooltip("The name of the item that the lock requires.")]
    [SerializeField] private string requiredItemName;

    [Tooltip("The quantity of the item required.")]
    [SerializeField] private int requiredItemCount;

    [Tooltip("Whether a button press should be required to unlock the lock. If false, it will check automatically on the trigger collision.")]
    [SerializeField] private bool requireButtonPress;

    [Tooltip("The key input that the script is listening for.")]
    [SerializeField] private KeyCode keyToPress = KeyCode.E;

    [Tooltip("Whether the items required for the lock should be deleted when unlocking.")]
    [SerializeField] private bool deleteItemsWhenUsed;

    [Space(20)]
    [SerializeField] private UnityEvent onUnlocked, onUnlockFailed;

    private Inventory inventory;

    private void Update()
    {
        if (Input.GetKeyDown(keyToPress) && requireButtonPress && inventory != null)
        {
            CheckUnlock();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!string.IsNullOrEmpty(tagName) && collision.CompareTag(tagName))
        {
            if (collision.gameObject.TryGetComponent(out Inventory inv))
            {
                inventory = inv;

                if (!requireButtonPress)
                {
                    CheckUnlock();
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!string.IsNullOrEmpty(tagName) && collision.CompareTag(tagName))
        {
            if (collision.gameObject.TryGetComponent(out Inventory inv) && inv == inventory)
            {
                inventory = null;
            }
        }
    }

    // Check if the required items are in the inventory, then handle the unlocking
    private void CheckUnlock()
    {
        int count = inventory.GetItemCount(requiredItemName);
        if (count >= requiredItemCount)
        {
            if (deleteItemsWhenUsed)
            {
                inventory.DeleteItemFromInventory(requiredItemName, requiredItemCount);
            }

            onUnlocked.Invoke();
        }
        else
        {
            onUnlockFailed.Invoke();
        }
    }
}
