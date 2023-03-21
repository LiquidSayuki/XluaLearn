using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace manager
{
    public class PoolManager : MonoSingleton<PoolManager>
    {
        Transform m_PoolParent;

        Dictionary<string, PoolBase> m_Pools = new Dictionary<string, PoolBase>();

        protected override void OnStart()
        {
            m_PoolParent = this.transform.Find("Pool");
        }

        //注册对象池
        private void CreatePool<T>(string poolName, float releaseTime) where T : PoolBase
        {
            if (!m_Pools.TryGetValue(poolName, out PoolBase pool))
            {
                GameObject go = new GameObject(poolName);
                go.transform.SetParent(m_PoolParent);
                pool = go.AddComponent<T>();
                pool.Init(releaseTime);
                m_Pools.Add(poolName, pool);
            }
        }

        public void CreateGameObjectPool(string poolName, float releaseTime)
        {
            CreatePool<GameObjectPool>(poolName, releaseTime);
        }

        public void CreateAssetPool(string poolName, float releaseTime)
        {
            CreatePool<AssetPool>(poolName, releaseTime);
        }

        // 取出对象
        public Object Spawn(string poolName, string assetName)
        {
            if (m_Pools.TryGetValue(poolName, out PoolBase pool))
            {
                return pool.Spawn(assetName);
            }
            return null;
        }

        //回收对象
        public void UnSpawn(string poolName, string assetName, Object asset)
        {
            if (m_Pools.TryGetValue(poolName, out PoolBase pool))
            {
                pool.UnSpawn(assetName, asset);
            }
        }
    }
}