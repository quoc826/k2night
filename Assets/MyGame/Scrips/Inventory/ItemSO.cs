using UnityEngine;


[CreateAssetMenu(fileName = "Item", menuName = "InventoryItem")]
public class ItemSO : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public int maxStackSize;
    public string description;
    public GameObject itemPrefab;
    // public GameObject handItemPrefab;


    [Header("status_item")]
    public int attackDamge;
    public int armor;
    public int health;


}
