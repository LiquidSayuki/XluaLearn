using behavoiur;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LuaSceneLogic : LuaBehaviour
{
    public string SceneName;

    Action m_LuaActive;
    Action m_LuaDeactive;
    Action m_LuaOnEnter;
    Action m_LuaOnQuit;

    public override void Init(string luaName)
    {
        base.Init(luaName);
        m_ScriptEnv.Get("OnActive", out m_LuaActive);
        m_ScriptEnv.Get("OnDeactive", out m_LuaDeactive);
        m_ScriptEnv.Get("OnEnter", out m_LuaOnEnter);
        m_ScriptEnv.Get("OnQuit", out m_LuaOnQuit);
    }

    public void OnActive()
    {
        m_LuaActive?.Invoke();
    }
    public void OnDeactive()
    {
        m_LuaDeactive?.Invoke();
    }
    public void OnEnter()
    {
        m_LuaOnEnter?.Invoke();
    }
    public void OnQuit()
    {
        m_LuaOnQuit?.Invoke();
    }

    protected override void Clear()
    {
        base.Clear();
        m_LuaActive = null;
        m_LuaDeactive = null;
        m_LuaOnEnter = null;
        m_LuaOnQuit = null;
    }
}
