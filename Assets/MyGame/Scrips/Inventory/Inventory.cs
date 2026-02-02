using UnityEngine;
using System.Collections;
using System.Collections.Generic; 
using System; 

public class Inventory : MonoBehaviour
{
    public ItemSO woodItem;
    public ItemSO axeItem;
    public GameObject hotBarObj;
    public GameObject inventorySlotParent;

    private List<Slot> inventorySlots = new List<Slot>();
    private List<Slot> hotBarSlots = new List<Slot>();
    private List<Slot> allSlots = new List<Slot>();

    private void Awake()
    {
        inventorySlots.AddRange(inventorySlotParent.GetComponentsInChildren<Slot>());
        hotBarSlots.AddRange(hotBarObj.GetComponentsInChildren<Slot>());
        
        allSlots.AddRange(inventorySlots);
        allSlots.AddRange(hotBarSlots);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            AddItem(woodItem, 5);
        }
    }

    public void AddItem(ItemSO itemToAdd, int amount)
    {
        int remanining = amount;

        foreach (Slot slot in allSlots)
        {
            if (slot.HasItem() && slot.GetItem() == itemToAdd)
            {
                int currentAmount = slot.GetItemAmount();
                int maxStack = itemToAdd.maxStackSize;

                if (currentAmount < maxStack)
                {
                    int spaceLeft = maxStack - currentAmount;
                    int amountToAdd = Math.Min(spaceLeft, remanining);

                    slot.SetItem(itemToAdd, currentAmount + amountToAdd);
                    remanining -= amountToAdd;

                    if (remanining <= 0) return;
                }
            }
        }

        foreach (Slot slot in allSlots)
        {
            if (!slot.HasItem())
            {
                int amountToPlace = Math.Min(itemToAdd.maxStackSize, remanining);
                slot.SetItem(itemToAdd, amountToPlace);
                
                remanining -= amountToPlace; 

                if (remanining <= 0) return;
            }
        }

        if (remanining > 0)
        {
            Debug.Log("Không đủ chỗ trống trong túi đồ!");
        }
    }
}