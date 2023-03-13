using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    public GameObject[] playerPrefab;
    public Player player;
    public InventorySO playerInventory;

    private int _selectedPlayer = 0;
    public Player Player 
    { 
        get 
        { 
            if (player != null)
            {
                return player;
            }
            GameObject go = Instantiate(playerPrefab[_selectedPlayer], new Vector2(0, 0), Quaternion.identity);
            this.player = go.GetComponent<Player>();
            return this.player;
        } 
    }

    public void LoadMainGameScene(int gameMode)
    {
        _selectedPlayer = gameMode;
        if (gameMode == 0)
        {
            SceneManager.Instance.LoadScene("TestDungeon");
        }
        else
        {
            SceneManager.Instance.LoadScene("BossRush");
        }
    }

    public void RegisterPlayer(Player player)
    {
        this.player = player;
        UIManager.Instance.InitUI(player.maxHealth);

        this.playerInventory.itemList = new List<InventoryItemSO>(new InventoryItemSO[15]);
        BagManager.Instance.Init(playerInventory);
    }
}
