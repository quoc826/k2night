using UnityEngine;


[CreateAssetMenu(fileName = "Item", menuName = "InventoryItem")]
public class ItemSO : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public int maxStackSize;
    public GameObject itemPrefab;
    public GameObject handItemPrefab;
}
