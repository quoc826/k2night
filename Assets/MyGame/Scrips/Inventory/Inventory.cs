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

    [Header("--- Crafing Settings ---")]
    public List<Recipe> allRecipes = new List<Recipe>();
    public Transform craftingGrid;
    public GameObject craftingBTNprefab;



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

        PopulateCraftingGrid();

        LoadGame();

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

    // load game
    public void LoadGame()
    {
        InventoryData data = SaveSystem.LoadInventory();
        if (data == null) { Debug.Log("Không tìm thấy file save!"); return; }

        for (int i = 0; i < allSlots.Count; i++)
        {
            if (i < data.itemNames.Count && !string.IsNullOrEmpty(data.itemNames[i]))
            {
                ItemSO item = Resources.Load<ItemSO>("Items/" + data.itemNames[i]);

                if (item != null)
                {
                    allSlots[i].SetItem(item, data.itemAmounts[i]);
                    Debug.Log("Đã nạp thành công: " + data.itemNames[i]);
                }
                else
                {
                    Debug.LogError("LỖI: Không tìm thấy file " + data.itemNames[i] + " trong Resources/Items/");
                }
            }
        }
    }

    public void SaveGame()
    {
        SaveSystem.SaveInventory(this, allSlots);
    }

    private void OnApplicationQuit()
    {
        SaveGame();
        Debug.Log("Game đang thoát... Đã tự động lưu dữ liệu!");
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

                    if (remaining <= 0)
                    {
                        PopulateCraftingGrid();
                        return;
                    }
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

                if (remaining <= 0)
                {
                    PopulateCraftingGrid();
                    return;
                }
            }
        }

        SaveGame();

        if (remaining > 0)
        {
            Debug.Log("Không đủ chỗ trống trong túi đồ!");
        }
        PopulateCraftingGrid();

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
            if (hovered != null && draggedSlot != null)
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
        if (from == null && to == null) return;

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
                from.RemoveAmount(move);

                if (from.GetItemAmount() <= 0)
                    from.ClearSlot();
                PopulateCraftingGrid();
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
            PopulateCraftingGrid();
            return;
        }

        //Empty slot
        to.SetItem(from.GetItem(), from.GetItemAmount());
        from.ClearSlot();

        PopulateCraftingGrid();
        SaveGame();
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


    // crafting methods would go here
    private void PopulateCraftingGrid()
    {
        for (int i = craftingGrid.childCount - 1; i >= 0; i--)
        {
            Destroy(craftingGrid.GetChild(i).gameObject);
        }

        foreach (Recipe recipe in allRecipes)
        {
            GameObject btnObj = Instantiate(craftingBTNprefab, craftingGrid);
            Image img = btnObj.transform.GetChild(0).GetComponent<Image>();
            img.sprite = recipe.result.icon;
            Button btn = btnObj.GetComponent<Button>();

            btn.interactable = CanCraft(recipe);
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => Craft(recipe));
        }
    }

    private void Craft(Recipe recipe)
    {
        if (!CanCraft(recipe))
        {
            return;
        }

        ConsumeIngredients(recipe);
        AddItem(recipe.result, recipe.resultAmount);
        PopulateCraftingGrid();
    }

    private void ConsumeIngredients(Recipe recipe)
    {
        foreach (Ingredient ingredient in recipe.ingredients)
        {
            int remainingToConsume = ingredient.amount;

            foreach (Slot slot in allSlots)
            {
                if (slot.HasItem() && slot.GetItem() == ingredient.item)
                {
                    int currentInSlot = slot.GetItemAmount();
                    int take = Math.Min(currentInSlot, remainingToConsume);

                    // Cập nhật lại số lượng trong slot
                    slot.SetItem(slot.GetItem(), currentInSlot - take);
                    remainingToConsume -= take;

                    // Nếu slot hết sạch thì xóa item
                    if (slot.GetItemAmount() <= 0)
                        slot.ClearSlot();

                    if (remainingToConsume <= 0) break;
                }
            }
        }
    }

    private bool CanCraft(Recipe recipe)
    {
        foreach (Ingredient ingredient in recipe.ingredients)
        {
            int totalFound = 0;
            foreach (Slot slot in allSlots)
            {
                if (slot.HasItem() && slot.GetItem() == ingredient.item)
                {
                    totalFound += slot.GetItemAmount();
                }
            }

            // Sau khi check hết tất cả các slot mà vẫn không đủ thì mới return false
            if (totalFound < ingredient.amount)
            {
                return false;
            }
        }
        return true;
    }
}