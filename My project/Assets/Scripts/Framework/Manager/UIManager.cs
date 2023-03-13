using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    class UIElement
    {
        public string Resources;
        public bool Cache;
        public GameObject Instance;
    }

    private Dictionary<Type,UIElement> UIResouces = new Dictionary<Type,UIElement>();

    public UIManager()
    {
        this.UIResouces.Add(typeof(BagUI), new UIElement() { Resources = "Bag/ItemBag",Cache = true});
    }
    ~UIManager(){ }

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
