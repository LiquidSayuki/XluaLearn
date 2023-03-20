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
    /// һ������AB����Դ�Ĺ�����
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

            //���ü���
            public int Count;

            public BundleData (AssetBundle ab)
            {
                Bundle = ab;
                Count = 1;
            }
        }


        private Dictionary<string, BundleInfo> m_BundleInfos = new Dictionary<string, BundleInfo>();
        // ����Ѿ���ȡ�õ�Bundle
        // ��ֹ�ظ���ȡAssetBundle ����
        private Dictionary<string, BundleData> m_AssetBundles = new Dictionary<string, BundleData>();

        /// <summary>
        /// �����汾�ļ�
        /// </summary>
        public void ParseVersionFile()
        {
            // ��ȡ�汾�ļ��ĵ�ַ
            string url = Path.Combine(PathUtil.BundleResourcesPath, AppConst.FileListName);
            string[] data = File.ReadAllLines(url);

            //���汾�ļ���ÿһ�ж���ȡ��һ����Դ��
            for (int i = 0; i < data.Length; i++)
            {
                BundleInfo bundleInfo = new BundleInfo();
                // �����汾�ļ�ʱ��ʹ���ˡ�|���ָ�
                // �ṹ�� AssetName|BundleName|dependences|dependences
                string[] info = data[i].Split("|");

                bundleInfo.AssetsName = info[0];
                bundleInfo.BundleName = info[1];
                bundleInfo.Dependences = new List<string>(info.Length - 2);
                for (int j = 2; j < info.Length; j++)
                {
                    bundleInfo.Dependences.Add(info[j]);
                }
                m_BundleInfos.Add(bundleInfo.AssetsName, bundleInfo);

                //����Luascripte�ַ�������
                if (info[0].IndexOf("LuaScripts") > 0)
                {
                    //֤������һ��Lua�ļ���������ӽ�lua��������
                    GameManager.Lua.LuaNames.Add(info[0]);
                }
            }
        }

        /// <summary>
        ///  �첽������Դ
        /// </summary>
        /// <param name="assetName">��Դ����</param>
        /// <param name="action">���ʱ�Ļص�</param>
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
                    // ��ѯ�����
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
                        // �ݹ����������Դ
                        // Ϊ������Դ��������
                        yield return LoadBundleAsync(dependences[i]);
                    }
                }

                //������Դʹ���첽������Դ�ķ�ʽ���У��ᱨ��
                //���ǳ��������������MysceneManager����ɵģ�������Ҫ�˴����ػص�������
                //���Կ����ڳ�����������Դ��������ɺ�ֱ�ӻص�һ����ֵ�����س���
                //ͬʱ��Ҫ�˳����Ա���������������������Դ
                if (assetName.EndsWith(".unity"))
                {
                    action?.Invoke(null);
                    yield break;
                }

                // actionΪ�գ�˵�����μ��ص���һ��������Դ
                // ����Ҫ��ʵ�ļ��������Դ������ֱ���˳�
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
        /// �ⲿ������Դ�ӿ�
        /// </summary>
        /// <param name="assetName">��Դ��</param>
        /// <param name="action">������ɺ󴥷��Ļص�����</param>
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
        /// ��ȥbundle����������Դ�����ü���
        /// </summary>
        /// <param name="assetName"></param>
        public void MinusBundleCount(string assetName)
        {
            string bundleName = m_BundleInfos[assetName].BundleName;

            //�����������
            MinusOneBundleCount(bundleName);
            // ������������
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
        ///  �༭������������Դ
        /// </summary>
        /// <param name="assetName">��Դ����</param>
        /// <param name="action">����ص�����</param>
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

