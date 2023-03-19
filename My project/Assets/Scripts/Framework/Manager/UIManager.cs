using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class UIManager : MonoSingleton<UIManager>
{
    class UIElement
    {
        public string Resources;
        public bool Cache;
        public GameObject Instance;
    }

    private Dictionary<Type,UIElement> UIResouces = new Dictionary<Type,UIElement>();

    private Dictionary<string, GameObject> m_UI = new Dictionary<string, GameObject>();

    // UI分层
    private Dictionary<string, Transform> m_UIGroups = new Dictionary<string, Transform>();
    private Transform m_UIParent;

    public void Awake()
    {
        m_UIParent = this.transform.Find("UI");

        this.UIResouces.Add(typeof(BagUI), new UIElement() { Resources = "Bag/ItemBag", Cache = true });
    }

    public void SetUIGroup(List<string> group)
    {
        foreach(var i in group)
        {
            GameObject go = new GameObject("Group-" + i);
            go.transform.SetParent(m_UIParent,false);
            m_UIGroups.Add(i, go.transform);
        }
    }

    Transform GetUIGroup(string group)
    {
        Transform g = null;
        if (!m_UIGroups.TryGetValue(group, out g))
        {
            Debug.LogError("UI group not exist");
        }
        return g;
    }

    #region OpenAndCloseUI

    public bool IsShow<T>()
    {
        Type type = typeof(T);

        if (this.UIResouces.ContainsKey(type))
        {
            UIElement info = this.UIResouces[type];
            if (info.Instance != null)
            {
                if (info.Instance.activeSelf == true)
                {
                    return true;
                }
                return false;
            }
            return false;
        }
        return false;
    }


    public void Show(string uiName, string luaName, string group)
    {
        GameObject go = null;
        if (m_UI.TryGetValue(uiName, out go))
        {
            LuaUILogic uiLogic = go.GetComponent<LuaUILogic>();
            uiLogic.OnOpen();
            return;
        }

        GameManager.Resource.LoadUI(uiName, (Action<UnityEngine.Object>)((UnityEngine.Object obj) =>
        {
            go = GameObject.Instantiate(obj) as GameObject;
            m_UI.Add(uiName, go);

            Transform parent = GetUIGroup(group);
            go.transform.SetParent(parent, false);

            // 将UI组件与Lua脚本绑定
            LuaUILogic uiLogic = go.AddComponent<LuaUILogic>();
            uiLogic.Init(luaName);
            uiLogic.OnOpen();
        }));
    }
    public T Show<T>()
    {
        Type type = typeof(T);

        if (this.UIResouces.ContainsKey(type))
        {
            UIElement info = this.UIResouces[type];
            if (info.Instance != null)
            {
                info.Instance.SetActive(true);
            }
            else
            {
                UnityEngine.Object prefab = Resources.Load(info.Resources);
                if (prefab == null) 
                { 
                    return default(T);
                }
                info.Instance = (GameObject)GameObject.Instantiate(prefab);
            }
            return info.Instance.GetComponent<T>();
        }
        return default(T);
    }

    public void Close(Type type)
    {
        if (this.UIResouces.ContainsKey(type))
        {
            UIElement element = this.UIResouces[type];
            if (element.Cache)
            {
                element.Instance.SetActive(false);
            }
            else
            {
                GameObject.Destroy(element.Instance);
                element.Instance = null;
            }
        }
    }

    public void Close<T>()
    {
        Type type = typeof(T);
        if (this.UIResouces.ContainsKey(type))
        {
            UIElement element = this.UIResouces[type];
            if (element.Cache)
            {
                element.Instance.SetActive(false);
            }
            else
            {
                GameObject.Destroy(element.Instance);
                element.Instance = null;
            }
        }
    }
    #endregion

    #region MainUI
    public void InitUI(float playerMaxHealth)
    {
        MainUI.Instance.InitHealth(playerMaxHealth);
    }
    public void UpdateHealth(float health)
    {
        MainUI.Instance.UpdateHealth(health);
    }
    #endregion
}
