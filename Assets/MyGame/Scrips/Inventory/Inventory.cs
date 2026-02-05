using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using System.Security.Cryptography;
using Unity.VisualScripting;
using TMPro;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance;

    [Header("--- Items Data ---")]
    public ItemSO woodItem;
    public ItemSO axeItem;

    [Header("--- UI References ---")]
    public GameObject container;
    public GameObject hotBarObj;
    public GameObject inventorySlotParent;

    [Header("--- Drag & Drop System ---")]
    public Image dragIcon;

    [Header("--- Item Description UI ---")]
    public GameObject itemDescriptionParent;
    public Image itemDescriptionImage;
    public TextMeshProUGUI descriptionIteamNameTxt;
    public TextMeshProUGUI itemdescriptionTxt;

    [Header("--- Debug/Internal State ---")]
    [SerializeField] private Slot draggedSlot = null;
    [SerializeField] private bool isDragging = false;

    private List<Slot> inventorySlots = new List<Slot>();
    private List<Slot> hotBarSlots = new List<Slot>();
    private List<Slot> allSlots = new List<Slot>();

    private void Awake()
    {
        Instance = this;

        inventorySlots.AddRange(inventorySlotParent.GetComponentsInChildren<Slot>());
        hotBarSlots.AddRange(hotBarObj.GetComponentsInChildren<Slot>());

        allSlots.AddRange(inventorySlots);
        allSlots.AddRange(hotBarSlots);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            container.SetActive(!container.activeInHierarchy);
            Cursor.lockState = Cursor.lockState == CursorLockMode.Locked ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = !Cursor.visible;
        }

        StartDrag();
        EndDrag();
        UpdateDragItemPosition();
        UpdateItemDescription();
    }

    public void AddItem(ItemSO itemToAdd, int amount)
    {
        int remaining = amount;

        foreach (Slot slot in allSlots)
        {
            if (slot.HasItem() && slot.GetItem() == itemToAdd)
            {
                int currentAmount = slot.GetItemAmount();
                int maxStack = itemToAdd.maxStackSize;

                if (currentAmount < maxStack)
                {
                    int spaceLeft = maxStack - currentAmount;
                    int amountToAdd = Math.Min(spaceLeft, remaining);

                    slot.SetItem(itemToAdd, currentAmount + amountToAdd);
                    remaining -= amountToAdd;

                    if (remaining <= 0) return;
                }
            }
        }

        foreach (Slot slot in allSlots)
        {
            if (!slot.HasItem())
            {
                int amountToPlace = Math.Min(itemToAdd.maxStackSize, remaining);
                slot.SetItem(itemToAdd, amountToPlace);

                remaining -= amountToPlace;

                if (remaining <= 0) return;
            }
        }

        if (remaining > 0)
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

    private void UpdateItemDescription()
    {
        Slot hovered = GetHoveredSlot();

        if (hovered != null)
        {
            ItemSO hoveredItem = hovered.GetItem();

            if (hoveredItem != null)
            {
                itemDescriptionParent.SetActive(true);
                itemDescriptionImage.sprite = hoveredItem.icon;
                itemdescriptionTxt.text = hoveredItem.description;
                descriptionIteamNameTxt.text = hoveredItem.itemName;
                return;
            }

        }
        itemDescriptionParent.SetActive(false);
    }
}