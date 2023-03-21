using behavoiur;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LuaEntityLogic : LuaBehaviour
{
    Action m_LuaOnShow;
    Action m_LuaOnHide;
    Action m_LuaOnFixedUpdate;
    public override void Init(string luaName)
    {
        base.Init(luaName);
        m_ScriptEnv.Get("OnShow", out m_LuaOnShow);
        m_ScriptEnv.Get("OnHide", out m_LuaOnHide);
        m_ScriptEnv.Get("FixedUpdate", out m_LuaOnFixedUpdate);
    }

    public void OnShow()
    {
        m_LuaOnShow?.Invoke();
    }
    public void OnHide()
    {
        m_LuaOnHide?.Invoke();
    }

    public void FixedUpdate()
    {
        m_LuaOnFixedUpdate?.Invoke();
    }

    protected override void Clear()
    {
        base.Clear();
        m_LuaOnHide = null;
        m_LuaOnShow = null;
    }
}


