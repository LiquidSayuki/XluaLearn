using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BagUI : MonoBehaviour
{
    public GameObject ItemPrefab;
    public Transform Content;
    private List<GameObject> slots = new List<GameObject>();

    public System.Type Type { get { return this.GetType(); } }

    private List<GameObject> cache;


    void Start()
    {
        cache= new List<GameObject>();

        // 当一个物品栏格子靠多个image组成时，get components in children会把孙物体get到，非常恶心
        foreach (Transform child in Content.transform)
        {
            slots.Add(child.gameObject);
        }
        for (int i = 0; i < slots.Count; i++)
        {
            slots[i].GetComponent<Slot>().slotId = i;
        }
        Refresh();
        BagManager.Instance.BagChanged = Refresh;
    }

    private void Refresh()
    {
        foreach(var i in cache)
        {
            if (i != null)
            {
                Destroy(i);
            }
        }
        InventorySO bag = BagManager.Instance.GetPlayerBag();
        for (int i = 0; i < bag.itemList.Count; i++)
        {
            CreateItem(bag.itemList[i], slots[i]);
        }
    }

    private void CreateItem(InventoryItemSO item, GameObject parent)
    {
        GameObject go = Instantiate(ItemPrefab, parent.transform);

        if (item != null)
        {
            ItemUI ui = go.GetComponent<ItemUI>();
            ui.SetIcon(item.itemImage, item.itemCount.ToString(), item.itemName, item.itemInfo);
        }
        else
        {
            go.SetActive(false);
        }
        this.cache.Add(go);
    }

    public void Close()
    {
        UIManager.Instance.Close(this.Type);
    }
}
