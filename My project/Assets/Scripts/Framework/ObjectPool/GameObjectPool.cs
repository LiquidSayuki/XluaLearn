using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectPool : PoolBase
{

    //取出对象
    public override Object Spawn(string name)
    {
        Object obj = base.Spawn(name);
        if(obj == null)
        {
            return null;
        }

        GameObject go = obj as GameObject;
        go.SetActive(true);
        return obj;
    }

    public override void UnSpawn(string name, Object obj)
    {
        GameObject go = obj as GameObject;
        go.SetActive(false);
        go.transform.SetParent(this.transform, false);
        base.UnSpawn(name, obj);
    }

    public override void Release()
    {
        base.Release();
        foreach(PoolObject item in m_Objects)
        {
            if(System.DateTime.Now.Ticks - item.LastUseTime.Ticks >= m_ReleaseTime * 10000000)
            {
                Debug.Log("GameObject release at:" + System.DateTime.Now);
                Destroy(item.Object);
                GameManager.Resource.MinusBundleCount(item.Name);
                m_Objects.Remove(item);
                Release();
                return;
            }
        }
    }
}
