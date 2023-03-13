using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "Item", menuName = "PCG/Item")]
public class InventoryItemSO : ScriptableObject
{
    public string itemName;
    public Sprite itemImage;
    public int itemCount;
    [TextArea]
    public string itemInfo;
}
