using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[XLua.LuaCallCSharp]
public static class UnityEx
{
    public static void OnClickSet(this Button button, object callback)
    {
        XLua.LuaFunction func = callback as XLua.LuaFunction;
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(
            () =>
            {
                func?.Call();
            });
    }

    public static void OnValueChangedSet(this Slider slider, object callback)
    {
        XLua.LuaFunction func = callback as XLua.LuaFunction;
        slider.onValueChanged.RemoveAllListeners();
        slider.onValueChanged.AddListener((float value) =>
        {
            func?.Call(value);
        });
    }
}
