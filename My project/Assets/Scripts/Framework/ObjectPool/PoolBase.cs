using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolBase : MonoBehaviour
{
    //自动释放的时间 秒
    protected float m_ReleaseTime;

    //上次释放资源的事件 /豪微妙 1s = 10000000毫微秒
    protected long m_LastReleaseTime = 0;

    [SerializeField]
    protected List<PoolObject> m_Objects;

    /// <summary>
    /// 初始化池
    /// </summary>
    /// <param name="time"> 自动释放物体的时间</param>
    public void Init(float time)
    {
        m_ReleaseTime = time;
        m_Objects= new List<PoolObject>();
    }
    //取出对象
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

    //回收对象
    public virtual void UnSpawn(string name, Object obj)
    {
        PoolObject po = new PoolObject(name, obj);
        m_Objects.Add(po);
    }

    public virtual void Release()
    {

    }

    private void Update()
    {
        if (System.DateTime.Now.Ticks - m_LastReleaseTime >= m_ReleaseTime * 10000000)
        {
            m_LastReleaseTime=System.DateTime.Now.Ticks;
            Release();
        }
    }
}
