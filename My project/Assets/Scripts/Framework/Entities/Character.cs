using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField]
    protected float speed;

    public float maxHealth;
    [SerializeField]
    protected float currentHealth;

    protected virtual void OnEnable()
    {
        currentHealth = maxHealth;
    }

    public virtual void OnDamageTake(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public virtual void Heal(float health)
    {
        currentHealth += health;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
    }

    public virtual void Die()
    {
        currentHealth = 0f;
    }
}
