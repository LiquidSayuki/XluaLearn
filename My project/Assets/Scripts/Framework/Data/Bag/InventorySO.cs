using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Inventory", menuName = "PCG/Inventory")]
public class InventorySO : ScriptableObject
{
    public List<InventoryItemSO> itemList;
}
