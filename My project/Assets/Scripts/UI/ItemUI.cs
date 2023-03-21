using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemUI : MonoBehaviour
{
    public TMPro.TextMeshProUGUI count;
    public Image icon;

    private string itemDescription;
    private string itemName;

    public void SetIcon(string icon,string count, string itemName, string itemDescription)
    {
        this.icon.overrideSprite = Resources.Load<Sprite>(icon);
        this.count.text = count;
        this.itemName = itemName;
        this.itemDescription = itemDescription;
    }

    public void SetIcon(Sprite icon, string count, string itemName, string itemDescription)
    {
        this.icon.overrideSprite = icon;
        this.count.text = count;
        this.itemName = itemName;
        this.itemDescription = itemDescription;
    }
}
