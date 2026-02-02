using UnityEngine;


[CreateAssetMenu(fileName = "Item", menuName = "InventoryItem")]
public class ItemSO : ScriptableObject
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public string itemName;
    public Sprite icon;
    public int maxStackSize;
    public GameObject itemPrefab;
    public GameObject handItemPrefab;
}
