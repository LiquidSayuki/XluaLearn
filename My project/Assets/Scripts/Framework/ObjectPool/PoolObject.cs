using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolObject
{
    public Object Object;

    public string Name;

    // 使用时间戳来当LRU的回收机制
    public System.DateTime LastUseTime;

    public PoolObject(string name, Object @object)
    {
        Object = @object;
        Name = name;
        LastUseTime = System.DateTime.Now;
    }
}
