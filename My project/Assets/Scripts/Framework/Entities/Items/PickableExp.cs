using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickableExp : Pickable
{
    private int expValue = 100;
    [SerializeField]
    private Sprite[] expImages;
    public void Init(int exp)
    {
        this.expValue = exp;
        if (exp < 500)
        {
            this.gameObject.GetComponent<SpriteRenderer>().sprite = expImages[0];
        }else if (exp < 1000)
        {
            this.gameObject.GetComponent<SpriteRenderer>().sprite = expImages[1];
        }else if(exp >1000)
        {
            this.gameObject.GetComponent<SpriteRenderer>().sprite = expImages[2];
        }
    }

    public override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
    }

    protected override void OnContactWithPlayer()
    {
        GameObject.Destroy(this.gameObject);
    }
}
