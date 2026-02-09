using System.Collections.Generic;
using UnityEngine;
using System.IO;
public class InventoryData
{
    public List<string> itemNames = new List<string>();
    public List<int> itemAmounts = new List<int>();
}


public class SaveSystem : MonoBehaviour
{
    private static string path = Application.persistentDataPath + "inventory.sav";

    public static void SaveInventory(Inventory inv, List<Slot> allSlots)
    {
        InventoryData data = new InventoryData();

        foreach (Slot slot in allSlots)
        {
            if (slot.HasItem())
            {
                data.itemNames.Add(slot.GetItem().itemName);
                data.itemAmounts.Add(slot.GetItemAmount());
            }
            else
            {
                data.itemNames.Add("");
                data.itemAmounts.Add(0);
            }

            string json = JsonUtility.ToJson(data);
            File.WriteAllText(path, json);
            Debug.Log("Đã lưu tại: " + path);
        }

    }

    public static InventoryData LoadInventory()
    {
        if (File.Exists(path)){
            string  json = File.ReadAllText(path);
            return JsonUtility.FromJson<InventoryData>(json);
        }
        return null;
    }


}

