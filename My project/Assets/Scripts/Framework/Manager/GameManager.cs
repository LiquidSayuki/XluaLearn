using manager;
using System.Collections;
using System.Collections.Generic;
using System.Resources;
using UnityEngine;
using Util;

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
            MySceneManager.Instance.LoadScene("TestDungeon");
        }
        else
        {
            MySceneManager.Instance.LoadScene("BossRush");
        }
    }

    public void RegisterPlayer(Player player)
    {
        this.player = player;
        UIManager.Instance.InitUI(player.maxHealth);

        this.playerInventory.itemList = new List<InventoryItemSO>(new InventoryItemSO[15]);
        BagManager.Instance.Init(playerInventory);
    }

    // 设置游戏模式（管理ab包加载）
    public GameMode GameMode;
    // 其他管理器实例统一管理
    private static ResourcesManager _resource;
    public static ResourcesManager Resource
    {
        get { return _resource; }
    }

    private static LuaManager _lua;
    public static LuaManager Lua
    {
        get { return _lua; }
    }

    private static UIManager _ui;
    public static UIManager UI
    {
        get { return _ui; }
    }

    private static EntityManager _entity;
    public static EntityManager Entity
    {
        get { return _entity; }
    }
    private static MySceneManager _scene;
    public static MySceneManager Scene
    {
        get { return _scene; }
    }

    private static SoundManager _sound;
    public static SoundManager Sound
    {
        get { return _sound; }
    }
    private static EventManager _event;
    public static EventManager Event
    {
        get { return _event; }
    }

    protected override void OnStart()
    {
        AppConst.GameMode = this.GameMode;
        _resource = ResourcesManager.Instance;
        _lua = LuaManager.Instance;
        _ui = UIManager.Instance;
        _entity = EntityManager.Instance;
        _scene = MySceneManager.Instance;
        _sound = SoundManager.Instance;
        _event = EventManager.Instance;

        // Init Resources manager
        Resource.ParseVersionFile();
        // Init Lua manager
        Event.Subscribe(10000, OnLuaInit);
        Lua.Init();
        

        // 通过大G表来在lua全局寻找函数进行调用，效率太低了
        //XLua.LuaFunction func = Lua.luaEnv.Global.Get<XLua.LuaFunction>("Main");
        //func.Call();
     }

    void OnLuaInit(object args)
    {
        Lua.StartLua("LuaMain");
    }

    private void OnApplicationQuit()
    {
        Event.UnSubscribe(10000, OnLuaInit);
    }
}
