using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolBase : MonoBehaviour
{
    //�Զ��ͷŵ�ʱ��
    protected float m_ReleaseTime;

    //�ϴ��ͷ���Դ���¼� /��΢�� 1s = 10000000��΢��
    protected long m_LastReleaseTime = 0;

    protected List<PoolObject> m_Objects;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="time"> �Զ��ͷ������ʱ��</param>
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
