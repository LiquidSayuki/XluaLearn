using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace manager
{
    public class EntityManager : MonoSingleton<EntityManager>
    {
        private Dictionary<string, GameObject> m_Entities = new Dictionary<string, GameObject>();

        private Dictionary<string, Transform> m_EntityGroups = new Dictionary<string, Transform>();
        private Transform m_EntityParent;

        private void Awake()
        {
            m_EntityParent = this.transform.Find("Entity");
        }

        public void SetEntityGroup(List<string> group)
        {
            foreach (var i in group)
            {
                GameObject go = new GameObject("Group-" + i);
                go.transform.SetParent(m_EntityParent, false);
                m_EntityGroups.Add(i, go.transform);
            }
        }

        Transform GetEntityGroup(string group)
        {
            Transform g = null;
            if (!m_EntityGroups.TryGetValue(group, out g))
            {
                Debug.LogError("Entity group not exist");
            }
            return g;
        }

        public void Show(string entityName, string luaName, string group)
        {
            GameObject go = null;
            if (m_Entities.TryGetValue(entityName, out go))
            {
                LuaEntityLogic entityLogic = go.GetComponent<LuaEntityLogic>();
                entityLogic.OnShow();
                return;
            }

            GameManager.Resource.LoadPrefab(entityName, (UnityEngine.Object obj) =>
            {
                go = GameObject.Instantiate(obj) as GameObject;
                m_Entities.Add(entityName, go);

                Transform parent = GetEntityGroup(group);
                go.transform.SetParent(parent, false);

                // 将UI组件与Lua脚本绑定
                LuaEntityLogic entityLogic = go.AddComponent<LuaEntityLogic>();
                entityLogic.Init(luaName);
                entityLogic.OnShow();
            });
        }

    }
}

