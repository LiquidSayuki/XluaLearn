using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class MainUI : MonoSingleton<MainUI>
{
    int health;
    public TextMeshProUGUI currentHealth;
    public TextMeshProUGUI maxHealth;

    public void InitHealth(float health)
    {
        this.health = (int)health;
        maxHealth.text = health.ToString();
        currentHealth.text = health.ToString();
    }
    public void UpdateHealth(float health)
    {
        this.health = (int)health;
        currentHealth.text = this.health.ToString();
    }


    private void Update()
    {
        // Inventory / Bag
        if (Input.GetKeyDown(KeyCode.I))
        {
            OnBagIconClicked();
        }
    }


    public void OnBagIconClicked()
    {
        if (GameManager.UI.IsShow<BagUI>())
        {
            GameManager.UI.Close<BagUI>();
        }
        else
        {
            GameManager.UI.Show<BagUI>();
        }
    }
}
