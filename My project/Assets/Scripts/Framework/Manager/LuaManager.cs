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

        //����lua�ű�����
        private Dictionary<string, byte[]> m_LuaScripts;

        //Lua�����
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
        ///  ��װ���ⲿ����Lua�ű��ķ���
        /// </summary>
        /// <param name="name">Lua�ű���</param>
        public void StartLua(string name)
        {
            luaEnv.DoString(string.Format("require '{0}'", name));
        }

        // �õ�lua�ű�����
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
                    // ���lua�ļ����ֵ��е��������ڵ���filelist��lua�ļ�����
                    // ˵��lua�ļ����ض������
                    if (m_LuaScripts.Count >= LuaNames.Count)
                    {
                        // ���ⲿ������ɻص�
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
