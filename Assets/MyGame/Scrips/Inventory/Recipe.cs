using UnityEngine;
using System.Collections.Generic;
using System.IO;

[System.Serializable]

public class Ingredient
{
    public ItemSO item;
    public int amount;
}


[CreateAssetMenu(fileName = "New Recipe", menuName = "Inventory/Recipe")]
public class Recipe : ScriptableObject
{
    public List<Ingredient> ingredients;
    public ItemSO result;
    public int resultAmount = 1;

}
