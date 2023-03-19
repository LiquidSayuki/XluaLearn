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

        private Dictionary<string, BundleInfo> m_BundleInfos = new Dictionary<string, BundleInfo>();
        // ����Ѿ���ȡ�õ�Bundle
        // ��ֹ�ظ���ȡAssetBundle ����
        private Dictionary<string, AssetBundle> m_AssetBundles = new Dictionary<string, AssetBundle>();
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

                AssetBundle bundle = GetBundle(bundleName);
                if (bundle == null)
                {
                    if (dependences != null && dependences.Count > 0)
                    {
                        for (int i = 0; i < dependences.Count; i++)
                        {
                            // �ݹ����������Դ
                            yield return LoadBundleAsync(dependences[i]);
                        }
                    }
                    AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(bundlePath);
                    yield return request;
                    bundle = request.assetBundle;
                    m_AssetBundles.Add(bundleName, bundle);
                }
                //������Դʹ���첽������Դ�ķ�ʽ���У��ᱨ��
                //���ǳ��������������MysceneManager����ɵģ�������Ҫ�˴����ػص�������
                //���Կ����ڳ�����������Դ��������ɺ�ֱ�ӻص�һ����ֵ
                if (assetName.EndsWith(".unity"))
                {
                    action?.Invoke(null);
                    yield break;
                }

                AssetBundleRequest bundleRequest = bundle.LoadAssetAsync(assetName);
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
            if(AppConst.GameMode == GameMode.EditorMode)
            {
#if UNITY_EDITOR
                EditorLoadAsset(assetName, action);
#endif
            }
            else
            {
                StartCoroutine(LoadBundleAsync(assetName, action));
            }
        }

        AssetBundle GetBundle(string bundleName)
        {
            AssetBundle bundle = null;
            if (m_AssetBundles.TryGetValue(bundleName, out bundle))
            {
                return bundle;
            }
            return null;
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
    }
}

