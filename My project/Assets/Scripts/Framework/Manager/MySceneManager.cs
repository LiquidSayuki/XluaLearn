using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace manager
{
    public class MySceneManager : MonoSingleton<MySceneManager>
    {
        UnityAction<float> onProgress = null;

        private string m_LogicName = "[SceneLogic]";

        private void Awake()
        {
            // 切换场景时的回调
            SceneManager.activeSceneChanged += OnActiveSceneChanged;
        }

        public void LoadScene(string name)
        {
            StartCoroutine(LoadLevel(name));
        }

        /// <summary>
        /// 叠加的加载场景
        /// </summary>
        /// <param name="sceneName">场景路径</param>
        /// <param name="luaName">lua脚本名</param>
        public void LoadScene(string sceneName, string luaName)
        {
            GameManager.Resource.LoadScene(sceneName, (UnityEngine.Object obj) =>
            {
                StartCoroutine(StartLoadScene(sceneName, luaName, LoadSceneMode.Additive));
            });
        }

        /// <summary>
        ///  切换场景
        /// </summary>
        /// <param name="sceneName">场景路径</param>
        /// <param name="luaName">lua脚本名</param>
        public void ChangeScene(string sceneName, string luaName)
        {
            GameManager.Resource.LoadScene(sceneName, (UnityEngine.Object obj) =>
            {
                StartCoroutine(StartLoadScene(sceneName, luaName, LoadSceneMode.Single));
            });
        }

        /// <summary>
        /// 切换当前激活的场景
        /// </summary>
        /// <param name="sceneName"></param>
        public void SetActive(string sceneName)
        {
            Scene scene = SceneManager.GetSceneByName(sceneName);
            SceneManager.SetActiveScene(scene);
        }


        /// <summary>
        /// 卸载场景
        /// </summary>
        /// <param name="sceneName"></param>
        public void UnloadSceneAsync(string sceneName)
        {
            StartCoroutine(UnloadScene(sceneName));
        }


        IEnumerator StartLoadScene(string sceneName, string luaName, LoadSceneMode mode)
        {
            if (IsLoadedScene(sceneName))
                yield break;

            AsyncOperation async = SceneManager.LoadSceneAsync(sceneName, mode);
            async.allowSceneActivation= true;
            yield return async;

            // LoadSceneAsync 不反回场景本身 需要为场景绑定lua脚本，还需单独获取场景
            Scene scene = SceneManager.GetSceneByName(sceneName);
            GameObject go = new GameObject(m_LogicName);
            SceneManager.MoveGameObjectToScene(go, scene);
            LuaSceneLogic logic = go.AddComponent<LuaSceneLogic>();
            logic.SceneName = sceneName;
            logic.Init(luaName);
            logic.OnEnter();
        }

        private bool IsLoadedScene(string sceneName)
        {
            Scene scene = SceneManager.GetSceneByName(sceneName);
            return scene.isLoaded;
        }

        IEnumerator LoadLevel(string name)
        {
            Debug.LogFormat("LoadScene: {0}", name);
            AsyncOperation async = SceneManager.LoadSceneAsync(name);
            async.allowSceneActivation = true;
            async.completed += LevelLoadCompleted;
            while (!async.isDone)
            {
                if (onProgress != null)
                    onProgress(async.progress);
                yield return null;
            }
        }

        private void LevelLoadCompleted(AsyncOperation obj)
        {
            if (onProgress != null)
                onProgress(1f);
            Debug.Log("SceneLoadCompleted:" + obj.progress);
        }

        IEnumerator UnloadScene(string sceneName)
        {
            Scene scene = SceneManager.GetSceneByName(sceneName);
            if (!scene.isLoaded)
            {
                Debug.LogError("Trying to unload a scene that was never loaded");
                yield break;
            }
            LuaSceneLogic logic = GetSceneLogic(scene);
            logic?.OnQuit();
            AsyncOperation async = SceneManager.UnloadSceneAsync(scene);
            yield return async;
        }

        private LuaSceneLogic GetSceneLogic(Scene scene)
        {
            GameObject[] gos = scene.GetRootGameObjects();
            foreach (GameObject go in gos)
            {
                if (go.name.CompareTo(m_LogicName) == 0)
                {
                    LuaSceneLogic logic = go.GetComponent<LuaSceneLogic>();
                    return logic;
                }
            }
            return null;
        }

        private void OnActiveSceneChanged(Scene s1, Scene s2)
        {
            if (!s1.isLoaded || !s2.isLoaded)
                return;

            LuaSceneLogic logic1 = GetSceneLogic(s1);
            LuaSceneLogic logic2 = GetSceneLogic(s2);

            logic1?.OnDeactive();
            logic2?.OnActive();
        }
    }
}

