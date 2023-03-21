using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Util;
using XLua;

namespace manager
{
    public class LuaManager : MonoSingleton<LuaManager>
    {
        public List<string> LuaNames = new List<string>();

        //缓存lua脚本内容
        private Dictionary<string, byte[]> m_LuaScripts;

        //Lua虚拟机
        public LuaEnv luaEnv;

        public void Init()
        {
            luaEnv = new LuaEnv();
            luaEnv.AddBuildin("rapidjson", XLua.LuaDLL.Lua.LoadRapidJson);

            luaEnv.AddLoader(Loader);
            m_LuaScripts = new Dictionary<string, byte[]>();

#if UNITY_EDITOR
            if ( AppConst.GameMode == GameMode.EditorMode)
            {
                EditorLoadLuaScript();
            }
            else
#endif
            {
                LoadLuaScript();
            }
        }

        /// <summary>
        ///  封装的外部调用Lua脚本的方法
        /// </summary>
        /// <param name="name">Lua脚本名</param>
        public void StartLua(string name)
        {
            luaEnv.DoString(string.Format("require '{0}'", name));
        }

        // 得到lua脚本内容
        byte[] Loader(ref string name)
        {
            return GetLuaScript(name);
        }

        public byte[] GetLuaScript(string name)
        {
            // requie ui.login.register
            name = name.Replace(".", "/");
            string filename = PathUtil.GetLuaPath(name);

            byte[] luaScript = null;
            if(!m_LuaScripts.TryGetValue(filename, out luaScript))
            {
                Debug.LogError("lua script is not exist:" + filename);
            }
            return luaScript;
        }

        void LoadLuaScript()
        {
            foreach(string name in LuaNames)
            {
                GameManager.Resource.LoadLua(name, (UnityEngine.Object obj) =>
                {
                    AddLuaScript(name, (obj as TextAsset).bytes);
                    // 存放lua文件的字典中的数量大于等于filelist中lua文件数量
                    // 说明lua文件加载都完成了
                    if (m_LuaScripts.Count >= LuaNames.Count)
                    {
                        // 向外部传输完成回调
                        GameManager.Event.Fire((int)GameEvent.StartLua);
                        LuaNames.Clear();
                        LuaNames = null;
                    }
                });
            }
        }



        private void AddLuaScript(string assetName, byte[] luaScript)
        {
            m_LuaScripts[assetName] = luaScript;
        }

#if UNITY_EDITOR
        void EditorLoadLuaScript()
        {
            string[] luaFiles = Directory.GetFiles(PathUtil.LuaPath, "*.bytes", SearchOption.AllDirectories);
            for(int i=0; i< luaFiles.Length; i++)
            {
                string fileName = PathUtil.GetStandardPath(luaFiles[i]);
                byte[] file =  File.ReadAllBytes(fileName);
                AddLuaScript(PathUtil.GetUnityPath(fileName), file);
            }
            GameManager.Event.Fire((int)GameEvent.StartLua);
        }
#endif

        private void Update()
        {
            if (luaEnv != null)
            {
                luaEnv.Tick();
            }
        }
        private void OnDestroy()
        {
            if(luaEnv != null)
            {
                luaEnv.Dispose();
                luaEnv = null;
            }
        }
    }
}
