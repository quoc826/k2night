using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using System.Security.Cryptography;
using Unity.VisualScripting;

public class Inventory : MonoBehaviour
{
    public ItemSO woodItem;
    public ItemSO axeItem;
    public GameObject hotBarObj;
    public GameObject inventorySlotParent;

    public Image dragIcon;
    private Slot draggedSlot = null;
    private bool isDragging = false;
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

        StartDrag();
        EndDrag();
        UpdateDragItemPosition();
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

    private void StartDrag()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Slot hovered = GetHoveredSlot();
            if (hovered != null && hovered.HasItem())
            {
                draggedSlot = hovered;
                isDragging = true;
                dragIcon.sprite = hovered.GetItem().icon;
                dragIcon.color = new Color(1, 1, 1, 0.5f);
                dragIcon.enabled = true;
            }
        }
    }

    private void EndDrag()
    {
        if (Input.GetMouseButtonUp(0))
        {
            Slot hovered = GetHoveredSlot();
            if (hovered != null)
            {
                HandleDrop(draggedSlot, hovered);
                dragIcon.enabled = false;
                draggedSlot = null;
                isDragging = false;
            }
        }


    }

    private void HandleDrop(Slot from, Slot to)
    {
        if (from == to) return;

        //stacking
        if (to.HasItem() && to.GetItem() == from.GetItem())
        {
            int max = to.GetItem().maxStackSize;
            int space = max - to.GetItemAmount();
            if (space > 0)
            {
                int move = Math.Min(space, from.GetItemAmount());
                to.SetItem(from.GetItem(), to.GetItemAmount() + move);

                if (from.GetItemAmount() <= 0)
                    from.ClearSlot();
                    return;
            }
        }

        //different items
        if (to.HasItem())
        {
            ItemSO tempItem = to.GetItem();
            int tempAmount = to.GetItemAmount();

            to.SetItem(from.GetItem(), from.GetItemAmount());
            from.SetItem(tempItem, tempAmount);
            return;
        }

        //Empty slot
        to.SetItem(from.GetItem(), from.GetItemAmount());
        from.ClearSlot();
    }

    private void UpdateDragItemPosition()
    {
        if (isDragging)
        {
            dragIcon.transform.position = Input.mousePosition;
        }
    }
    private Slot GetHoveredSlot()
    {
        foreach (Slot s in allSlots)
        {
            if (s.hovering)
            {
                return s;
            }
        }
        return null;
    }
}