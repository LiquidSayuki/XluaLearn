using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolBase : MonoBehaviour
{
    //�Զ��ͷŵ�ʱ�� ��
    protected float m_ReleaseTime;

    //�ϴ��ͷ���Դ���¼� /��΢�� 1s = 10000000��΢��
    protected long m_LastReleaseTime = 0;

    [SerializeField]
    protected List<PoolObject> m_Objects;

    /// <summary>
    /// ��ʼ����
    /// </summary>
    /// <param name="time"> �Զ��ͷ������ʱ��</param>
    public void Init(float time)
    {
        m_ReleaseTime = time;
        m_Objects= new List<PoolObject>();
    }
    //ȡ������
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

    //���ն���
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
