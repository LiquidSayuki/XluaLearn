using manager;
using System.Collections;
using System.Collections.Generic;
using System.Resources;
using Unity.VisualScripting;
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
    // 设置是否打开Log
    public bool OpenLog;
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

    private static PoolManager _pool;
    public static PoolManager Pool
    {
        get { return _pool; }
    }
    private static NetManager _net;
    public static NetManager Net
    {
        get { return _net; }
    }


    protected override void OnStart()
    {
        AppConst.GameMode = this.GameMode;
        AppConst.OpenLog = this.OpenLog;

        _resource = ResourcesManager.Instance;
        _lua = LuaManager.Instance;
        _ui = UIManager.Instance;
        _entity = EntityManager.Instance;
        _scene = MySceneManager.Instance;
        _sound = SoundManager.Instance;
        _event = EventManager.Instance;
        _pool = PoolManager.Instance;
        _net= NetManager.Instance;

        // Init Lua manager
        //Event.Subscribe(10000, OnLuaInit);
        Event.Subscribe((int)GameEvent.StartLua, StartLua);
        Event.Subscribe((int)GameEvent.GameInit, GameInit);

        if (AppConst.GameMode == GameMode.UpdateMode)
            this.gameObject.AddComponent<HotUpdate>();
        else
            Event.Fire((int)GameEvent.GameInit);
     }

    private void GameInit(object args)
    {
        if(AppConst.GameMode != GameMode.EditorMode)
            Resource.ParseVersionFile();
        Lua.Init();
    }

    private void StartLua(object args)
    {
        Lua.StartLua("LuaMain");

        // 通过大G表来在lua全局寻找函数进行调用
        //XLua.LuaFunction func = Lua.luaEnv.Global.Get<XLua.LuaFunction>("Main");
        //func.Call();

        Pool.CreateGameObjectPool("UI", 10);
        Pool.CreateGameObjectPool("Monster", 120);
        Pool.CreateGameObjectPool("Effect", 120);
        Pool.CreateGameObjectPool("Bullet", 120);
        Pool.CreateAssetPool("AssetBundle", 10);
    }

    private void OnApplicationQuit()
    {
        Event.UnSubscribe((int)GameEvent.StartLua, StartLua);
    }

    public void QuitGane()
    {
        Application.Quit();
    }
}
