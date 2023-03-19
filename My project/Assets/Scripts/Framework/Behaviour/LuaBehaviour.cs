using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

namespace behavoiur
{
    public class LuaBehaviour : MonoBehaviour
    {
        private LuaEnv m_luaEnv = GameManager.Lua.luaEnv;
        protected LuaTable m_ScriptEnv;
        // Unity��������ת��Ϊlua��������
        private Action m_LuaInit;
        private Action m_LuaUpdate;
        private Action m_LuaOnDestroy;


        private void Awake()
        {

            m_ScriptEnv = m_luaEnv.NewTable();
            // Ϊÿ���ű�����һ�������Ļ�������һ���̶��Ϸ�ֹ�ű���ȫ�ֱ�����������ͻ
            LuaTable meta = m_luaEnv.NewTable();
            meta.Set("__index", m_luaEnv.Global);
            m_ScriptEnv.SetMetaTable(meta);
            meta.Dispose();

            m_ScriptEnv.Set("self", this);
        }

        public virtual void Init(string luaName)
        {
            m_luaEnv.DoString(GameManager.Lua.GetLuaScript(luaName), luaName, m_ScriptEnv);

            m_ScriptEnv.Get("Init", out m_LuaInit);
            m_ScriptEnv.Get("Update", out m_LuaUpdate);
            m_LuaInit?.Invoke();
        }

        private void Update()
        {
            m_LuaUpdate?.Invoke();
        }

        //����
        protected virtual void Clear()
        {
            m_LuaInit=null;
            m_LuaUpdate=null;
            m_LuaOnDestroy = null;
            m_ScriptEnv?.Dispose();
            m_ScriptEnv = null;

        }

        private void OnDestroy()
        {
            m_LuaOnDestroy?.Invoke();
            Clear();
        }

        private void OnApplicationQuit()
        {
            Clear();
        }
    }
}

