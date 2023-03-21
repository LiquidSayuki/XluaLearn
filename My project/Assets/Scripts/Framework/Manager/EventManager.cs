using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace manager
{
    public class EventManager : MonoSingleton<EventManager>
    {
        //为lua传递参数使用的事件
        public delegate void EventHandler(object args);

        Dictionary<int, EventHandler> m_Events = new Dictionary<int, EventHandler>();

        public void Subscribe(int id, EventHandler e)
        {
            if (m_Events.ContainsKey(id))
            {
                m_Events[id] += e;
            }
            else
            {
                m_Events.Add(id, e);
            }
        }

        public void UnSubscribe(int id, EventHandler e)
        {
            if (m_Events.ContainsKey(id))
            {
                if (m_Events[id] != null)
                {
                    m_Events[id] -= e;
                }
                if (m_Events[id] == null)
                {
                    m_Events.Remove(id);
                }
            }
        }

        public void Fire(int id, object args = null)
        {
            EventHandler handler;
            if (m_Events.TryGetValue(id, out handler))
            {
                handler(args);
            }
        }
    }
}