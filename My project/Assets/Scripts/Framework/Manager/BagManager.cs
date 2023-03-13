using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BagManager : Singleton<BagManager>
{
    public Action BagChanged;
    private InventorySO playerBag;

    public void Init(InventorySO inventory)
    {
        this.playerBag = inventory;
    }

    public void AddItem(InventoryItemSO item)
    {
        if (!this.playerBag.itemList.Contains(item))
        {
            for (int i = 0; i < this.playerBag.itemList.Count; i++)
            {
                if (this.playerBag.itemList[i] == null)
                {
                    this.playerBag.itemList[i] = item;
                    this.playerBag.itemList[i].itemCount = 1;
                    break;
                }
            }
        }
        else
        {
            item.itemCount++;
        }

        // Refresh Bag UI
        if (this.BagChanged != null)
        {
            this.BagChanged();
        }
    }

    public void AddItem(InventoryItemSO item, int count)
    {
        if (!this.playerBag.itemList.Contains(item))
        {
            playerBag.itemList.Add(item);
            item.itemCount = count;
        }
        else
        {
            item.itemCount += count;
        }
        if (this.BagChanged != null)
        {
            BagChanged();
        }
    }

    public InventorySO GetPlayerBag()
    {
        return this.playerBag;
    }


    public void SwapItem(int itemId, int targetId)
    {
        InventoryItemSO temp = this.playerBag.itemList[itemId];
        this.playerBag.itemList[itemId] = this.playerBag.itemList[targetId];
        this.playerBag.itemList[targetId] = temp;

        if (this.BagChanged != null)
        {
            this.BagChanged();
        }
    }
}
