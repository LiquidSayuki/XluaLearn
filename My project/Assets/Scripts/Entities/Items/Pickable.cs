using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickable : MonoBehaviour
{

    public virtual void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Trigger Enter");
        if (collision.CompareTag("Player"))
        {
            OnContactWithPlayer();
        }
    }
    protected virtual void OnContactWithPlayer() {}
}
