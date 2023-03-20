using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using Util;
using UObject = UnityEngine.Object;

namespace manager
{
    /// <summary>
    /// 一个加载AB包资源的管理器
    /// </summary>
    public class ResourcesManager: MonoSingleton<ResourcesManager>
    {
        internal class BundleInfo
        {
            public string AssetsName;
            public string BundleName;
            public List<string> Dependences;
        }

        internal class BundleData
        {
            public AssetBundle Bundle;

            //引用计数
            public int Count;

            public BundleData (AssetBundle ab)
            {
                Bundle = ab;
                Count = 1;
            }
        }


        private Dictionary<string, BundleInfo> m_BundleInfos = new Dictionary<string, BundleInfo>();
        // 存放已经读取好的Bundle
        // 防止重复读取AssetBundle 报错
        private Dictionary<string, BundleData> m_AssetBundles = new Dictionary<string, BundleData>();

        /// <summary>
        /// 解析版本文件
        /// </summary>
        public void ParseVersionFile()
        {
            // 读取版本文件的地址
            string url = Path.Combine(PathUtil.BundleResourcesPath, AppConst.FileListName);
            string[] data = File.ReadAllLines(url);

            //将版本文件的每一行都读取成一个资源包
            for (int i = 0; i < data.Length; i++)
            {
                BundleInfo bundleInfo = new BundleInfo();
                // 制作版本文件时就使用了“|”分割
                // 结构： AssetName|BundleName|dependences|dependences
                string[] info = data[i].Split("|");

                bundleInfo.AssetsName = info[0];
                bundleInfo.BundleName = info[1];
                bundleInfo.Dependences = new List<string>(info.Length - 2);
                for (int j = 2; j < info.Length; j++)
                {
                    bundleInfo.Dependences.Add(info[j]);
                }
                m_BundleInfos.Add(bundleInfo.AssetsName, bundleInfo);

                //查找Luascripte字符串存在
                if (info[0].IndexOf("LuaScripts") > 0)
                {
                    //证明这是一个Lua文件，将其添加进lua管理器中
                    GameManager.Lua.LuaNames.Add(info[0]);
                }
            }
        }

        /// <summary>
        ///  异步加载资源
        /// </summary>
        /// <param name="assetName">资源名字</param>
        /// <param name="action">完成时的回调</param>
        /// <returns></returns>
        IEnumerator LoadBundleAsync(string assetName, Action<UObject> action = null)
        {
            if (m_BundleInfos.ContainsKey(assetName))
            {
                string bundleName = m_BundleInfos[assetName].BundleName;
                string bundlePath = Path.Combine(PathUtil.BundleResourcesPath, bundleName);
                List<string> dependences = m_BundleInfos[assetName].Dependences;

                BundleData bundle = GetBundle(bundleName);
                if (bundle == null)
                {
                    // 查询对象池
                    UObject obj = GameManager.Pool.Spawn("AssetBundle", bundleName);
                    if (obj != null)
                    {
                        AssetBundle ab = obj as AssetBundle;
                        bundle = new BundleData(ab);
                    }
                    else
                    {
                        AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(bundlePath);
                        yield return request;
                        bundle = new BundleData(request.assetBundle);
                    }
                    m_AssetBundles.Add(bundleName, bundle);
                }

                if (dependences != null && dependences.Count > 0)
                {
                    for (int i = 0; i < dependences.Count; i++)
                    {
                        // 递归加载依赖资源
                        // 为依赖资源计数引用
                        yield return LoadBundleAsync(dependences[i]);
                    }
                }

                //场景资源使用异步加载资源的方式进行，会报错
                //但是场景本身加载是在MysceneManager中完成的，并不需要此处加载回调的内容
                //所以可以在场景的依赖资源都加载完成后，直接回调一个空值来加载场景
                //同时需要退出，以避免在这里加载这个场景资源
                if (assetName.EndsWith(".unity"))
                {
                    action?.Invoke(null);
                    yield break;
                }

                // action为空，说明本次加载的是一个依赖资源
                // 则不需要真实的加载这个资源，可以直接退出
                if(action == null)
                {
                    yield break;
                }

                AssetBundleRequest bundleRequest = bundle.Bundle.LoadAssetAsync(assetName);
                yield return bundleRequest;
                Debug.Log("Async Load Asstes");

                if (action != null && bundleRequest != null)
                {
                    action.Invoke(bundleRequest.asset);
                }
            }
        }



        /// <summary>
        /// 外部加载资源接口
        /// </summary>
        /// <param name="assetName">资源名</param>
        /// <param name="action">加载完成后触发的回调函数</param>
        public void LoadMusic(string assetName, Action<UObject> action = null)
        {
            LoadAsset(PathUtil.GetMusicPath(assetName), action);
        }

        public void LoadSound(string assetName, Action<UObject> action = null)
        {
            LoadAsset(PathUtil.GetSoundPath(assetName), action);
        }

        public void LoadUI(string assetName, Action<UObject> action = null)
        {
            LoadAsset(PathUtil.GetUIPath(assetName), action);
        }

        public void LoadEffect(string assetName, Action<UObject> action = null)
        {
            LoadAsset(PathUtil.GetEffectPath(assetName), action);
        }

        public void LoadScene(string assetName, Action<UObject> action = null)
        {
            LoadAsset(PathUtil.GetScenePath(assetName), action);
        }

        public void LoadLua(string assetName, Action<UObject> action = null)
        {
            LoadAsset(assetName, action);
        }

        public void LoadPrefab(string path, Action<UObject> action = null)
        {
            LoadAsset(path, action);
        }

        private void LoadAsset(string assetName, Action<UObject> action)
        {
#if UNITY_EDITOR
            if (AppConst.GameMode == GameMode.EditorMode)
            {
                EditorLoadAsset(assetName, action);
            }
            else
#endif
            {
                StartCoroutine(LoadBundleAsync(assetName, action));
            }
        }

        BundleData GetBundle(string bundleName)
        {
            BundleData bundle = null;
            if (m_AssetBundles.TryGetValue(bundleName, out bundle))
            {
                bundle.Count++;
                return bundle;
            }
            return null;
        }

        /// <summary>
        /// 减去bundle与其依赖资源的引用计数
        /// </summary>
        /// <param name="assetName"></param>
        public void MinusBundleCount(string assetName)
        {
            string bundleName = m_BundleInfos[assetName].BundleName;

            //本身减少引用
            MinusOneBundleCount(bundleName);
            // 依赖减少引用
            List<string> dependencies = m_BundleInfos[assetName].Dependences;
            if(dependencies != null)
            {
                foreach (string dependency in dependencies) 
                {
                    if (m_BundleInfos.ContainsKey(dependency))
                    {
                        string name = m_BundleInfos[dependency].BundleName;
                        MinusOneBundleCount(name);
                    }
                }
            }
        }

        private void MinusOneBundleCount(string bundleName)
        {
            if(m_AssetBundles.TryGetValue(bundleName, out BundleData bundle))
            {
                if (bundle.Count > 0)
                {
                    bundle.Count--;
                    Debug.Log("bundle:" + bundleName + "Count:" + bundle.Count);
                }
                if(bundle.Count <= 0)
                {
                    Debug.Log("bundle:" + bundleName + " be stored in pool");
                    GameManager.Pool.UnSpawn("AssetBundle", bundleName, bundle.Bundle);
                    m_AssetBundles.Remove(bundleName);
                }
            }
        }

#if UNITY_EDITOR
        /// <summary>
        ///  编辑器环境加载资源
        /// </summary>
        /// <param name="assetName">资源名称</param>
        /// <param name="action">传入回调函数</param>
        void EditorLoadAsset(string assetName, Action<UObject> action = null)
        {
            Debug.Log("Editor Load Asstes");
            UObject obj = UnityEditor.AssetDatabase.LoadAssetAtPath(assetName, typeof(UObject));
            if (obj == null)
            {
                Debug.LogError("assets name is not exist:" + assetName);
            }
            action?.Invoke(obj);
        }
#endif

        public void UnloadBundle(UObject obj)
        {
            AssetBundle ab = obj as AssetBundle;
            ab.Unload(true);
        }
    }
}

