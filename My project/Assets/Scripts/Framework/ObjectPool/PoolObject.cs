using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolObject
{
    public Object Object;

    public string Name;

    // ʹ��ʱ�������LRU�Ļ��ջ���
    public System.DateTime LastUseTime;

    public PoolObject(Object @object, string name, System.DateTime lastUseTime)
    {
        Object = @object;
        Name = name;
        LastUseTime = lastUseTime;
    }
}
