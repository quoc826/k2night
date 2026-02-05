using UnityEngine;

public class PlayerPickup : MonoBehaviour
{
    private Item currentItem; 

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && currentItem != null)
        {
            Inventory.Instance.AddItem(currentItem.item, currentItem.amount);

            Destroy(currentItem.gameObject);
            currentItem = null;
        }
    }

    private void OnTriggerEnter2D(Collider2D coll)
    {
        Item item = coll.GetComponent<Item>();
        if (item != null)
        {
            currentItem = item;
        }
    }

    private void OnTriggerExit2D(Collider2D coll)
    {
        Item item = coll.GetComponent<Item>();
        if (item != null && item == currentItem)
        {
            currentItem = null;
        }
    }
}