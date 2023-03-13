using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class PickableItem: Pickable
{
    [SerializeField]
    private InventoryItemSO item;
    [SerializeField]
    private SpriteRenderer image;

    public void Init(InventoryItemSO item)
    {
        this.item = item;
        image.sprite = item.itemImage;
    }

    // Pick logic
    public override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
    }

    protected override void OnContactWithPlayer()
    {
        BagManager.Instance.AddItem(item);
        GameObject.Destroy(this.gameObject);
    }
}
