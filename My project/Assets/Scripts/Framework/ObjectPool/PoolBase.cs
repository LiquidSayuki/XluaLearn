using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolBase : MonoBehaviour
{
    //自动释放的时间
    protected float m_ReleaseTime;

    //上次释放资源的事件 /豪微妙 1s = 10000000毫微秒
    protected long m_LastReleaseTime = 0;

    protected List<PoolObject> m_Objects;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="time"> 自动释放物体的时间</param>
    public void Init(float time)
    {
        m_ReleaseTime = time;
        m_Objects= new List<PoolObject>();
    }

    public virtual Object Spawn(string name)
    {
        foreach (PoolObject po in m_Objects)
        {
            if(po.Name == name)
            {
                m_Objects.Remove(po);
                return po.Object;
            }
        }
        return null;
    }
}
